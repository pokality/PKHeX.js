/**
 * PKHeX WASM Library - TypeScript Definitions
 * 
 * This library provides comprehensive Pokemon save file editing functionality
 * through WebAssembly. All API methods return objects directly (no JSON parsing needed).
 */

/**
 * Handle identifier returned by LoadSave
 */
export type SaveHandle = number;

/**
 * Success response wrapper
 */
export interface SuccessResponse {
  success: true;
}

/**
 * Error response structure
 */
export interface ErrorResponse {
  error: string;
  code?: string;
}

/**
 * API response type - either success with data or error
 */
export type ApiResult<T> = (SuccessResponse & T) | ErrorResponse;

/**
 * Save file information
 */
export interface SaveFileInfo {
  generation: string;
  gameVersion: string;
  ot: string;
  tid: number;
  sid: number;
  boxCount: number;
}

/**
 * Save file handle returned by LoadSave
 */
export interface SaveFileHandle {
  handle: SaveHandle;
}

/**
 * Pokemon summary for list views
 */
export interface PokemonSummary {
  box: number;
  slot: number;
  species: number;
  speciesName: string;
  level: number;
  isEgg: boolean;
  isShiny: boolean;
}

/**
 * Detailed Pokemon data
 */
export interface PokemonDetail {
  species: number;
  speciesName: string;
  nickname: string;
  level: number;
  nature: number;
  natureName: string;
  ability: number;
  abilityName: string;
  heldItem: number;
  heldItemName: string;
  moves: number[];
  moveNames: string[];
  ivs: number[];
  evs: number[];
  stats: number[];
  gender: number;
  isShiny: boolean;
  isEgg: boolean;
  ot_Name: string;
  ot_Gender: number;
  pid: number;
  ball: number;
  metLevel: number;
  metLocation: number;
  metLocationName: string;
}

/**
 * Pokemon modifications for editing
 */
export interface PokemonModifications {
  species?: number;
  nickname?: string;
  level?: number;
  nature?: number;
  ability?: number;
  heldItem?: number;
  moves?: number[];
  ivs?: number[];
  evs?: number[];
  gender?: number;
  isShiny?: boolean;
  ot_Name?: string;
  ball?: number;
}

/**
 * Legality check result
 */
export interface LegalityResult {
  valid: boolean;
  errors: string[];
  parsed: string;
}

/**
 * Ribbon data
 */
export interface RibbonData {
  name: string;
  hasRibbon: boolean;
  ribbonCount: number;
  type: string;
}

/**
 * Contest stats
 */
export interface ContestStats {
  cool: number;
  beauty: number;
  cute: number;
  smart: number;
  tough: number;
  sheen: number;
}

/**
 * Trainer information
 */
export interface TrainerInfo {
  ot: string;
  tid: number;
  sid: number;
  gender: number;
  language: number;
  money: number;
  playedHours: number;
  playedMinutes: number;
  playedSeconds: number;
}

/**
 * Trainer card data
 */
export interface TrainerCard {
  ot: string;
  tid: number;
  sid: number;
  money: number;
  startDate: string | null;
  fame: number;
}

/**
 * Trainer appearance customization
 */
export interface TrainerAppearance {
  skin: number;
  hair: number;
  top: number;
  bottom: number;
  shoes: number;
  accessory: number;
  bag: number;
  hat: number;
}

/**
 * Badge data
 */
export interface BadgeData {
  badgeCount: number;
  badges: boolean[];
}

/**
 * Box information
 */
export interface BoxInfo {
  name: string;
  wallpaper: number;
}

/**
 * Daycare data
 */
export interface DaycareData {
  slot1Species: number;
  slot1SpeciesName: string;
  slot1Level: number;
  slot2Species: number;
  slot2SpeciesName: string;
  slot2Level: number;
  hasEgg: boolean;
}

/**
 * Item slot in inventory
 */
export interface ItemSlot {
  itemId: number;
  itemName: string;
  count: number;
}

/**
 * Pouch data (inventory category)
 */
export interface PouchData {
  pouchType: string;
  pouchIndex: number;
  items: ItemSlot[];
  totalSlots: number;
}

/**
 * Pokedex entry
 */
export interface PokedexEntry {
  species: number;
  speciesName: string;
  seen: boolean;
  caught: boolean;
}

/**
 * Hall of Fame entry
 */
export interface HallOfFameEntry {
  index: number;
  timestamp: string;
  team: PokemonSummary[];
}

/**
 * Mystery Gift card
 */
export interface MysteryGiftCard {
  index: number;
  type: string;
  cardTitle: string;
  isItem: boolean;
  isPokemon: boolean;
  isBP: boolean;
  itemId: number;
  species: number;
  level: number;
  isShiny: boolean;
  isEgg: boolean;
}

/**
 * Named entity (species, move, ability, item, etc.)
 */
export interface NamedEntity {
  id: number;
  name: string;
}

/**
 * Success message response
 */
export interface MessageResponse {
  message: string;
}

/**
 * PKHeX WASM API
 * 
 * All methods return objects directly - no JSON parsing needed!
 * Successful responses include { success: true, ...data }
 * Error responses include { error: string, code?: string }
 */
export interface PKHeXApi {
  /**
   * Load a save file from base64-encoded data
   * @param base64Data Base64-encoded save file data
   * @returns Object with handle property or error
   */
  LoadSave(base64Data: string): ApiResult<SaveFileHandle>;

  /**
   * Get information about a loaded save file
   * @param handle Session identifier from LoadSave
   * @returns Object with save file information or error
   */
  GetSaveInfo(handle: SaveHandle): ApiResult<SaveFileInfo>;

  /**
   * Export a save file as base64-encoded data
   * @param handle Session identifier
   * @returns Object with base64Data property or error
   */
  ExportSave(handle: SaveHandle): ApiResult<{ base64Data: string }>;

  /**
   * Dispose a save file session and free memory
   * @param handle Session identifier
   * @returns Success message or error
   */
  DisposeSave(handle: SaveHandle): ApiResult<MessageResponse>;

  /**
   * Get all non-empty Pokemon from boxes
   * @param handle Session identifier
   * @returns Object with pokemon array or error
   */
  GetAllPokemon(handle: SaveHandle): ApiResult<{ pokemon: PokemonSummary[] }>;

  /**
   * Get party Pokemon
   * @param handle Session identifier
   * @returns Object with party array or error
   */
  GetParty(handle: SaveHandle): ApiResult<{ party: PokemonSummary[] }>;

  /**
   * Get detailed Pokemon data from a box slot
   * @param handle Session identifier
   * @param box Box number (0-based)
   * @param slot Slot number (0-based)
   * @returns Pokemon details or error
   */
  GetPokemon(handle: SaveHandle, box: number, slot: number): ApiResult<PokemonDetail>;

  /**
   * Get detailed Pokemon data from a party slot
   * @param handle Session identifier
   * @param slot Party slot number (0-5)
   * @returns Pokemon details or error
   */
  GetPartySlot(handle: SaveHandle, slot: number): ApiResult<PokemonDetail>;

  /**
   * Modify a Pokemon's attributes
   * @param handle Session identifier
   * @param box Box number
   * @param slot Slot number
   * @param modificationsJson JSON string of PokemonModifications
   * @returns Success message or error
   */
  ModifyPokemon(handle: SaveHandle, box: number, slot: number, modificationsJson: string): ApiResult<MessageResponse>;

  /**
   * Set a Pokemon from base64 PKM data
   * @param handle Session identifier
   * @param box Box number
   * @param slot Slot number
   * @param base64PkmData Base64-encoded PKM data
   * @returns Success message or error
   */
  SetPokemon(handle: SaveHandle, box: number, slot: number, base64PkmData: string): ApiResult<MessageResponse>;

  /**
   * Delete a Pokemon from a box slot
   * @param handle Session identifier
   * @param box Box number
   * @param slot Slot number
   * @returns Success message or error
   */
  DeletePokemon(handle: SaveHandle, box: number, slot: number): ApiResult<MessageResponse>;

  /**
   * Move or swap Pokemon between slots
   * @param handle Session identifier
   * @param fromBox Source box number
   * @param fromSlot Source slot number
   * @param toBox Destination box number
   * @param toSlot Destination slot number
   * @returns Success message or error
   */
  MovePokemon(handle: SaveHandle, fromBox: number, fromSlot: number, toBox: number, toSlot: number): ApiResult<MessageResponse>;

  /**
   * Check legality of a Pokemon
   * @param handle Session identifier
   * @param box Box number
   * @param slot Slot number
   * @returns Legality result or error
   */
  CheckLegality(handle: SaveHandle, box: number, slot: number): ApiResult<LegalityResult>;

  /**
   * Legalize a Pokemon (heal, set moves, refresh checksum)
   * @param handle Session identifier
   * @param box Box number
   * @param slot Slot number
   * @returns Success message or error
   */
  LegalizePokemon(handle: SaveHandle, box: number, slot: number): ApiResult<MessageResponse>;

  /**
   * Export Pokemon in Showdown format
   * @param handle Session identifier
   * @param box Box number
   * @param slot Slot number
   * @returns Object with showdownText property or error
   */
  ExportShowdown(handle: SaveHandle, box: number, slot: number): ApiResult<{ showdownText: string }>;

  /**
   * Import Pokemon from Showdown format
   * @param handle Session identifier
   * @param box Box number
   * @param slot Slot number
   * @param showdownText Showdown format text
   * @returns Success message or error
   */
  ImportShowdown(handle: SaveHandle, box: number, slot: number, showdownText: string): ApiResult<MessageResponse>;

  /**
   * Get all ribbons for a Pokemon
   * @param handle Session identifier
   * @param box Box number
   * @param slot Slot number
   * @returns Object with ribbons array or error
   */
  GetRibbons(handle: SaveHandle, box: number, slot: number): ApiResult<{ ribbons: RibbonData[] }>;

  /**
   * Get ribbon count for a Pokemon
   * @param handle Session identifier
   * @param box Box number
   * @param slot Slot number
   * @returns Object with ribbonCount property or error
   */
  GetRibbonCount(handle: SaveHandle, box: number, slot: number): ApiResult<{ ribbonCount: number }>;

  /**
   * Set a ribbon on a Pokemon
   * @param handle Session identifier
   * @param box Box number
   * @param slot Slot number
   * @param ribbonName Name of the ribbon property
   * @param value Ribbon value
   * @returns Success message or error
   */
  SetRibbon(handle: SaveHandle, box: number, slot: number, ribbonName: string, value: boolean): ApiResult<MessageResponse>;

  /**
   * Get contest stats for a Pokemon
   * @param handle Session identifier
   * @param box Box number
   * @param slot Slot number
   * @returns Contest stats or error
   */
  GetContestStats(handle: SaveHandle, box: number, slot: number): ApiResult<ContestStats>;

  /**
   * Set a contest stat for a Pokemon
   * @param handle Session identifier
   * @param box Box number
   * @param slot Slot number
   * @param statName Stat name (cool, beauty, cute, smart, tough, sheen)
   * @param value Stat value
   * @returns Success message or error
   */
  SetContestStat(handle: SaveHandle, box: number, slot: number, statName: string, value: number): ApiResult<MessageResponse>;

  /**
   * Get trainer information
   * @param handle Session identifier
   * @returns Trainer info or error
   */
  GetTrainerInfo(handle: SaveHandle): ApiResult<TrainerInfo>;

  /**
   * Set trainer information
   * @param handle Session identifier
   * @param trainerDataJson JSON string of TrainerInfo
   * @returns Success message or error
   */
  SetTrainerInfo(handle: SaveHandle, trainerDataJson: string): ApiResult<MessageResponse>;

  /**
   * Get trainer card data
   * @param handle Session identifier
   * @returns Trainer card or error
   */
  GetTrainerCard(handle: SaveHandle): ApiResult<TrainerCard>;

  /**
   * Get trainer appearance customization
   * @param handle Session identifier
   * @returns Trainer appearance or error
   */
  GetTrainerAppearance(handle: SaveHandle): ApiResult<TrainerAppearance>;

  /**
   * Set trainer appearance customization
   * @param handle Session identifier
   * @param appearanceJson JSON string of TrainerAppearance
   * @returns Success message or error
   */
  SetTrainerAppearance(handle: SaveHandle, appearanceJson: string): ApiResult<MessageResponse>;

  /**
   * Get all box names
   * @param handle Session identifier
   * @returns Object with boxNames array or error
   */
  GetBoxNames(handle: SaveHandle): ApiResult<{ boxNames: string[] }>;

  /**
   * Get box wallpapers
   * @param handle Session identifier
   * @returns Object with boxes array or error
   */
  GetBoxWallpapers(handle: SaveHandle): ApiResult<{ boxes: BoxInfo[] }>;

  /**
   * Set box wallpaper
   * @param handle Session identifier
   * @param box Box number
   * @param wallpaperId Wallpaper ID
   * @returns Success message or error
   */
  SetBoxWallpaper(handle: SaveHandle, box: number, wallpaperId: number): ApiResult<MessageResponse>;

  /**
   * Get daycare data
   * @param handle Session identifier
   * @returns Daycare data or error
   */
  GetDaycare(handle: SaveHandle): ApiResult<DaycareData>;

  /**
   * Get all items in pouches
   * @param handle Session identifier
   * @returns Object with pouches array or error
   */
  GetPouchItems(handle: SaveHandle): ApiResult<{ pouches: PouchData[] }>;

  /**
   * Add item to pouch
   * @param handle Session identifier
   * @param itemId Item ID
   * @param count Item count
   * @param pouchIndex Pouch index
   * @returns Success message or error
   */
  AddItemToPouch(handle: SaveHandle, itemId: number, count: number, pouchIndex: number): ApiResult<MessageResponse>;

  /**
   * Remove item from pouch
   * @param handle Session identifier
   * @param itemId Item ID
   * @param count Item count
   * @returns Success message or error
   */
  RemoveItemFromPouch(handle: SaveHandle, itemId: number, count: number): ApiResult<MessageResponse>;

  /**
   * Get Pokedex data
   * @param handle Session identifier
   * @returns Object with pokedex array or error
   */
  GetPokedex(handle: SaveHandle): ApiResult<{ pokedex: PokedexEntry[] }>;

  /**
   * Set Pokedex seen status
   * @param handle Session identifier
   * @param species Species ID
   * @param form Form ID
   * @returns Success message or error
   */
  SetPokedexSeen(handle: SaveHandle, species: number, form: number): ApiResult<MessageResponse>;

  /**
   * Set Pokedex caught status
   * @param handle Session identifier
   * @param species Species ID
   * @param form Form ID
   * @returns Success message or error
   */
  SetPokedexCaught(handle: SaveHandle, species: number, form: number): ApiResult<MessageResponse>;

  /**
   * Get badge data
   * @param handle Session identifier
   * @returns Badge data or error
   */
  GetBadges(handle: SaveHandle): ApiResult<BadgeData>;

  /**
   * Set badge status
   * @param handle Session identifier
   * @param badgeIndex Badge index
   * @param value Badge status
   * @returns Success message or error
   */
  SetBadge(handle: SaveHandle, badgeIndex: number, value: boolean): ApiResult<MessageResponse>;

  /**
   * Get Battle Points
   * @param handle Session identifier
   * @returns Object with battlePoints property or error
   */
  GetBattlePoints(handle: SaveHandle): ApiResult<{ battlePoints: number }>;

  /**
   * Set Battle Points
   * @param handle Session identifier
   * @param battlePoints Battle Points value
   * @returns Success message or error
   */
  SetBattlePoints(handle: SaveHandle, battlePoints: number): ApiResult<MessageResponse>;

  /**
   * Get Game Corner coins
   * @param handle Session identifier
   * @returns Object with coins property or error
   */
  GetCoins(handle: SaveHandle): ApiResult<{ coins: number }>;

  /**
   * Set Game Corner coins
   * @param handle Session identifier
   * @param coins Coins value
   * @returns Success message or error
   */
  SetCoins(handle: SaveHandle, coins: number): ApiResult<MessageResponse>;

  /**
   * Get trainer records/statistics
   * @param handle Session identifier
   * @returns Object with records or error
   */
  GetRecords(handle: SaveHandle): ApiResult<{ records: Record<string, number> }>;

  /**
   * Set a specific record value
   * @param handle Session identifier
   * @param recordIndex Record index
   * @param value Record value
   * @returns Success message or error
   */
  SetRecord(handle: SaveHandle, recordIndex: number, value: number): ApiResult<MessageResponse>;

  /**
   * Get event flag value
   * @param handle Session identifier
   * @param flagIndex Flag index
   * @returns Object with flag value or error
   */
  GetEventFlag(handle: SaveHandle, flagIndex: number): ApiResult<{ flag: boolean }>;

  /**
   * Set event flag value
   * @param handle Session identifier
   * @param flagIndex Flag index
   * @param value Flag value
   * @returns Success message or error
   */
  SetEventFlag(handle: SaveHandle, flagIndex: number, value: boolean): ApiResult<MessageResponse>;

  /**
   * Get Hall of Fame entries
   * @param handle Session identifier
   * @returns Object with entries array or error
   */
  GetHallOfFame(handle: SaveHandle): ApiResult<{ entries: HallOfFameEntry[] }>;

  /**
   * Set Hall of Fame entry
   * @param handle Session identifier
   * @param index Entry index
   * @param teamJson JSON string of PokemonSummary array
   * @returns Success message or error
   */
  SetHallOfFameEntry(handle: SaveHandle, index: number, teamJson: string): ApiResult<MessageResponse>;

  /**
   * Get total seconds played
   * @param handle Session identifier
   * @returns Object with secondsPlayed property or error
   */
  GetSecondsPlayed(handle: SaveHandle): ApiResult<{ secondsPlayed: number }>;

  /**
   * Get seconds from game start to save creation
   * @param handle Session identifier
   * @returns Object with secondsToStart property or error
   */
  GetSecondsToStart(handle: SaveHandle): ApiResult<{ secondsToStart: number }>;

  /**
   * Get seconds from game start to Hall of Fame
   * @param handle Session identifier
   * @returns Object with secondsToFame property or error
   */
  GetSecondsToFame(handle: SaveHandle): ApiResult<{ secondsToFame: number }>;

  /**
   * Set game time
   * @param handle Session identifier
   * @param hours Hours played
   * @param minutes Minutes played
   * @param seconds Seconds played
   * @returns Success message or error
   */
  SetGameTime(handle: SaveHandle, hours: number, minutes: number, seconds: number): ApiResult<MessageResponse>;

  /**
   * Get all Mystery Gift cards
   * @param handle Session identifier
   * @returns Object with gifts array or error
   */
  GetMysteryGifts(handle: SaveHandle): ApiResult<{ gifts: MysteryGiftCard[] }>;

  /**
   * Get specific Mystery Gift card
   * @param handle Session identifier
   * @param index Gift index
   * @returns Object with card or error
   */
  GetMysteryGiftCard(handle: SaveHandle, index: number): ApiResult<{ card: MysteryGiftCard }>;

  /**
   * Set Mystery Gift card
   * @param handle Session identifier
   * @param index Gift index
   * @param cardJson JSON string of MysteryGiftCard
   * @returns Success message or error
   */
  SetMysteryGiftCard(handle: SaveHandle, index: number, cardJson: string): ApiResult<MessageResponse>;

  /**
   * Get Mystery Gift received flags
   * @param handle Session identifier
   * @returns Object with flags array or error
   */
  GetMysteryGiftFlags(handle: SaveHandle): ApiResult<{ flags: boolean[] }>;

  /**
   * Delete Mystery Gift
   * @param handle Session identifier
   * @param index Gift index
   * @returns Success message or error
   */
  DeleteMysteryGift(handle: SaveHandle, index: number): ApiResult<MessageResponse>;

  /**
   * Get species name by ID
   * @param speciesId Species ID
   * @returns Object with name or error
   */
  GetSpeciesName(speciesId: number): ApiResult<{ name: string }>;

  /**
   * Get all species names
   * @returns Object with species array or error
   */
  GetAllSpecies(): ApiResult<{ species: NamedEntity[] }>;

  /**
   * Get move name by ID
   * @param moveId Move ID
   * @returns Object with name or error
   */
  GetMoveName(moveId: number): ApiResult<{ name: string }>;

  /**
   * Get all move names
   * @returns Object with moves array or error
   */
  GetAllMoves(): ApiResult<{ moves: NamedEntity[] }>;

  /**
   * Get ability name by ID
   * @param abilityId Ability ID
   * @returns Object with name or error
   */
  GetAbilityName(abilityId: number): ApiResult<{ name: string }>;

  /**
   * Get all ability names
   * @returns Object with abilities array or error
   */
  GetAllAbilities(): ApiResult<{ abilities: NamedEntity[] }>;

  /**
   * Get item name by ID
   * @param itemId Item ID
   * @returns Object with name or error
   */
  GetItemName(itemId: number): ApiResult<{ name: string }>;

  /**
   * Get all item names
   * @returns Object with items array or error
   */
  GetAllItems(): ApiResult<{ items: NamedEntity[] }>;

  /**
   * Get nature name by ID
   * @param natureId Nature ID
   * @returns Object with name or error
   */
  GetNatureName(natureId: number): ApiResult<{ name: string }>;

  /**
   * Get all nature names
   * @returns Object with natures array or error
   */
  GetAllNatures(): ApiResult<{ natures: NamedEntity[] }>;

  /**
   * Get type name by ID
   * @param typeId Type ID
   * @returns Object with name or error
   */
  GetTypeName(typeId: number): ApiResult<{ name: string }>;

  /**
   * Get all type names
   * @returns Object with types array or error
   */
  GetAllTypes(): ApiResult<{ types: NamedEntity[] }>;
}

/**
 * Type guard to check if response is an error
 * @param response API response
 * @returns True if response is an error
 */
export function isError(response: any): response is ErrorResponse;

/**
 * Type guard to check if response is successful
 * @param response API response
 * @returns True if response is successful
 */
export function isSuccess<T>(response: ApiResult<T>): response is SuccessResponse & T;

/**
 * Load the PKHeX WASM module
 * @returns Promise that resolves to the PKHeXApi instance
 */
export function loadPKHeX(): Promise<PKHeXApi>;
