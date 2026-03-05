import { saveOps } from '../dist/pkhex.js';

/**
 * FinalizationRegistry for automatic save file handle cleanup.
 *
 * When a SaveFile instance is garbage collected, the associated handle
 * will be automatically disposed to prevent memory leaks in the WASM module.
 */
let handleRegistry: FinalizationRegistry<{ handle: number; dispose: (handle: number) => void }> | null = null;

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
 * Wraps a WASI component save handle and ensures proper cleanup.
 * Explicit disposal via `dispose()` is recommended for deterministic cleanup.
 */
export class SaveFile {
  private _handle: number;
  private _disposed = false;

  constructor(handle: number) {
    this._handle = handle;

    if (handleRegistry) {
      handleRegistry.register(this, { handle, dispose: saveOps.disposeSave }, this);
    }
  }

  get handle(): number {
    if (this._disposed) {
      throw new Error('SaveFile has been disposed');
    }
    return this._handle;
  }

  get disposed(): boolean {
    return this._disposed;
  }

  dispose(): void {
    if (this._disposed) return;
    this._disposed = true;

    if (handleRegistry) {
      handleRegistry.unregister(this);
    }

    try {
      saveOps.disposeSave(this._handle);
    } catch {
      // Ignore errors during disposal
    }
  }

  [Symbol.dispose](): void {
    this.dispose();
  }
}
