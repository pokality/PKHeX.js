import { describe, it, expect, beforeEach, vi } from 'vitest';
import { createPKHeXApiWrapper } from '../src/api-wrapper';

describe('PKM Format Conversion API', () => {
  let mockRawApi: any;
  let api: ReturnType<typeof createPKHeXApiWrapper>;

  beforeEach(() => {
    // Create a minimal mock with all required methods
    mockRawApi = {
      LoadSave: vi.fn(),
      GetSaveInfo: vi.fn(),
      ExportSave: vi.fn(),
      DisposeSave: vi.fn(),
      GetAllPokemon: vi.fn(),
      GetParty: vi.fn(),
      GetPokemon: vi.fn(),
      GetPartySlot: vi.fn(),
      ModifyPokemon: vi.fn(),
      SetPokemon: vi.fn(),
      DeletePokemon: vi.fn(),
      MovePokemon: vi.fn(),
      GeneratePID: vi.fn(),
      SetPID: vi.fn(),
      SetShiny: vi.fn(),
      GetPIDInfo: vi.fn(),
      CheckLegality: vi.fn(),
      LegalizePokemon: vi.fn(),
      ExportShowdown: vi.fn(),
      ImportShowdown: vi.fn(),
      GetRibbons: vi.fn(),
      GetRibbonCount: vi.fn(),
      SetRibbon: vi.fn(),
      GetContestStats: vi.fn(),
      SetContestStat: vi.fn(),
      GetTrainerInfo: vi.fn(),
      SetTrainerInfo: vi.fn(),
      GetTrainerCard: vi.fn(),
      GetTrainerAppearance: vi.fn(),
      SetTrainerAppearance: vi.fn(),
      GetRivalName: vi.fn(),
      SetRivalName: vi.fn(),
      GetBadges: vi.fn(),
      SetBadge: vi.fn(),
      GetBoxNames: vi.fn(),
      GetBoxWallpapers: vi.fn(),
      SetBoxWallpaper: vi.fn(),
      GetBattleBox: vi.fn(),
      SetBattleBoxSlot: vi.fn(),
      GetDaycare: vi.fn(),
      GetPouchItems: vi.fn(),
      AddItemToPouch: vi.fn(),
      RemoveItemFromPouch: vi.fn(),
      GetPokedex: vi.fn(),
      SetPokedexSeen: vi.fn(),
      SetPokedexCaught: vi.fn(),
      GetBattlePoints: vi.fn(),
      SetBattlePoints: vi.fn(),
      GetCoins: vi.fn(),
      SetCoins: vi.fn(),
      GetRecords: vi.fn(),
      SetRecord: vi.fn(),
      GetEventFlag: vi.fn(),
      SetEventFlag: vi.fn(),
      GetEventConst: vi.fn(),
      SetEventConst: vi.fn(),
      GetBattleFacilityStats: vi.fn(),
      GetHallOfFame: vi.fn(),
      SetHallOfFameEntry: vi.fn(),
      GetSecondsPlayed: vi.fn(),
      GetSecondsToStart: vi.fn(),
      GetSecondsToFame: vi.fn(),
      SetGameTime: vi.fn(),
      SetSecondsToStart: vi.fn(),
      SetSecondsToFame: vi.fn(),
      GetMailbox: vi.fn(),
      GetMailMessage: vi.fn(),
      SetMailMessage: vi.fn(),
      DeleteMail: vi.fn(),
      GetMysteryGifts: vi.fn(),
      GetMysteryGiftCard: vi.fn(),
      SetMysteryGiftCard: vi.fn(),
      GetMysteryGiftFlags: vi.fn(),
      DeleteMysteryGift: vi.fn(),
      GetSecretBase: vi.fn(),
      GetEntralinkData: vi.fn(),
      GetPokePelago: vi.fn(),
      GetFestivalPlaza: vi.fn(),
      GetPokeJobs: vi.fn(),
      GetSpeciesName: vi.fn(),
      GetAllSpecies: vi.fn(),
      GetMoveName: vi.fn(),
      GetAllMoves: vi.fn(),
      GetAbilityName: vi.fn(),
      GetAllAbilities: vi.fn(),
      GetItemName: vi.fn(),
      GetAllItems: vi.fn(),
      GetNatureName: vi.fn(),
      GetAllNatures: vi.fn(),
      GetTypeName: vi.fn(),
      GetAllTypes: vi.fn(),
      GetPKMData: vi.fn(),
      ModifyPKMData: vi.fn(),
      CheckPKMLegality: vi.fn(),
      LegalizePKMData: vi.fn(),
      ExportPKMShowdown: vi.fn(),
      CalculatePKMStats: vi.fn(),
      GetPKMRibbons: vi.fn(),
      SetPKMRibbon: vi.fn(),
      SetPKMShiny: vi.fn(),
      GetPKMPIDInfo: vi.fn(),
      SetPKMPID: vi.fn(),
      RerollPKMEncryptionConstant: vi.fn(),
      GetPKMHiddenPower: vi.fn(),
      GetPKMCharacteristic: vi.fn(),
      GetPKMMetLocations: vi.fn(),
      GetSpeciesForms: vi.fn(),
      GetSpeciesEvolutions: vi.fn(),
      ConvertPKMFormat: vi.fn(),
    };
    api = createPKHeXApiWrapper(mockRawApi);
  });

  it('should convert Pokemon from Gen 3 to Gen 4', () => {
    const mockResponse = JSON.stringify({
      success: true,
      base64Data: 'converteddata==',
      fromGeneration: 3,
      toGeneration: 4,
      fromFormat: 'PK3',
      toFormat: 'PK4',
      conversionResult: 'Success',
    });
    mockRawApi.ConvertPKMFormat.mockReturnValue(mockResponse);

    const result = api.pkm.convertFormat('originaldata==', 3, 4);

    expect(mockRawApi.ConvertPKMFormat).toHaveBeenCalledWith('originaldata==', 3, 4);
    expect(result.success).toBe(true);
    expect(result.base64Data).toBe('converteddata==');
    expect(result.fromGeneration).toBe(3);
    expect(result.toGeneration).toBe(4);
    expect(result.fromFormat).toBe('PK3');
    expect(result.toFormat).toBe('PK4');
    expect(result.conversionResult).toBe('Success');
  });

  it('should convert Pokemon from Gen 4 to Gen 5', () => {
    const mockResponse = JSON.stringify({
      success: true,
      base64Data: 'gen5data==',
      fromGeneration: 4,
      toGeneration: 5,
      fromFormat: 'PK4',
      toFormat: 'PK5',
      conversionResult: 'Success',
    });
    mockRawApi.ConvertPKMFormat.mockReturnValue(mockResponse);

    const result = api.pkm.convertFormat('gen4data==', 4, 5);

    expect(result.success).toBe(true);
    expect(result.fromFormat).toBe('PK4');
    expect(result.toFormat).toBe('PK5');
  });

  it('should convert Pokemon from Gen 5 to Gen 6', () => {
    const mockResponse = JSON.stringify({
      success: true,
      base64Data: 'gen6data==',
      fromGeneration: 5,
      toGeneration: 6,
      fromFormat: 'PK5',
      toFormat: 'PK6',
      conversionResult: 'Success',
    });
    mockRawApi.ConvertPKMFormat.mockReturnValue(mockResponse);

    const result = api.pkm.convertFormat('gen5data==', 5, 6);

    expect(result.success).toBe(true);
    expect(result.fromGeneration).toBe(5);
    expect(result.toGeneration).toBe(6);
  });

  it('should convert Pokemon from Gen 6 to Gen 7', () => {
    const mockResponse = JSON.stringify({
      success: true,
      base64Data: 'gen7data==',
      fromGeneration: 6,
      toGeneration: 7,
      fromFormat: 'PK6',
      toFormat: 'PK7',
      conversionResult: 'Success',
    });
    mockRawApi.ConvertPKMFormat.mockReturnValue(mockResponse);

    const result = api.pkm.convertFormat('gen6data==', 6, 7);

    expect(result.success).toBe(true);
    expect(result.fromFormat).toBe('PK6');
    expect(result.toFormat).toBe('PK7');
  });

  it('should convert Pokemon from Gen 7 to Gen 8', () => {
    const mockResponse = JSON.stringify({
      success: true,
      base64Data: 'gen8data==',
      fromGeneration: 7,
      toGeneration: 8,
      fromFormat: 'PK7',
      toFormat: 'PK8',
      conversionResult: 'Success',
    });
    mockRawApi.ConvertPKMFormat.mockReturnValue(mockResponse);

    const result = api.pkm.convertFormat('gen7data==', 7, 8);

    expect(result.success).toBe(true);
    expect(result.toGeneration).toBe(8);
  });

  it('should convert Pokemon from Gen 8 to Gen 9', () => {
    const mockResponse = JSON.stringify({
      success: true,
      base64Data: 'gen9data==',
      fromGeneration: 8,
      toGeneration: 9,
      fromFormat: 'PK8',
      toFormat: 'PK9',
      conversionResult: 'Success',
    });
    mockRawApi.ConvertPKMFormat.mockReturnValue(mockResponse);

    const result = api.pkm.convertFormat('gen8data==', 8, 9);

    expect(result.success).toBe(true);
    expect(result.fromFormat).toBe('PK8');
    expect(result.toFormat).toBe('PK9');
  });

  it('should handle conversion errors', () => {
    const mockResponse = JSON.stringify({
      error: 'No valid conversion route exists between these generations',
      code: 'CONVERSION_FAILED',
    });
    mockRawApi.ConvertPKMFormat.mockReturnValue(mockResponse);

    const result = api.pkm.convertFormat('invaliddata==', 1, 3);

    expect(result.error).toBeDefined();
    expect(result.code).toBe('CONVERSION_FAILED');
  });

  it('should return same format if already in target generation', () => {
    const mockResponse = JSON.stringify({
      success: true,
      base64Data: 'samedata==',
      fromGeneration: 5,
      toGeneration: 5,
      fromFormat: 'PK5',
      toFormat: 'PK5',
      conversionResult: 'None',
    });
    mockRawApi.ConvertPKMFormat.mockReturnValue(mockResponse);

    const result = api.pkm.convertFormat('samedata==', 5, 5);

    expect(result.success).toBe(true);
    expect(result.fromFormat).toBe('PK5');
    expect(result.toFormat).toBe('PK5');
    expect(result.conversionResult).toBe('None');
  });
});
