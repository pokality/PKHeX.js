/**
 * PKHeX WASM Library - Main Entry Point
 * Simple API for loading and using PKHeX in JavaScript/TypeScript
 */

import { dotnet } from '../dist/dotnet.js';
import { createPKHeXApiWrapper } from './api-wrapper.js';
import type { PKHeXApi } from './index.d.ts';

// Re-export helper functions
export { isError, isSuccess, unwrap, getError } from './helpers.js';

// Re-export wrapper function
export { createPKHeXApiWrapper } from './api-wrapper.js';

// Re-export all types from the type definitions
export type {
  SaveHandle,
  SuccessResponse,
  ErrorResponse,
  ApiResult,
  SaveFileInfo,
  SaveFileHandle,
  PokemonSummary,
  PokemonDetail,
  PokemonModifications,
  LegalityResult,
  RibbonData,
  ContestStats,
  TrainerInfo,
  TrainerCard,
  TrainerAppearance,
  BadgeData,
  PIDInfo,
  PKMDataResponse,
  ShinyType,
  BoxInfo,
  DaycareData,
  ItemSlot,
  PouchData,
  PokedexEntry,
  HallOfFameEntry,
  MailMessage,
  MysteryGiftCard,
  SecretBaseData,
  EntralinkData,
  PokePelagoData,
  FestivalPlazaData,
  PokeJobsData,
  NamedEntity,
  BattleFacilityStats,
  MessageResponse,
  PKHeXApi,
} from './index.d.js';

/**
 * Setup the PKHeX WASM library via automatic initialization
 * @returns Promise that resolves to the PKHeX API
 * 
 * @example
 * ```typescript
 * import { loadPKHeX } from 'pkhex-wasm';
 * 
 * const PKHeX = await loadPKHeX();
 * const result = PKHeX.LoadSave(base64Data);
 * if (result.success) {
 *   console.log('Loaded save with handle:', result.handle);
 * }
 * ```
 */
async function setupPKHeX(): Promise<PKHeXApi> {
  const { getAssemblyExports } = await dotnet
    .withDiagnosticTracing(false)
    .withEnvironmentVariable('DOTNET_SYSTEM_GLOBALIZATION_INVARIANT', '1')
    .create();

  const exports = await getAssemblyExports('PKHeX.dll');
  
  return createPKHeXApiWrapper(exports);
}

/**
 * Create PKHeX API from existing dotnet runtime (manual initialization)
 * Use this if you've already initialized the dotnet runtime yourself
 * 
 * @param exports - The assembly exports from getAssemblyExports('PKHeX.dll')
 * @returns The PKHeX API wrapper
 * 
 * @example
 * ```typescript
 * import { dotnet } from './dist/dotnet.js';
 * import { createPKHeX } from 'pkhex-wasm';
 * 
 * const { getAssemblyExports } = await dotnet.create();
 * const exports = await getAssemblyExports('PKHeX.dll');
 * const PKHeX = createPKHeX(exports);
 * ```
 */
export function createPKHeX(exports: any): PKHeXApi {
  return createPKHeXApiWrapper(exports);
}

export default setupPKHeX;
