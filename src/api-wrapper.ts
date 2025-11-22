

import type {
  PKHeXApi,
  ApiResult,
  SaveHandle,
  SaveFileHandle,
  SaveFileInfo,
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
  PouchData,
  PokedexEntry,
  HallOfFameEntry,
  MysteryGiftCard,
  MailMessage,
  SecretBaseData,
  EntralinkData,
  PokePelagoData,
  FestivalPlazaData,
  PokeJobsData,
  NamedEntity,
  BattleFacilityStats,
  MessageResponse,
} from './index.js';

interface RawPKHeXApi {
  LoadSave(base64Data: string): string;
  GetSaveInfo(handle: number): string;
  ExportSave(handle: number): string;
  DisposeSave(handle: number): string;
  GetAllPokemon(handle: number): string;
  GetParty(handle: number): string;
  GetPokemon(handle: number, box: number, slot: number): string;
  GetPartySlot(handle: number, slot: number): string;
  ModifyPokemon(handle: number, box: number, slot: number, modificationsJson: string): string;
  SetPokemon(handle: number, box: number, slot: number, base64PkmData: string): string;
  DeletePokemon(handle: number, box: number, slot: number): string;
  MovePokemon(handle: number, fromBox: number, fromSlot: number, toBox: number, toSlot: number): string;
  GeneratePID(handle: number, box: number, slot: number, nature: number, shiny: boolean): string;
  SetPID(handle: number, box: number, slot: number, pid: number): string;
  SetShiny(handle: number, box: number, slot: number, shinyType: number): string;
  GetPIDInfo(handle: number, box: number, slot: number): string;
  CheckLegality(handle: number, box: number, slot: number): string;
  LegalizePokemon(handle: number, box: number, slot: number): string;
  ExportShowdown(handle: number, box: number, slot: number): string;
  ImportShowdown(handle: number, box: number, slot: number, showdownText: string): string;
  GetRibbons(handle: number, box: number, slot: number): string;
  GetRibbonCount(handle: number, box: number, slot: number): string;
  SetRibbon(handle: number, box: number, slot: number, ribbonName: string, value: boolean): string;
  GetContestStats(handle: number, box: number, slot: number): string;
  SetContestStat(handle: number, box: number, slot: number, statName: string, value: number): string;
  GetTrainerInfo(handle: number): string;
  SetTrainerInfo(handle: number, trainerDataJson: string): string;
  GetTrainerCard(handle: number): string;
  GetTrainerAppearance(handle: number): string;
  SetTrainerAppearance(handle: number, appearanceJson: string): string;
  GetRivalName(handle: number): string;
  SetRivalName(handle: number, rivalName: string): string;
  GetBoxNames(handle: number): string;
  GetBoxWallpapers(handle: number): string;
  SetBoxWallpaper(handle: number, box: number, wallpaperId: number): string;
  GetBattleBox(handle: number): string;
  SetBattleBoxSlot(handle: number, slot: number, base64PkmData: string): string;
  GetDaycare(handle: number): string;
  GetPouchItems(handle: number): string;
  AddItemToPouch(handle: number, itemId: number, count: number, pouchIndex: number): string;
  RemoveItemFromPouch(handle: number, itemId: number, count: number): string;
  GetPokedex(handle: number): string;
  SetPokedexSeen(handle: number, species: number, form: number): string;
  SetPokedexCaught(handle: number, species: number, form: number): string;
  GetBadges(handle: number): string;
  SetBadge(handle: number, badgeIndex: number, value: boolean): string;
  GetBattlePoints(handle: number): string;
  SetBattlePoints(handle: number, battlePoints: number): string;
  GetCoins(handle: number): string;
  SetCoins(handle: number, coins: number): string;
  GetRecords(handle: number): string;
  SetRecord(handle: number, recordIndex: number, value: number): string;
  GetEventFlag(handle: number, flagIndex: number): string;
  SetEventFlag(handle: number, flagIndex: number, value: boolean): string;
  GetEventConst(handle: number, constIndex: number): string;
  SetEventConst(handle: number, constIndex: number, value: number): string;
  GetBattleFacilityStats(handle: number): string;
  GetHallOfFame(handle: number): string;
  SetHallOfFameEntry(handle: number, index: number, teamJson: string): string;
  GetSecondsPlayed(handle: number): string;
  GetSecondsToStart(handle: number): string;
  GetSecondsToFame(handle: number): string;
  SetGameTime(handle: number, hours: number, minutes: number, seconds: number): string;
  SetSecondsToStart(handle: number, seconds: number): string;
  SetSecondsToFame(handle: number, seconds: number): string;
  GetMailbox(handle: number): string;
  GetMailMessage(handle: number, index: number): string;
  SetMailMessage(handle: number, index: number, mailJson: string): string;
  DeleteMail(handle: number, index: number): string;
  GetMysteryGifts(handle: number): string;
  GetMysteryGiftCard(handle: number, index: number): string;
  SetMysteryGiftCard(handle: number, index: number, cardJson: string): string;
  GetMysteryGiftFlags(handle: number): string;
  DeleteMysteryGift(handle: number, index: number): string;
  GetSecretBase(handle: number): string;
  GetEntralinkData(handle: number): string;
  GetPokePelago(handle: number): string;
  GetFestivalPlaza(handle: number): string;
  GetPokeJobs(handle: number): string;
  GetSpeciesName(speciesId: number): string;
  GetAllSpecies(): string;
  GetMoveName(moveId: number): string;
  GetAllMoves(): string;
  GetAbilityName(abilityId: number): string;
  GetAllAbilities(): string;
  GetItemName(itemId: number): string;
  GetAllItems(): string;
  GetNatureName(natureId: number): string;
  GetAllNatures(): string;
  GetTypeName(typeId: number): string;
  GetAllTypes(): string;
  
  GetPKMData(base64PkmData: string, generation: number): string;
  ModifyPKMData(base64PkmData: string, generation: number, modificationsJson: string): string;
  CheckPKMLegality(base64PkmData: string, generation: number): string;
  LegalizePKMData(base64PkmData: string, generation: number): string;
  ExportPKMShowdown(base64PkmData: string, generation: number): string;
  CalculatePKMStats(base64PkmData: string, generation: number): string;
  GetPKMRibbons(base64PkmData: string, generation: number): string;
  SetPKMRibbon(base64PkmData: string, generation: number, ribbonName: string, value: boolean): string;
  SetPKMShiny(base64PkmData: string, generation: number, shinyType: number): string;
  GetPKMPIDInfo(base64PkmData: string, generation: number): string;
  SetPKMPID(base64PkmData: string, generation: number, pid: number): string;
  RerollPKMEncryptionConstant(base64PkmData: string, generation: number): string;
  GetPKMHiddenPower(base64PkmData: string, generation: number): string;
  GetPKMCharacteristic(base64PkmData: string, generation: number): string;
  GetPKMMetLocations(generation: number, gameVersion: number, eggLocations: boolean): string;
  GetSpeciesForms(species: number, generation: number): string;
  GetSpeciesEvolutions(species: number, generation: number): string;
  ConvertPKMFormat(base64PkmData: string, fromGeneration: number, toGeneration: number): string;
  
  // Gen9a (Legends Z-A) methods
  CollectColorfulScrews(handle: number): string;
  GetColorfulScrewLocations(handle: number, collected: boolean): string;
  SetTextSpeed(handle: number, speed: number): string;
  GetTextSpeed(handle: number): string;
  UnlockFashionCategory(handle: number, category: string): string;
  UnlockAllFashion(handle: number): string;
  UnlockAllHairMakeup(handle: number): string;
  GetInfiniteRoyalePoints(handle: number): string;
  SetInfiniteRoyalePoints(handle: number, royalePoints: number, infinitePoints: number): string;
}

function parseJson<T>(jsonString: string): ApiResult<T> {
  try {
    const parsed = JSON.parse(jsonString);
    return parsed as ApiResult<T>;
  } catch {
    return { error: 'Failed to parse API response', code: 'PARSE_ERROR' };
  }
}

export function createPKHeXApiWrapper(rawApiOrExports: RawPKHeXApi | any): PKHeXApi {
  const rawApi: RawPKHeXApi = 
    'LoadSave' in rawApiOrExports 
      ? rawApiOrExports 
      : rawApiOrExports?.PKHeX?.Api?.PKHeXApi;
  
  if (!rawApi) {
    throw new Error('Invalid API object provided to createPKHeXApiWrapper');
  }

  return {
    getSpeciesName: (speciesId: number) => parseJson<{ name: string }>(rawApi.GetSpeciesName(speciesId)),
    getAllSpecies: () => parseJson<{ species: NamedEntity[] }>(rawApi.GetAllSpecies()),
    getMoveName: (moveId: number) => parseJson<{ name: string }>(rawApi.GetMoveName(moveId)),
    getAllMoves: () => parseJson<{ moves: NamedEntity[] }>(rawApi.GetAllMoves()),
    getAbilityName: (abilityId: number) => parseJson<{ name: string }>(rawApi.GetAbilityName(abilityId)),
    getAllAbilities: () => parseJson<{ abilities: NamedEntity[] }>(rawApi.GetAllAbilities()),
    getItemName: (itemId: number) => parseJson<{ name: string }>(rawApi.GetItemName(itemId)),
    getAllItems: () => parseJson<{ items: NamedEntity[] }>(rawApi.GetAllItems()),
    getNatureName: (natureId: number) => parseJson<{ name: string }>(rawApi.GetNatureName(natureId)),
    getAllNatures: () => parseJson<{ natures: NamedEntity[] }>(rawApi.GetAllNatures()),
    getTypeName: (typeId: number) => parseJson<{ name: string }>(rawApi.GetTypeName(typeId)),
    getAllTypes: () => parseJson<{ types: NamedEntity[] }>(rawApi.GetAllTypes()),

    save: {
      load: (base64Data: string) => parseJson<SaveFileHandle>(rawApi.LoadSave(base64Data)),
      getInfo: (handle: SaveHandle) => parseJson<SaveFileInfo>(rawApi.GetSaveInfo(handle)),
      export: (handle: SaveHandle) => parseJson<{ base64Data: string }>(rawApi.ExportSave(handle)),
      dispose: (handle: SaveHandle) => parseJson<MessageResponse>(rawApi.DisposeSave(handle)),
      setTextSpeed: (handle: SaveHandle, speed: number) => parseJson<MessageResponse>(rawApi.SetTextSpeed(handle, speed)),
      getTextSpeed: (handle: SaveHandle) => parseJson<{ textSpeed: number; speedName: string }>(rawApi.GetTextSpeed(handle)),

      pokemon: {
        get: (handle: SaveHandle, box: number, slot: number) => parseJson<PokemonDetail>(rawApi.GetPokemon(handle, box, slot)),
        getAll: (handle: SaveHandle) => parseJson<PokemonSummary[]>(rawApi.GetAllPokemon(handle)),
        getParty: (handle: SaveHandle) => parseJson<PokemonSummary[]>(rawApi.GetParty(handle)),
        getPartySlot: (handle: SaveHandle, slot: number) => parseJson<PokemonDetail>(rawApi.GetPartySlot(handle, slot)),
        modify: (handle: SaveHandle, box: number, slot: number, modifications: PokemonModifications) => parseJson<MessageResponse>(rawApi.ModifyPokemon(handle, box, slot, JSON.stringify(modifications))),
        set: (handle: SaveHandle, box: number, slot: number, base64PkmData: string) => parseJson<MessageResponse>(rawApi.SetPokemon(handle, box, slot, base64PkmData)),
        delete: (handle: SaveHandle, box: number, slot: number) => parseJson<MessageResponse>(rawApi.DeletePokemon(handle, box, slot)),
        move: (handle: SaveHandle, fromBox: number, fromSlot: number, toBox: number, toSlot: number) => parseJson<MessageResponse>(rawApi.MovePokemon(handle, fromBox, fromSlot, toBox, toSlot)),
        generatePID: (handle: SaveHandle, box: number, slot: number, nature: number, shiny: boolean) => parseJson<MessageResponse>(rawApi.GeneratePID(handle, box, slot, nature, shiny)),
        setPID: (handle: SaveHandle, box: number, slot: number, pid: number) => parseJson<MessageResponse>(rawApi.SetPID(handle, box, slot, pid)),
        setShiny: (handle: SaveHandle, box: number, slot: number, shinyType: ShinyType) => parseJson<MessageResponse>(rawApi.SetShiny(handle, box, slot, shinyType)),
        getPIDInfo: (handle: SaveHandle, box: number, slot: number) => parseJson<PIDInfo>(rawApi.GetPIDInfo(handle, box, slot)),
        checkLegality: (handle: SaveHandle, box: number, slot: number) => parseJson<LegalityResult>(rawApi.CheckLegality(handle, box, slot)),
        legalize: (handle: SaveHandle, box: number, slot: number) => parseJson<MessageResponse>(rawApi.LegalizePokemon(handle, box, slot)),
        exportShowdown: (handle: SaveHandle, box: number, slot: number) => parseJson<{ showdownText: string }>(rawApi.ExportShowdown(handle, box, slot)),
        importShowdown: (handle: SaveHandle, box: number, slot: number, showdownText: string) => parseJson<MessageResponse>(rawApi.ImportShowdown(handle, box, slot, showdownText)),
        getRibbons: (handle: SaveHandle, box: number, slot: number) => parseJson<RibbonData[]>(rawApi.GetRibbons(handle, box, slot)),
        getRibbonCount: (handle: SaveHandle, box: number, slot: number) => parseJson<{ ribbonCount: number }>(rawApi.GetRibbonCount(handle, box, slot)),
        setRibbon: (handle: SaveHandle, box: number, slot: number, ribbonName: string, value: boolean) => parseJson<MessageResponse>(rawApi.SetRibbon(handle, box, slot, ribbonName, value)),
        getContestStats: (handle: SaveHandle, box: number, slot: number) => parseJson<ContestStats>(rawApi.GetContestStats(handle, box, slot)),
        setContestStat: (handle: SaveHandle, box: number, slot: number, statName: string, value: number) => parseJson<MessageResponse>(rawApi.SetContestStat(handle, box, slot, statName, value)),
      },

      trainer: {
        getInfo: (handle: SaveHandle) => parseJson<TrainerInfo>(rawApi.GetTrainerInfo(handle)),
        setInfo: (handle: SaveHandle, trainerData: TrainerInfo) => parseJson<MessageResponse>(rawApi.SetTrainerInfo(handle, JSON.stringify(trainerData))),
        getCard: (handle: SaveHandle) => parseJson<TrainerCard>(rawApi.GetTrainerCard(handle)),
        getAppearance: (handle: SaveHandle) => parseJson<TrainerAppearance>(rawApi.GetTrainerAppearance(handle)),
        setAppearance: (handle: SaveHandle, appearance: TrainerAppearance) => parseJson<MessageResponse>(rawApi.SetTrainerAppearance(handle, JSON.stringify(appearance))),
        getRivalName: (handle: SaveHandle) => parseJson<{ rivalName: string }>(rawApi.GetRivalName(handle)),
        setRivalName: (handle: SaveHandle, rivalName: string) => parseJson<MessageResponse>(rawApi.SetRivalName(handle, rivalName)),
        getBadges: (handle: SaveHandle) => parseJson<BadgeData>(rawApi.GetBadges(handle)),
        setBadge: (handle: SaveHandle, badgeIndex: number, value: boolean) => parseJson<MessageResponse>(rawApi.SetBadge(handle, badgeIndex, value)),
      },

      boxes: {
        getNames: (handle: SaveHandle) => parseJson<string[]>(rawApi.GetBoxNames(handle)),
        getWallpapers: (handle: SaveHandle) => parseJson<BoxInfo[]>(rawApi.GetBoxWallpapers(handle)),
        setWallpaper: (handle: SaveHandle, box: number, wallpaperId: number) => parseJson<MessageResponse>(rawApi.SetBoxWallpaper(handle, box, wallpaperId)),
        getBattleBox: (handle: SaveHandle) => parseJson<PokemonSummary[]>(rawApi.GetBattleBox(handle)),
        setBattleBoxSlot: (handle: SaveHandle, slot: number, base64PkmData: string) => parseJson<MessageResponse>(rawApi.SetBattleBoxSlot(handle, slot, base64PkmData)),
        getDaycare: (handle: SaveHandle) => parseJson<DaycareData>(rawApi.GetDaycare(handle)),
      },

      items: {
        getPouches: (handle: SaveHandle) => parseJson<PouchData[]>(rawApi.GetPouchItems(handle)),
        add: (handle: SaveHandle, itemId: number, count: number, pouchIndex: number) => parseJson<MessageResponse>(rawApi.AddItemToPouch(handle, itemId, count, pouchIndex)),
        remove: (handle: SaveHandle, itemId: number, count: number) => parseJson<MessageResponse>(rawApi.RemoveItemFromPouch(handle, itemId, count)),
      },

      pokedex: {
        get: (handle: SaveHandle) => parseJson<PokedexEntry[]>(rawApi.GetPokedex(handle)),
        setSeen: (handle: SaveHandle, species: number, form: number) => parseJson<MessageResponse>(rawApi.SetPokedexSeen(handle, species, form)),
        setCaught: (handle: SaveHandle, species: number, form: number) => parseJson<MessageResponse>(rawApi.SetPokedexCaught(handle, species, form)),
      },

      progress: {
        getBattlePoints: (handle: SaveHandle) => parseJson<{ battlePoints: number }>(rawApi.GetBattlePoints(handle)),
        setBattlePoints: (handle: SaveHandle, battlePoints: number) => parseJson<MessageResponse>(rawApi.SetBattlePoints(handle, battlePoints)),
        getCoins: (handle: SaveHandle) => parseJson<{ coins: number }>(rawApi.GetCoins(handle)),
        setCoins: (handle: SaveHandle, coins: number) => parseJson<MessageResponse>(rawApi.SetCoins(handle, coins)),
        getRecords: (handle: SaveHandle) => parseJson<{ records: Record<string, number> }>(rawApi.GetRecords(handle)),
        setRecord: (handle: SaveHandle, recordIndex: number, value: number) => parseJson<MessageResponse>(rawApi.SetRecord(handle, recordIndex, value)),
        getEventFlag: (handle: SaveHandle, flagIndex: number) => parseJson<{ flagIndex: number; value: boolean }>(rawApi.GetEventFlag(handle, flagIndex)),
        setEventFlag: (handle: SaveHandle, flagIndex: number, value: boolean) => parseJson<MessageResponse>(rawApi.SetEventFlag(handle, flagIndex, value)),
        getEventConst: (handle: SaveHandle, constIndex: number) => parseJson<{ constIndex: number; value: number }>(rawApi.GetEventConst(handle, constIndex)),
        setEventConst: (handle: SaveHandle, constIndex: number, value: number) => parseJson<MessageResponse>(rawApi.SetEventConst(handle, constIndex, value)),
        getBattleFacilityStats: (handle: SaveHandle) => parseJson<BattleFacilityStats>(rawApi.GetBattleFacilityStats(handle)),
        getHallOfFame: (handle: SaveHandle) => parseJson<{ entries: HallOfFameEntry[] }>(rawApi.GetHallOfFame(handle)),
        setHallOfFameEntry: (handle: SaveHandle, index: number, team: PokemonSummary[]) => parseJson<MessageResponse>(rawApi.SetHallOfFameEntry(handle, index, JSON.stringify(team))),
        collectColorfulScrews: (handle: SaveHandle) => parseJson<{ screwsCollected: number; message: string }>(rawApi.CollectColorfulScrews(handle)),
        getColorfulScrewLocations: (handle: SaveHandle, collected: boolean) => parseJson<{ collected: boolean; count: number; locations: Array<{ fieldItem: string; x: number; y: number; z: number }> }>(rawApi.GetColorfulScrewLocations(handle, collected)),
        getInfiniteRoyalePoints: (handle: SaveHandle) => parseJson<{ royalePoints: number; infiniteRoyalePoints: number }>(rawApi.GetInfiniteRoyalePoints(handle)),
        setInfiniteRoyalePoints: (handle: SaveHandle, royalePoints: number, infinitePoints: number) => parseJson<MessageResponse>(rawApi.SetInfiniteRoyalePoints(handle, royalePoints, infinitePoints)),
      },

      time: {
        getSecondsPlayed: (handle: SaveHandle) => parseJson<{ secondsPlayed: number }>(rawApi.GetSecondsPlayed(handle)),
        getSecondsToStart: (handle: SaveHandle) => parseJson<{ secondsToStart: number }>(rawApi.GetSecondsToStart(handle)),
        getSecondsToFame: (handle: SaveHandle) => parseJson<{ secondsToFame: number }>(rawApi.GetSecondsToFame(handle)),
        setGameTime: (handle: SaveHandle, hours: number, minutes: number, seconds: number) => parseJson<MessageResponse>(rawApi.SetGameTime(handle, hours, minutes, seconds)),
        setSecondsToStart: (handle: SaveHandle, seconds: number) => parseJson<MessageResponse>(rawApi.SetSecondsToStart(handle, seconds)),
        setSecondsToFame: (handle: SaveHandle, seconds: number) => parseJson<MessageResponse>(rawApi.SetSecondsToFame(handle, seconds)),
      },

      mail: {
        getMailbox: (handle: SaveHandle) => parseJson<MailMessage[]>(rawApi.GetMailbox(handle)),
        getMessage: (handle: SaveHandle, index: number) => parseJson<MailMessage>(rawApi.GetMailMessage(handle, index)),
        setMessage: (handle: SaveHandle, index: number, mail: MailMessage) => parseJson<MessageResponse>(rawApi.SetMailMessage(handle, index, JSON.stringify(mail))),
        delete: (handle: SaveHandle, index: number) => parseJson<MessageResponse>(rawApi.DeleteMail(handle, index)),
      },

      mysteryGift: {
        getAll: (handle: SaveHandle) => parseJson<{ gifts: MysteryGiftCard[] }>(rawApi.GetMysteryGifts(handle)),
        getCard: (handle: SaveHandle, index: number) => parseJson<{ card: MysteryGiftCard }>(rawApi.GetMysteryGiftCard(handle, index)),
        setCard: (handle: SaveHandle, index: number, card: MysteryGiftCard) => parseJson<MessageResponse>(rawApi.SetMysteryGiftCard(handle, index, JSON.stringify(card))),
        getFlags: (handle: SaveHandle) => parseJson<{ flags: boolean[] }>(rawApi.GetMysteryGiftFlags(handle)),
        delete: (handle: SaveHandle, index: number) => parseJson<MessageResponse>(rawApi.DeleteMysteryGift(handle, index)),
      },

      features: {
        getSecretBase: (handle: SaveHandle) => parseJson<{ secretBase: SecretBaseData }>(rawApi.GetSecretBase(handle)),
        getEntralinkData: (handle: SaveHandle) => parseJson<{ entralinkData: EntralinkData }>(rawApi.GetEntralinkData(handle)),
        getPokePelago: (handle: SaveHandle) => parseJson<{ pokePelagoData: PokePelagoData }>(rawApi.GetPokePelago(handle)),
        getFestivalPlaza: (handle: SaveHandle) => parseJson<{ festivalPlazaData: FestivalPlazaData }>(rawApi.GetFestivalPlaza(handle)),
        getPokeJobs: (handle: SaveHandle) => parseJson<{ pokeJobsData: PokeJobsData }>(rawApi.GetPokeJobs(handle)),
        unlockFashionCategory: (handle: SaveHandle, category: string) => parseJson<MessageResponse>(rawApi.UnlockFashionCategory(handle, category)),
        unlockAllFashion: (handle: SaveHandle) => parseJson<{ itemsUnlocked: number; message: string }>(rawApi.UnlockAllFashion(handle)),
        unlockAllHairMakeup: (handle: SaveHandle) => parseJson<{ itemsUnlocked: number; message: string }>(rawApi.UnlockAllHairMakeup(handle)),
      },
    },

    pkm: {
      getData: (base64PkmData: string, generation: number) => parseJson<PokemonDetail>(rawApi.GetPKMData(base64PkmData, generation)),
      modify: (base64PkmData: string, generation: number, modifications: PokemonModifications) => parseJson<PKMDataResponse>(rawApi.ModifyPKMData(base64PkmData, generation, JSON.stringify(modifications))),
      checkLegality: (base64PkmData: string, generation: number) => parseJson<LegalityResult>(rawApi.CheckPKMLegality(base64PkmData, generation)),
      legalize: (base64PkmData: string, generation: number) => parseJson<PKMDataResponse>(rawApi.LegalizePKMData(base64PkmData, generation)),
      exportShowdown: (base64PkmData: string, generation: number) => parseJson<{ showdownText: string }>(rawApi.ExportPKMShowdown(base64PkmData, generation)),
      calculateStats: (base64PkmData: string, generation: number) => parseJson<{ hp: number; attack: number; defense: number; spAttack: number; spDefense: number; speed: number }>(rawApi.CalculatePKMStats(base64PkmData, generation)),
      getRibbons: (base64PkmData: string, generation: number) => parseJson<RibbonData[]>(rawApi.GetPKMRibbons(base64PkmData, generation)),
      setRibbon: (base64PkmData: string, generation: number, ribbonName: string, value: boolean) => parseJson<PKMDataResponse>(rawApi.SetPKMRibbon(base64PkmData, generation, ribbonName, value)),
      setShiny: (base64PkmData: string, generation: number, shinyType: ShinyType) => parseJson<PKMDataResponse>(rawApi.SetPKMShiny(base64PkmData, generation, shinyType)),
      getPIDInfo: (base64PkmData: string, generation: number) => parseJson<PIDInfo>(rawApi.GetPKMPIDInfo(base64PkmData, generation)),
      setPID: (base64PkmData: string, generation: number, pid: number) => parseJson<PKMDataResponse>(rawApi.SetPKMPID(base64PkmData, generation, pid)),
      rerollEncryptionConstant: (base64PkmData: string, generation: number) => parseJson<PKMDataResponse>(rawApi.RerollPKMEncryptionConstant(base64PkmData, generation)),
      getHiddenPower: (base64PkmData: string, generation: number) => parseJson<{ type: number; typeName: string; power: number }>(rawApi.GetPKMHiddenPower(base64PkmData, generation)),
      getCharacteristic: (base64PkmData: string, generation: number) => parseJson<{ index: number; text: string }>(rawApi.GetPKMCharacteristic(base64PkmData, generation)),
      convertFormat: (base64PkmData: string, fromGeneration: number, toGeneration: number) => parseJson<{ success: boolean; base64Data: string; fromGeneration: number; toGeneration: number; fromFormat: string; toFormat: string; conversionResult: string }>(rawApi.ConvertPKMFormat(base64PkmData, fromGeneration, toGeneration)),
    },

    gameData: {
      getMetLocations: (generation: number, gameVersion: number, eggLocations: boolean) => parseJson<{ generation: number; gameVersion: number; isEggLocations: boolean; locations: Array<{ value: number; text: string }>; count: number }>(rawApi.GetPKMMetLocations(generation, gameVersion, eggLocations)),
      getSpeciesForms: (species: number, generation: number) => parseJson<{ species: number; speciesName: string; generation: number; forms: Array<{ formIndex: number; formName: string; type1: number; type1Name: string; type2: number; type2Name: string; baseStats: { hp: number; attack: number; defense: number; spAttack: number; spDefense: number; speed: number }; genderRatio: number; isDualGender: boolean; isGenderless: boolean }>; formCount: number }>(rawApi.GetSpeciesForms(species, generation)),
      getSpeciesEvolutions: (species: number, generation: number) => parseJson<{ species: number; speciesName: string; generation: number; evolutionChain: Array<{ species: number; speciesName: string; form: number }>; chainLength: number; forwardEvolutions: Array<{ species: number; speciesName: string; form: number }>; preEvolutions: Array<{ species: number; speciesName: string; form: number }>; baseSpecies: number; baseSpeciesName: string; baseForm: number }>(rawApi.GetSpeciesEvolutions(species, generation)),
    },

    gen9a: {
      collectColorfulScrews: (handle: SaveHandle) => {
        console.warn('gen9a.collectColorfulScrews is deprecated. Use save.progress.collectColorfulScrews instead.');
        return parseJson<{ screwsCollected: number; message: string }>(rawApi.CollectColorfulScrews(handle));
      },
      getColorfulScrewLocations: (handle: SaveHandle, collected: boolean) => {
        console.warn('gen9a.getColorfulScrewLocations is deprecated. Use save.progress.getColorfulScrewLocations instead.');
        return parseJson<{ collected: boolean; count: number; locations: Array<{ fieldItem: string; x: number; y: number; z: number }> }>(rawApi.GetColorfulScrewLocations(handle, collected));
      },
      setTextSpeed: (handle: SaveHandle, speed: number) => {
        console.warn('gen9a.setTextSpeed is deprecated. Use save.setTextSpeed instead.');
        return parseJson<MessageResponse>(rawApi.SetTextSpeed(handle, speed));
      },
      getTextSpeed: (handle: SaveHandle) => {
        console.warn('gen9a.getTextSpeed is deprecated. Use save.getTextSpeed instead.');
        return parseJson<{ textSpeed: number; speedName: string }>(rawApi.GetTextSpeed(handle));
      },
      unlockFashionCategory: (handle: SaveHandle, category: string) => {
        console.warn('gen9a.unlockFashionCategory is deprecated. Use save.features.unlockFashionCategory instead.');
        return parseJson<MessageResponse>(rawApi.UnlockFashionCategory(handle, category));
      },
      unlockAllFashion: (handle: SaveHandle) => {
        console.warn('gen9a.unlockAllFashion is deprecated. Use save.features.unlockAllFashion instead.');
        return parseJson<{ itemsUnlocked: number; message: string }>(rawApi.UnlockAllFashion(handle));
      },
      unlockAllHairMakeup: (handle: SaveHandle) => {
        console.warn('gen9a.unlockAllHairMakeup is deprecated. Use save.features.unlockAllHairMakeup instead.');
        return parseJson<{ itemsUnlocked: number; message: string }>(rawApi.UnlockAllHairMakeup(handle));
      },
      getInfiniteRoyalePoints: (handle: SaveHandle) => {
        console.warn('gen9a.getInfiniteRoyalePoints is deprecated. Use save.progress.getInfiniteRoyalePoints instead.');
        return parseJson<{ royalePoints: number; infiniteRoyalePoints: number }>(rawApi.GetInfiniteRoyalePoints(handle));
      },
      setInfiniteRoyalePoints: (handle: SaveHandle, royalePoints: number, infinitePoints: number) => {
        console.warn('gen9a.setInfiniteRoyalePoints is deprecated. Use save.progress.setInfiniteRoyalePoints instead.');
        return parseJson<MessageResponse>(rawApi.SetInfiniteRoyalePoints(handle, royalePoints, infinitePoints));
      },
    },
  };
}
