/**
 * WASI Component Test Setup
 *
 * Imports the jco-transpiled WASI component and provides helpers
 * for loading test save files.
 *
 * IMPORTANT: Due to a NativeAOT-LLVM bug, loadSave can only be
 * called twice before thread statics corrupt. We load a single
 * shared save handle at module init and reuse it across all tests.
 */

import { readFileSync } from 'fs';
import { dirname, join } from 'path';
import { fileURLToPath } from 'url';

const __dirname = dirname(fileURLToPath(import.meta.url));

import {
  run,
  saveOps,
  pokemonOps,
  partyOps,
  pkmOps,
  trainerOps,
  storageOps,
  itemOps,
  gameDataOps,
  progressOps,
  featureOps,
  zaOps,
} from '../dist/pkhex.js';

// Initialize the .NET runtime inside the WASM component.
const g = globalThis as Record<string, unknown>;
if (!g.__pkhex_runtime_initialized) {
  run.run();
  g.__pkhex_runtime_initialized = true;
}

// Load the shared test save handle once per process.
// Reuse across all tests to avoid the 3-load crash bug.
const savePath = join(__dirname, 'PKHeX.Tests', 'TestData', 'emerald.sav');
const saveData = new Uint8Array(readFileSync(savePath));

let _sharedHandle: number | null = null;

function getSharedHandle(): number {
  if (_sharedHandle === null) {
    _sharedHandle = saveOps.loadSave(saveData);
  }
  return _sharedHandle;
}

export {
  saveOps,
  pokemonOps,
  partyOps,
  pkmOps,
  trainerOps,
  storageOps,
  itemOps,
  gameDataOps,
  progressOps,
  featureOps,
  zaOps,
};

/**
 * Creates a fresh test save. WARNING: due to a NativeAOT-LLVM bug,
 * calling loadSave more than twice per process will crash. Prefer
 * useTestSave() which reuses a shared handle.
 */
export function createTestSave(): number {
  return saveOps.loadSave(saveData);
}

export function disposeTestSave(handle: number): void {
  try {
    saveOps.disposeSave(handle);
  } catch {
    // Ignore errors during cleanup
  }
}

/**
 * Provides a shared save handle for testing. The handle persists
 * across all tests in the process — mutations accumulate.
 * For tests that need isolation, use createTestSave() sparingly.
 */
export function useTestSave(fn: (handle: number) => void): void {
  fn(getSharedHandle());
}

/**
 * @deprecated Use useTestSave() instead. This creates+disposes a
 * fresh save handle per call, which hits the 3-load crash limit.
 */
export async function withTestSave<T>(
  fn: (handle: number) => T | Promise<T>
): Promise<T> {
  return await fn(getSharedHandle());
}
