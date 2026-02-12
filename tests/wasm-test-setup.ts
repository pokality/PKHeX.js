/**
 * WASM Test Setup
 * 
 * Loads the WASM module once and provides it to all tests.
 * This avoids the overhead of loading WASM for each test file.
 */

import { createRequire } from 'module';
import { readFileSync } from 'fs';
import { fileURLToPath } from 'url';
import { dirname, join } from 'path';

const require = createRequire(import.meta.url);
const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

let wasmModule: any = null;
let isInitialized = false;
let initPromise: Promise<WASMTestContext> | null = null;

export interface WASMTestContext {
  rawApi: any;
  isReady: boolean;
}

/**
 * Initialize the WASM module (call once before all tests)
 */
export async function initializeWASM(): Promise<WASMTestContext> {
  // Return cached result if already initialized
  if (isInitialized && wasmModule) {
    return { rawApi: wasmModule, isReady: true };
  }

  // Return existing promise if initialization is in progress
  if (initPromise) {
    return initPromise;
  }

  // Start initialization
  initPromise = (async () => {
    try {
      // Import the dotnet runtime
      const distPath = join(__dirname, '..', 'dist');
      const dotnetPath = join(distPath, 'dotnet.js');

      // Dynamic import of the dotnet module
      const dotnetModule = await import(dotnetPath);
      const { dotnet } = dotnetModule;

      if (!dotnet || typeof dotnet.create !== 'function') {
        throw new Error(`Invalid dotnet module structure. Got: ${JSON.stringify(Object.keys(dotnetModule))}`);
      }

      // Read boot config from disk and convert to array-based format
      // that the .NET 10 runtime's withConfig() expects
      const bootConfigPath = join(distPath, 'blazor.boot.json');
      const bootConfig = JSON.parse(readFileSync(bootConfigPath, 'utf8'));

      if (bootConfig.resources) {
        for (const key of Object.keys(bootConfig.resources)) {
          const val = bootConfig.resources[key];
          if (val && typeof val === 'object' && !Array.isArray(val)) {
            bootConfig.resources[key] = Object.entries(val).map(
              ([name, hash]) => ({ name, hash: hash as string })
            );
          }
        }
      }

      // Create the runtime with config passed directly
      const runtime = await dotnet.withConfig(bootConfig).create();
      
      // Get the PKHeX assembly exports
      const exports = await runtime.getAssemblyExports('PKHeX.dll');
      
      // The exports structure is: exports.PKHeX.Api.PKHeXApi
      if (!exports || !exports.PKHeX || !exports.PKHeX.Api || !exports.PKHeX.Api.PKHeXApi) {
        // Debug output
        console.error('Assembly exports structure:');
        console.error('- exports:', !!exports);
        if (exports) {
          console.error('- exports keys:', Object.keys(exports));
          if (exports.PKHeX) {
            console.error('- PKHeX keys:', Object.keys(exports.PKHeX));
            if (exports.PKHeX.Api) {
              console.error('- Api keys:', Object.keys(exports.PKHeX.Api));
            }
          }
        }
        throw new Error(`PKHeXApi not found in assembly exports`);
      }

      wasmModule = exports.PKHeX.Api.PKHeXApi;
      isInitialized = true;

      return { rawApi: wasmModule, isReady: true };
    } catch (error) {
      console.error('Failed to initialize WASM:', error);
      initPromise = null; // Allow retry
      return { rawApi: null, isReady: false };
    }
  })();

  return initPromise;
}

/**
 * Get the initialized WASM module
 */
export function getWASM(): WASMTestContext {
  if (!isInitialized || !wasmModule) {
    throw new Error('WASM not initialized. Call initializeWASM() first.');
  }
  return { rawApi: wasmModule, isReady: true };
}

/**
 * Create a test save file for integration tests
 */
export function createTestSave(rawApi: any): number {
  const fs = require('fs');
  const path = require('path');
  
  const testSavePath = path.join(__dirname, 'PKHeX.Tests', 'TestData', 'emerald.sav');
  
  try {
    const saveData = fs.readFileSync(testSavePath);
    const base64Data = saveData.toString('base64');
    
    const result = JSON.parse(rawApi.LoadSave(base64Data));
    if (result.success) {
      return result.handle;
    }
    throw new Error(result.error || 'Failed to create test save');
  } catch (error) {
    throw new Error(`Failed to create test save: ${error}`);
  }
}

/**
 * Clean up a test save file
 */
export function disposeTestSave(rawApi: any, handle: number): void {
  try {
    rawApi.DisposeSave(handle);
  } catch (error) {
    console.warn(`Failed to dispose test save ${handle}:`, error);
  }
}

/**
 * Helper to run a test with a save file
 */
export async function withTestSave<T>(
  rawApi: any,
  testFn: (handle: number) => T | Promise<T>
): Promise<T> {
  let handle: number | null = null;
  try {
    handle = createTestSave(rawApi);
    return await testFn(handle);
  } finally {
    if (handle !== null) {
      disposeTestSave(rawApi, handle);
    }
  }
}
