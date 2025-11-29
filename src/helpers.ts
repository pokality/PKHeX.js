import type { ErrorResponse, SuccessResponse, ApiResult, SaveHandle } from './index';

export function isError(response: any): response is ErrorResponse {
  return Boolean(response && typeof response === 'object' && 'error' in response);
}

export function isSuccess<T>(response: ApiResult<T>): response is SuccessResponse & T {
  return response && typeof response === 'object' && 'success' in response && response.success === true;
}

export function unwrap<T>(response: ApiResult<T>): SuccessResponse & T {
  if (isError(response)) {
    throw new Error(`PKHeX API Error [${response.code || 'UNKNOWN'}]: ${response.error}`);
  }
  return response as SuccessResponse & T;
}

export function getError(response: any): string | null {
  return isError(response) ? response.error : null;
}

/**
 * FinalizationRegistry for automatic save file handle cleanup.
 *
 * When a SaveFile instance is garbage collected, the associated handle
 * will be automatically disposed to prevent memory leaks in the WASM module.
 *
 * Usage:
 * ```ts
 * const save = new SaveFile(handle, disposeFunction);
 * // When save goes out of scope and is GC'd, handle is automatically disposed
 * ```
 */
let handleRegistry: FinalizationRegistry<{ handle: SaveHandle; dispose: (handle: SaveHandle) => void }> | null = null;

// Initialize registry if FinalizationRegistry is available (modern browsers/Node.js)
if (typeof FinalizationRegistry !== 'undefined') {
  handleRegistry = new FinalizationRegistry(({ handle, dispose }) => {
    try {
      dispose(handle);
    } catch {
      // Ignore errors during finalization (handle may already be disposed)
    }
  });
}

/**
 * SaveFile wrapper that automatically disposes the handle when garbage collected.
 *
 * This class wraps a save file handle and ensures proper cleanup when the
 * SaveFile instance is no longer referenced and gets garbage collected.
 *
 * Note: Explicit disposal via `dispose()` is still recommended for deterministic
 * cleanup, but automatic cleanup will occur if the instance is abandoned.
 */
export class SaveFile {
  private _handle: SaveHandle;
  private _disposed = false;
  private readonly _disposeCallback: (handle: SaveHandle) => void;

  /**
   * Create a new SaveFile wrapper.
   * @param handle - The save file handle from LoadSave
   * @param disposeCallback - Function to call to dispose the handle (typically PKHeXApi.DisposeSave)
   */
  constructor(handle: SaveHandle, disposeCallback: (handle: SaveHandle) => void) {
    this._handle = handle;
    this._disposeCallback = disposeCallback;

    // Register for automatic cleanup if FinalizationRegistry is available
    if (handleRegistry) {
      handleRegistry.register(this, { handle, dispose: disposeCallback }, this);
    }
  }

  /**
   * Get the underlying handle for use with PKHeX API methods.
   * @throws Error if the save file has been disposed
   */
  get handle(): SaveHandle {
    if (this._disposed) {
      throw new Error('SaveFile has been disposed');
    }
    return this._handle;
  }

  /**
   * Check if this save file has been disposed.
   */
  get disposed(): boolean {
    return this._disposed;
  }

  /**
   * Explicitly dispose the save file handle.
   *
   * It's recommended to call this when you're done with a save file
   * for immediate resource cleanup, rather than waiting for GC.
   */
  dispose(): void {
    if (this._disposed) return;

    this._disposed = true;

    // Unregister from finalization registry since we're disposing manually
    if (handleRegistry) {
      handleRegistry.unregister(this);
    }

    try {
      this._disposeCallback(this._handle);
    } catch {
      // Ignore errors during disposal
    }
  }

  /**
   * Symbol.dispose support for using statement (TC39 proposal)
   */
  [Symbol.dispose](): void {
    this.dispose();
  }
}
