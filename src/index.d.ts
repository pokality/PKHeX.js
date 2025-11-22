/**
 * PKHeX WASM Library - TypeScript Definitions
 * 
 * This library provides comprehensive Pokemon save file editing functionality
 * through WebAssembly. All API methods return objects directly (no JSON parsing needed).
 */

// ============================================================================
// Common Type Definitions
// ============================================================================

/**
 * Pokemon generation (1-9)
 * - 1: Red/Blue/Yellow
 * - 2: Gold/Silver/Crystal
 * - 3: Ruby/Sapphire/Emerald/FireRed/LeafGreen
 * - 4: Diamond/Pearl/Platinum/HeartGold/SoulSilver
 * - 5: Black/White/Black2/White2
 * - 6: X/Y/OmegaRuby/AlphaSapphire
 * - 7: Sun/Moon/UltraSun/UltraMoon/Let's Go
 * - 8: Sword/Shield/Brilliant Diamond/Shining Pearl/Legends Arceus
 * - 9: Scarlet/Violet
 */
export type Generation = number;

/**
 * Pokemon species ID (1-1025)
 * National Pokedex number
 */
export type SpeciesID = number;

/**
 * Pokemon form ID (0-255)
 * 0 is the default form for most species
 */
export type FormID = number;

/**
 * Game version ID
 * Used for version-specific data like met locations
 */
export type GameVersion = number;

/**
 * Box number (0-based index)
 */
export type BoxNumber = number;

/**
 * Slot number (0-based index)
 */
export type SlotNumber = number;

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
 * Success message response
 */
export interface MessageResponse {
  message: string;
}

// ============================================================================
// Save File Models
// ============================================================================

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

// ============================================================================
// Pokemon Models
// ============================================================================

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

// ============================================================================
// Trainer Models
// ============================================================================

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
 * Shiny type specification
 */
export enum ShinyType {
  /** PID is purely random; can be shiny or not shiny */
  Random = 0,
  /** PID is randomly created and forced to be not shiny */
  Never = 1,
  /** PID is randomly created and forced to be shiny */
  Always = 2,
  /** PID is randomly created and forced to be shiny as Stars */
  AlwaysStar = 3,
  /** PID is randomly created and forced to be shiny as Squares */
  AlwaysSquare = 4,
  /** PID is fixed to a specified value */
  FixedValue = 5,
}

/**
 * PID information
 */
export interface PIDInfo {
  success: boolean;
  pid: number;
  isShiny: boolean;
  shinyType: 'None' | 'Star' | 'Square';
  nature: number;
  natureName: string;
  gender: number;
  genderName: 'Male' | 'Female' | 'Genderless';
}

/**
 * PKM data response
 */
export interface PKMDataResponse {
  success: boolean;
  base64Data: string;
}

// ============================================================================
// Storage Models
// ============================================================================

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

// ============================================================================
// Item Models
// ============================================================================

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

// ============================================================================
// Pokedex Models
// ============================================================================

/**
 * Pokedex entry
 */
export interface PokedexEntry {
  species: number;
  speciesName: string;
  seen: boolean;
  caught: boolean;
}

// ============================================================================
// Progress Models
// ============================================================================

/**
 * Hall of Fame entry
 */
export interface HallOfFameEntry {
  index: number;
  timestamp: string;
  team: PokemonSummary[];
}

// ============================================================================
// Communication Models
// ============================================================================

/**
 * Mail message
 */
export interface MailMessage {
  index: number;
  authorName: string;
  authorTID: number;
  mailType: number;
  message: string;
  isHeld: boolean;
  heldItem: number;
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

// ============================================================================
// World Models
// ============================================================================

/**
 * Secret Base data
 */
export interface SecretBaseData {
  trainerName: string;
  trainerID: number;
  secretID: number;
  gender: number;
  language: number;
  locationName: string;
  locationID: number;
}

/**
 * Entralink data (Gen 5)
 */
export interface EntralinkData {
  forestLevel: number;
  missionsCompleted: number;
  whiteForestCount: number;
  blackCityCount: number;
}

/**
 * Poké Pelago data (Gen 7)
 */
export interface PokePelagoData {
  beansCount: number;
  isleAevelynDevelopment: number;
  isleAphunDevelopment: number;
  isleEvelupDevelopment: number;
  pokemonCount: number;
}

/**
 * Festival Plaza data (Gen 7)
 */
export interface FestivalPlazaData {
  rank: number;
  festivalCoins: number;
  totalVisitors: number;
  facilityCount: number;
}

/**
 * Poké Jobs data (Gen 8)
 */
export interface PokeJobsData {
  activeJobsCount: number;
  completedJobsCount: number;
}

/**
 * Colorful Screw location (Gen 9a - Legends Z-A)
 */
export interface ColorfulScrewLocation {
  fieldItem: string;
  x: number;
  y: number;
  z: number;
}

// ============================================================================
// Game Data Models
// ============================================================================

/**
 * Met/Egg location data for a specific generation and game
 */
export interface MetLocationData {
  generation: Generation;
  gameVersion: GameVersion;
  isEggLocations: boolean;
  locations: Array<{
    value: number;
    text: string;
  }>;
  count: number;
}

/**
 * Base stats for a Pokemon form
 */
export interface BaseStats {
  hp: number;
  attack: number;
  defense: number;
  spAttack: number;
  spDefense: number;
  speed: number;
}

/**
 * Pokemon form data
 */
export interface FormData {
  formIndex: FormID;
  formName: string;
  type1: number;
  type1Name: string;
  type2: number;
  type2Name: string;
  baseStats: BaseStats;
  genderRatio: number;
  isDualGender: boolean;
  isGenderless: boolean;
}

/**
 * Species forms data for all forms of a species
 */
export interface SpeciesFormsData {
  species: SpeciesID;
  speciesName: string;
  generation: Generation;
  forms: FormData[];
  formCount: number;
}

/**
 * Evolution chain entry
 */
export interface EvolutionEntry {
  species: SpeciesID;
  speciesName: string;
  form: FormID;
}

/**
 * Species evolution data
 */
export interface SpeciesEvolutionData {
  species: SpeciesID;
  speciesName: string;
  generation: Generation;
  evolutionChain: EvolutionEntry[];
  chainLength: number;
  forwardEvolutions: EvolutionEntry[];
  preEvolutions: EvolutionEntry[];
  baseSpecies: SpeciesID;
  baseSpeciesName: string;
  baseForm: FormID;
}

/**
 * PKM format conversion result
 */
export interface PKMConversionResult {
  success: boolean;
  base64Data: string;
  fromGeneration: Generation;
  toGeneration: Generation;
  fromFormat: string;
  toFormat: string;
  conversionResult: string;
}

/**
 * Pokemon modification data structure
 * All fields are optional - only specify the fields you want to modify
 */
export interface PokemonModifications {
  /** National Pokedex number (1-1025) */
  species?: SpeciesID;
  /** Pokemon nickname (max length varies by generation) */
  nickname?: string;
  /** Pokemon level (1-100) */
  level?: number;
  /** Nature ID (0-24) */
  nature?: number;
  /** Ability ID */
  ability?: number;
  /** Held item ID */
  heldItem?: number;
  /** Array of move IDs (max 4 moves) */
  moves?: number[];
  /** Individual Values array [HP, Atk, Def, SpA, SpD, Spe] (0-31) */
  ivs?: number[];
  /** Effort Values array [HP, Atk, Def, SpA, SpD, Spe] (0-252 each, max 510 total) */
  evs?: number[];
  /** Gender (0=Male, 1=Female, 2=Genderless) */
  gender?: number;
  /** Whether Pokemon should be shiny */
  isShiny?: boolean;
  /** Original Trainer name */
  ot_Name?: string;
  /** Pokeball ID */
  ball?: number;
}



// ============================================================================
// Knowledge Models
// ============================================================================

/**
 * Named entity (species, move, ability, item, etc.)
 */
export interface NamedEntity {
  id: number;
  name: string;
}

/**
 * Battle Facility statistics
 */
export interface BattleFacilityStats {
  bp?: number;
  superSingle?: number;
  superDouble?: number;
  superMulti?: number;
  singlePast?: number;
  doublePast?: number;
  multiNPCPast?: number;
  multiFriendsPast?: number;
  superSinglePast?: number;
  superDoublePast?: number;
  superMultiNPCPast?: number;
  superMultiFriendsPast?: number;
  singleRecord?: number;
  doubleRecord?: number;
  multiNPCRecord?: number;
  multiFriendsRecord?: number;
  superSingleRecord?: number;
  superDoubleRecord?: number;
  superMultiNPCRecord?: number;
  superMultiFriendsRecord?: number;
}

// ============================================================================
// PKHeX WASM API Interface
// ============================================================================

/**
 * PKHeX WASM API
 * 
 * All methods return objects directly - no JSON parsing needed!
 * Successful responses include { success: true, ...data }
 * Error responses include { error: string, code?: string }
 */

/**
 * Main PKHeX API interface
 * 
 * Success responses include { success: true, ...data }
 * Error responses include { error: string, code?: string }
 */
export interface PKHeXApi {
  // Static utility functions (no save/pkm needed)
  getSpeciesName(speciesId: number): ApiResult<{ name: string }>;
  getAllSpecies(): ApiResult<{ species: NamedEntity[] }>;
  getMoveName(moveId: number): ApiResult<{ name: string }>;
  getAllMoves(): ApiResult<{ moves: NamedEntity[] }>;
  getAbilityName(abilityId: number): ApiResult<{ name: string }>;
  getAllAbilities(): ApiResult<{ abilities: NamedEntity[] }>;
  getItemName(itemId: number): ApiResult<{ name: string }>;
  getAllItems(): ApiResult<{ items: NamedEntity[] }>;
  getNatureName(natureId: number): ApiResult<{ name: string }>;
  getAllNatures(): ApiResult<{ natures: NamedEntity[] }>;
  getTypeName(typeId: number): ApiResult<{ name: string }>;
  getAllTypes(): ApiResult<{ types: NamedEntity[] }>;

  // Save file operations
  save: {
    load(base64Data: string): ApiResult<SaveFileHandle>;
    getInfo(handle: SaveHandle): ApiResult<SaveFileInfo>;
    export(handle: SaveHandle): ApiResult<{ base64Data: string }>;
    dispose(handle: SaveHandle): ApiResult<MessageResponse>;
    setTextSpeed(handle: SaveHandle, speed: number): ApiResult<MessageResponse>;
    getTextSpeed(handle: SaveHandle): ApiResult<{ textSpeed: number; speedName: string }>;

    // Pokemon operations within save
    pokemon: {
      get(handle: SaveHandle, box: number, slot: number): ApiResult<PokemonDetail>;
      getAll(handle: SaveHandle): ApiResult<PokemonSummary[]>;
      getParty(handle: SaveHandle): ApiResult<PokemonSummary[]>;
      getPartySlot(handle: SaveHandle, slot: number): ApiResult<PokemonDetail>;
      modify(handle: SaveHandle, box: number, slot: number, modifications: PokemonModifications): ApiResult<MessageResponse>;
      set(handle: SaveHandle, box: number, slot: number, base64PkmData: string): ApiResult<MessageResponse>;
      delete(handle: SaveHandle, box: number, slot: number): ApiResult<MessageResponse>;
      move(handle: SaveHandle, fromBox: number, fromSlot: number, toBox: number, toSlot: number): ApiResult<MessageResponse>;
      generatePID(handle: SaveHandle, box: number, slot: number, nature: number, shiny: boolean): ApiResult<MessageResponse>;
      setPID(handle: SaveHandle, box: number, slot: number, pid: number): ApiResult<MessageResponse>;
      setShiny(handle: SaveHandle, box: number, slot: number, shinyType: ShinyType): ApiResult<MessageResponse>;
      getPIDInfo(handle: SaveHandle, box: number, slot: number): ApiResult<PIDInfo>;
      checkLegality(handle: SaveHandle, box: number, slot: number): ApiResult<LegalityResult>;
      legalize(handle: SaveHandle, box: number, slot: number): ApiResult<MessageResponse>;
      exportShowdown(handle: SaveHandle, box: number, slot: number): ApiResult<{ showdownText: string }>;
      importShowdown(handle: SaveHandle, box: number, slot: number, showdownText: string): ApiResult<MessageResponse>;
      getRibbons(handle: SaveHandle, box: number, slot: number): ApiResult<RibbonData[]>;
      getRibbonCount(handle: SaveHandle, box: number, slot: number): ApiResult<{ ribbonCount: number }>;
      setRibbon(handle: SaveHandle, box: number, slot: number, ribbonName: string, value: boolean): ApiResult<MessageResponse>;
      getContestStats(handle: SaveHandle, box: number, slot: number): ApiResult<ContestStats>;
      setContestStat(handle: SaveHandle, box: number, slot: number, statName: string, value: number): ApiResult<MessageResponse>;
    };

    // Trainer operations
    trainer: {
      getInfo(handle: SaveHandle): ApiResult<TrainerInfo>;
      setInfo(handle: SaveHandle, trainerData: TrainerInfo): ApiResult<MessageResponse>;
      getCard(handle: SaveHandle): ApiResult<TrainerCard>;
      getAppearance(handle: SaveHandle): ApiResult<TrainerAppearance>;
      setAppearance(handle: SaveHandle, appearance: TrainerAppearance): ApiResult<MessageResponse>;
      getRivalName(handle: SaveHandle): ApiResult<{ rivalName: string }>;
      setRivalName(handle: SaveHandle, rivalName: string): ApiResult<MessageResponse>;
      getBadges(handle: SaveHandle): ApiResult<BadgeData>;
      setBadge(handle: SaveHandle, badgeIndex: number, value: boolean): ApiResult<MessageResponse>;
    };

    // Box operations
    boxes: {
      getNames(handle: SaveHandle): ApiResult<string[]>;
      getWallpapers(handle: SaveHandle): ApiResult<BoxInfo[]>;
      setWallpaper(handle: SaveHandle, box: number, wallpaperId: number): ApiResult<MessageResponse>;
      getBattleBox(handle: SaveHandle): ApiResult<PokemonSummary[]>;
      setBattleBoxSlot(handle: SaveHandle, slot: number, base64PkmData: string): ApiResult<MessageResponse>;
      getDaycare(handle: SaveHandle): ApiResult<DaycareData>;
    };

    // Item operations
    items: {
      getPouches(handle: SaveHandle): ApiResult<PouchData[]>;
      add(handle: SaveHandle, itemId: number, count: number, pouchIndex: number): ApiResult<MessageResponse>;
      remove(handle: SaveHandle, itemId: number, count: number): ApiResult<MessageResponse>;
    };

    // Pokedex operations
    pokedex: {
      get(handle: SaveHandle): ApiResult<PokedexEntry[]>;
      setSeen(handle: SaveHandle, species: number, form: number): ApiResult<MessageResponse>;
      setCaught(handle: SaveHandle, species: number, form: number): ApiResult<MessageResponse>;
    };

    // Game progress operations
    progress: {
      getBattlePoints(handle: SaveHandle): ApiResult<{ battlePoints: number }>;
      setBattlePoints(handle: SaveHandle, battlePoints: number): ApiResult<MessageResponse>;
      getCoins(handle: SaveHandle): ApiResult<{ coins: number }>;
      setCoins(handle: SaveHandle, coins: number): ApiResult<MessageResponse>;
      getRecords(handle: SaveHandle): ApiResult<{ records: Record<string, number> }>;
      setRecord(handle: SaveHandle, recordIndex: number, value: number): ApiResult<MessageResponse>;
      getEventFlag(handle: SaveHandle, flagIndex: number): ApiResult<{ flagIndex: number; value: boolean }>;
      setEventFlag(handle: SaveHandle, flagIndex: number, value: boolean): ApiResult<MessageResponse>;
      getEventConst(handle: SaveHandle, constIndex: number): ApiResult<{ constIndex: number; value: number }>;
      setEventConst(handle: SaveHandle, constIndex: number, value: number): ApiResult<MessageResponse>;
      getBattleFacilityStats(handle: SaveHandle): ApiResult<BattleFacilityStats>;
      getHallOfFame(handle: SaveHandle): ApiResult<{ entries: HallOfFameEntry[] }>;
      setHallOfFameEntry(handle: SaveHandle, index: number, team: PokemonSummary[]): ApiResult<MessageResponse>;
      collectColorfulScrews(handle: SaveHandle): ApiResult<{ screwsCollected: number; message: string }>;
      getColorfulScrewLocations(handle: SaveHandle, collected: boolean): ApiResult<{ collected: boolean; count: number; locations: ColorfulScrewLocation[] }>;
      getInfiniteRoyalePoints(handle: SaveHandle): ApiResult<{ royalePoints: number; infiniteRoyalePoints: number }>;
      setInfiniteRoyalePoints(handle: SaveHandle, royalePoints: number, infinitePoints: number): ApiResult<MessageResponse>;
    };

    // Time operations
    time: {
      getSecondsPlayed(handle: SaveHandle): ApiResult<{ secondsPlayed: number }>;
      getSecondsToStart(handle: SaveHandle): ApiResult<{ secondsToStart: number }>;
      getSecondsToFame(handle: SaveHandle): ApiResult<{ secondsToFame: number }>;
      setGameTime(handle: SaveHandle, hours: number, minutes: number, seconds: number): ApiResult<MessageResponse>;
      setSecondsToStart(handle: SaveHandle, seconds: number): ApiResult<MessageResponse>;
      setSecondsToFame(handle: SaveHandle, seconds: number): ApiResult<MessageResponse>;
    };

    // Mail operations
    mail: {
      getMailbox(handle: SaveHandle): ApiResult<MailMessage[]>;
      getMessage(handle: SaveHandle, index: number): ApiResult<MailMessage>;
      setMessage(handle: SaveHandle, index: number, mail: MailMessage): ApiResult<MessageResponse>;
      delete(handle: SaveHandle, index: number): ApiResult<MessageResponse>;
    };

    // Mystery Gift operations
    mysteryGift: {
      getAll(handle: SaveHandle): ApiResult<{ gifts: MysteryGiftCard[] }>;
      getCard(handle: SaveHandle, index: number): ApiResult<{ card: MysteryGiftCard }>;
      setCard(handle: SaveHandle, index: number, card: MysteryGiftCard): ApiResult<MessageResponse>;
      getFlags(handle: SaveHandle): ApiResult<{ flags: boolean[] }>;
      delete(handle: SaveHandle, index: number): ApiResult<MessageResponse>;
    };

    // Generation-specific features
    features: {
      getSecretBase(handle: SaveHandle): ApiResult<{ secretBase: SecretBaseData }>;
      getEntralinkData(handle: SaveHandle): ApiResult<{ entralinkData: EntralinkData }>;
      getPokePelago(handle: SaveHandle): ApiResult<{ pokePelagoData: PokePelagoData }>;
      getFestivalPlaza(handle: SaveHandle): ApiResult<{ festivalPlazaData: FestivalPlazaData }>;
      getPokeJobs(handle: SaveHandle): ApiResult<{ pokeJobsData: PokeJobsData }>;
      unlockFashionCategory(handle: SaveHandle, category: string): ApiResult<MessageResponse>;
      unlockAllFashion(handle: SaveHandle): ApiResult<{ itemsUnlocked: number; message: string }>;
      unlockAllHairMakeup(handle: SaveHandle): ApiResult<{ itemsUnlocked: number; message: string }>;
    };
  };

  // Standalone PKM operations (no save needed)
  pkm: {
    /**
     * Get detailed Pokemon data from raw PKM bytes
     * @param base64PkmData - Base64 encoded PKM data
     * @param generation - Pokemon generation (1-9)
     */
    getData(base64PkmData: string, generation: Generation): ApiResult<PokemonDetail>;

    /**
     * Modify Pokemon data
     * @param base64PkmData - Base64 encoded PKM data
     * @param generation - Pokemon generation (1-9)
     * @param modifications - Pokemon modifications to apply
     */
    modify(base64PkmData: string, generation: Generation, modifications: PokemonModifications): ApiResult<PKMDataResponse>;

    /**
     * Check if Pokemon is legal
     * @param base64PkmData - Base64 encoded PKM data
     * @param generation - Pokemon generation (1-9)
     */
    checkLegality(base64PkmData: string, generation: Generation): ApiResult<LegalityResult>;

    /**
     * Legalize Pokemon moveset and stats
     * @param base64PkmData - Base64 encoded PKM data
     * @param generation - Pokemon generation (1-9)
     */
    legalize(base64PkmData: string, generation: Generation): ApiResult<PKMDataResponse>;

    /**
     * Export Pokemon to Showdown format
     * @param base64PkmData - Base64 encoded PKM data
     * @param generation - Pokemon generation (1-9)
     */
    exportShowdown(base64PkmData: string, generation: Generation): ApiResult<{ showdownText: string }>;

    /**
     * Calculate Pokemon stats from IVs/EVs/Level
     * @param base64PkmData - Base64 encoded PKM data
     * @param generation - Pokemon generation (1-9)
     */
    calculateStats(base64PkmData: string, generation: Generation): ApiResult<BaseStats>;

    /**
     * Get all ribbons on a Pokemon
     * @param base64PkmData - Base64 encoded PKM data
     * @param generation - Pokemon generation (1-9)
     */
    getRibbons(base64PkmData: string, generation: Generation): ApiResult<RibbonData[]>;

    /**
     * Set a specific ribbon on a Pokemon
     * @param base64PkmData - Base64 encoded PKM data
     * @param generation - Pokemon generation (1-9)
     * @param ribbonName - Name of the ribbon to set
     * @param value - Whether to enable or disable the ribbon
     */
    setRibbon(base64PkmData: string, generation: Generation, ribbonName: string, value: boolean): ApiResult<PKMDataResponse>;

    /**
     * Make Pokemon shiny
     * @param base64PkmData - Base64 encoded PKM data
     * @param generation - Pokemon generation (1-9)
     * @param shinyType - Type of shiny (0 = not shiny, 1 = star, 2 = square)
     */
    setShiny(base64PkmData: string, generation: Generation, shinyType: ShinyType): ApiResult<PKMDataResponse>;

    /**
     * Get PID information
     * @param base64PkmData - Base64 encoded PKM data
     * @param generation - Pokemon generation (1-9)
     */
    getPIDInfo(base64PkmData: string, generation: Generation): ApiResult<PIDInfo>;

    /**
     * Set Pokemon PID
     * @param base64PkmData - Base64 encoded PKM data
     * @param generation - Pokemon generation (1-9)
     * @param pid - New PID value
     */
    setPID(base64PkmData: string, generation: Generation, pid: number): ApiResult<PKMDataResponse>;

    /**
     * Reroll encryption constant
     * @param base64PkmData - Base64 encoded PKM data
     * @param generation - Pokemon generation (1-9)
     */
    rerollEncryptionConstant(base64PkmData: string, generation: Generation): ApiResult<PKMDataResponse>;

    /**
     * Get Hidden Power type and power
     * @param base64PkmData - Base64 encoded PKM data
     * @param generation - Pokemon generation (1-9)
     */
    getHiddenPower(base64PkmData: string, generation: Generation): ApiResult<{ type: number; typeName: string; power: number }>;

    /**
     * Get IV characteristic text
     * @param base64PkmData - Base64 encoded PKM data
     * @param generation - Pokemon generation (1-9)
     */
    getCharacteristic(base64PkmData: string, generation: Generation): ApiResult<{ index: number; text: string }>;

    /**
     * Convert Pokemon from one generation format to another
     * @param base64PkmData - Base64 encoded PKM data
     * @param fromGeneration - Source generation (1-9)
     * @param toGeneration - Target generation (1-9)
     * @returns Converted Pokemon data or error if conversion is not possible
     */
    convertFormat(
      base64PkmData: string,
      fromGeneration: Generation,
      toGeneration: Generation
    ): ApiResult<PKMConversionResult>;
  };

  // Game data utilities
  gameData: {
    /**
     * Get met/egg locations for a specific generation and game version
     * @param generation - Pokemon generation (1-9)
     * @param gameVersion - Game version ID
     * @param eggLocations - Whether to get egg locations instead of met locations
     */
    getMetLocations(
      generation: Generation,
      gameVersion: GameVersion,
      eggLocations: boolean
    ): ApiResult<MetLocationData>;

    /**
     * Get all forms for a species in a specific generation
     * @param species - National Pokedex number (1-1025)
     * @param generation - Pokemon generation (1-9)
     */
    getSpeciesForms(species: SpeciesID, generation: Generation): ApiResult<SpeciesFormsData>;

    /**
     * Get evolution chain for a species
     * @param species - National Pokedex number (1-1025)
     * @param generation - Pokemon generation (1-9)
     */
    getSpeciesEvolutions(species: SpeciesID, generation: Generation): ApiResult<SpeciesEvolutionData>;
  };

  /**
   * @deprecated Gen9a namespace is deprecated. Use the following instead:
   * - collectColorfulScrews -> save.progress.collectColorfulScrews
   * - getColorfulScrewLocations -> save.progress.getColorfulScrewLocations
   * - setTextSpeed -> save.setTextSpeed
   * - getTextSpeed -> save.getTextSpeed
   * - unlockFashionCategory -> save.features.unlockFashionCategory
   * - unlockAllFashion -> save.features.unlockAllFashion
   * - unlockAllHairMakeup -> save.features.unlockAllHairMakeup
   * - getInfiniteRoyalePoints -> save.progress.getInfiniteRoyalePoints
   * - setInfiniteRoyalePoints -> save.progress.setInfiniteRoyalePoints
   */
  gen9a: {
    /**
     * @deprecated Use save.progress.collectColorfulScrews instead
     * Collect all Colorful Screws in Legends Z-A and update inventory
     * @param handle - Save file handle
     * @returns Number of screws collected
     */
    collectColorfulScrews(handle: SaveHandle): ApiResult<{ screwsCollected: number; message: string }>;

    /**
     * @deprecated Use save.progress.getColorfulScrewLocations instead
     * Get locations of Colorful Screws by collection state
     * @param handle - Save file handle
     * @param collected - True for collected screws, false for uncollected
     * @returns Array of screw locations with coordinates
     */
    getColorfulScrewLocations(
      handle: SaveHandle,
      collected: boolean
    ): ApiResult<{ collected: boolean; count: number; locations: ColorfulScrewLocation[] }>;

    /**
     * @deprecated Use save.setTextSpeed instead
     * Set text speed in ConfigSave
     * @param handle - Save file handle
     * @param speed - Text speed (0=Slow, 1=Normal, 2=Fast, 3=Instant)
     */
    setTextSpeed(handle: SaveHandle, speed: number): ApiResult<MessageResponse>;

    /**
     * @deprecated Use save.getTextSpeed instead
     * Get current text speed setting
     * @param handle - Save file handle
     * @returns Current text speed value and name
     */
    getTextSpeed(handle: SaveHandle): ApiResult<{ textSpeed: number; speedName: string }>;

    /**
     * @deprecated Use save.features.unlockFashionCategory instead
     * Unlock all fashion items in a specific category
     * @param handle - Save file handle
     * @param category - Fashion category (tops, bottoms, allinone, headwear, eyewear, gloves, legwear, footwear, satchels, earrings)
     */
    unlockFashionCategory(handle: SaveHandle, category: string): ApiResult<MessageResponse>;

    /**
     * @deprecated Use save.features.unlockAllFashion instead
     * Unlock all fashion items in all categories
     * @param handle - Save file handle
     * @returns Number of items unlocked
     */
    unlockAllFashion(handle: SaveHandle): ApiResult<{ itemsUnlocked: number; message: string }>;

    /**
     * @deprecated Use save.features.unlockAllHairMakeup instead
     * Unlock all hair and makeup options
     * @param handle - Save file handle
     * @returns Number of items unlocked
     */
    unlockAllHairMakeup(handle: SaveHandle): ApiResult<{ itemsUnlocked: number; message: string }>;

    /**
     * @deprecated Use save.progress.getInfiniteRoyalePoints instead
     * Get Infinite Royale ticket points
     * @param handle - Save file handle
     * @returns Regular and Infinite Royale points
     */
    getInfiniteRoyalePoints(handle: SaveHandle): ApiResult<{ royalePoints: number; infiniteRoyalePoints: number }>;

    /**
     * @deprecated Use save.progress.setInfiniteRoyalePoints instead
     * Set Infinite Royale ticket points
     * @param handle - Save file handle
     * @param royalePoints - Regular Royale ticket points (max: 2147483647)
     * @param infinitePoints - Infinite Royale ticket points (max: 2147483647)
     */
    setInfiniteRoyalePoints(
      handle: SaveHandle,
      royalePoints: number,
      infinitePoints: number
    ): ApiResult<MessageResponse>;
  };
}
