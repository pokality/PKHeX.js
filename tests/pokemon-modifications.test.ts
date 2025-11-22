import { describe, it, expect, beforeEach, vi } from 'vitest';
import { createPKHeXApiWrapper } from '../src/api-wrapper';
import type { PokemonModifications } from '../src/index';

describe('Pokemon Modifications Type Safety', () => {
  let mockRawApi: any;
  let api: ReturnType<typeof createPKHeXApiWrapper>;

  beforeEach(() => {
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

  it('should accept typed modifications for save file Pokemon', () => {
    const mockResponse = JSON.stringify({ success: true, message: 'Pokemon modified' });
    mockRawApi.ModifyPokemon.mockReturnValue(mockResponse);

    const modifications: PokemonModifications = {
      species: 25, // Pikachu
      nickname: 'Sparky',
      level: 50,
      nature: 5, // Modest
      ability: 9, // Static
      heldItem: 229, // Leftovers
      moves: [85, 87, 129, 113], // Thunderbolt, Thunder, Swift, Agility
      ivs: [31, 0, 31, 31, 31, 31], // Perfect except Attack
      evs: [4, 0, 0, 252, 252, 0], // HP/SpA/SpD spread
      gender: 0, // Male
      isShiny: true,
      ot_Name: 'Ash',
      ball: 4, // Great Ball
    };

    const result = api.save.pokemon.modify(1, 0, 0, modifications);

    expect(mockRawApi.ModifyPokemon).toHaveBeenCalledWith(1, 0, 0, JSON.stringify(modifications));
    expect(result).toHaveProperty('success', true);
  });

  it('should accept typed modifications for standalone PKM', () => {
    const mockResponse = JSON.stringify({ base64Data: 'modifieddata==' });
    mockRawApi.ModifyPKMData.mockReturnValue(mockResponse);

    const modifications: PokemonModifications = {
      level: 100,
      nature: 1, // Adamant
      isShiny: false,
    };

    const result = api.pkm.modify('base64data==', 3, modifications);

    expect(mockRawApi.ModifyPKMData).toHaveBeenCalledWith('base64data==', 3, JSON.stringify(modifications));
    expect(result).toHaveProperty('base64Data');
  });

  it('should accept partial modifications (all fields optional)', () => {
    const mockResponse = JSON.stringify({ success: true, message: 'Pokemon modified' });
    mockRawApi.ModifyPokemon.mockReturnValue(mockResponse);

    // Only modify level - all other fields are optional
    const modifications: PokemonModifications = {
      level: 75,
    };

    const result = api.save.pokemon.modify(1, 0, 0, modifications);

    expect(mockRawApi.ModifyPokemon).toHaveBeenCalledWith(1, 0, 0, '{"level":75}');
    expect(result).toHaveProperty('success', true);
  });

  it('should accept complex modifications with arrays', () => {
    const mockResponse = JSON.stringify({ base64Data: 'complexdata==' });
    mockRawApi.ModifyPKMData.mockReturnValue(mockResponse);

    const modifications: PokemonModifications = {
      species: 150, // Mewtwo
      moves: [94, 105, 129, 347], // Psychic, Recover, Swift, Calm Mind
      ivs: [31, 31, 31, 31, 31, 31], // Perfect IVs
      evs: [252, 0, 4, 252, 0, 0], // HP/SpA spread
    };

    const result = api.pkm.modify('base64data==', 3, modifications);

    const expectedJson = JSON.stringify(modifications);
    expect(mockRawApi.ModifyPKMData).toHaveBeenCalledWith('base64data==', 3, expectedJson);
    expect(result).toHaveProperty('base64Data');
  });

  it('should handle empty modifications object', () => {
    const mockResponse = JSON.stringify({ success: true, message: 'No changes made' });
    mockRawApi.ModifyPokemon.mockReturnValue(mockResponse);

    const modifications: PokemonModifications = {};

    const result = api.save.pokemon.modify(1, 0, 0, modifications);

    expect(mockRawApi.ModifyPokemon).toHaveBeenCalledWith(1, 0, 0, '{}');
    expect(result).toHaveProperty('success', true);
  });
});