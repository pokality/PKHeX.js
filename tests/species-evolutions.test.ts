import { describe, it, expect, beforeEach, vi } from 'vitest';
import { createPKHeXApiWrapper } from '../src/api-wrapper';

describe('Species Evolutions API', () => {
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
    };
    api = createPKHeXApiWrapper(mockRawApi);
  });

  it('should get evolution chain for Pikachu', () => {
    const mockResponse = JSON.stringify({
      species: 25,
      speciesName: 'Pikachu',
      generation: 3,
      evolutionChain: [
        { species: 172, speciesName: 'Pichu', form: 0 },
        { species: 25, speciesName: 'Pikachu', form: 0 },
        { species: 26, speciesName: 'Raichu', form: 0 },
      ],
      chainLength: 3,
      forwardEvolutions: [{ species: 26, speciesName: 'Raichu', form: 0 }],
      preEvolutions: [{ species: 172, speciesName: 'Pichu', form: 0 }],
      baseSpecies: 172,
      baseSpeciesName: 'Pichu',
      baseForm: 0,
    });
    mockRawApi.GetSpeciesEvolutions.mockReturnValue(mockResponse);

    const result = api.gameData.getSpeciesEvolutions(25, 3);

    expect(mockRawApi.GetSpeciesEvolutions).toHaveBeenCalledWith(25, 3);
    expect(result.species).toBe(25);
    expect(result.speciesName).toBe('Pikachu');
    expect(result.generation).toBe(3);
    expect(result.chainLength).toBe(3);
    expect(result.evolutionChain).toHaveLength(3);
    expect(result.baseSpecies).toBe(172);
    expect(result.baseSpeciesName).toBe('Pichu');
  });

  it('should get evolution chain for Eevee (multiple evolutions)', () => {
    const mockResponse = JSON.stringify({
      species: 133,
      speciesName: 'Eevee',
      generation: 3,
      evolutionChain: [
        { species: 133, speciesName: 'Eevee', form: 0 },
        { species: 134, speciesName: 'Vaporeon', form: 0 },
        { species: 135, speciesName: 'Jolteon', form: 0 },
        { species: 136, speciesName: 'Flareon', form: 0 },
        { species: 196, speciesName: 'Espeon', form: 0 },
        { species: 197, speciesName: 'Umbreon', form: 0 },
      ],
      chainLength: 6,
      forwardEvolutions: [
        { species: 134, speciesName: 'Vaporeon', form: 0 },
        { species: 135, speciesName: 'Jolteon', form: 0 },
        { species: 136, speciesName: 'Flareon', form: 0 },
        { species: 196, speciesName: 'Espeon', form: 0 },
        { species: 197, speciesName: 'Umbreon', form: 0 },
      ],
      preEvolutions: [],
      baseSpecies: 133,
      baseSpeciesName: 'Eevee',
      baseForm: 0,
    });
    mockRawApi.GetSpeciesEvolutions.mockReturnValue(mockResponse);

    const result = api.gameData.getSpeciesEvolutions(133, 3);

    expect(result.species).toBe(133);
    expect(result.speciesName).toBe('Eevee');
    expect(result.forwardEvolutions).toHaveLength(5);
    expect(result.preEvolutions).toHaveLength(0);
    expect(result.baseSpecies).toBe(133); // Eevee is its own base
  });

  it('should get evolution chain for final evolution', () => {
    const mockResponse = JSON.stringify({
      species: 6,
      speciesName: 'Charizard',
      generation: 3,
      evolutionChain: [
        { species: 4, speciesName: 'Charmander', form: 0 },
        { species: 5, speciesName: 'Charmeleon', form: 0 },
        { species: 6, speciesName: 'Charizard', form: 0 },
      ],
      chainLength: 3,
      forwardEvolutions: [],
      preEvolutions: [
        { species: 4, speciesName: 'Charmander', form: 0 },
        { species: 5, speciesName: 'Charmeleon', form: 0 },
      ],
      baseSpecies: 4,
      baseSpeciesName: 'Charmander',
      baseForm: 0,
    });
    mockRawApi.GetSpeciesEvolutions.mockReturnValue(mockResponse);

    const result = api.gameData.getSpeciesEvolutions(6, 3);

    expect(result.species).toBe(6);
    expect(result.speciesName).toBe('Charizard');
    expect(result.forwardEvolutions).toHaveLength(0); // No further evolutions
    expect(result.preEvolutions).toHaveLength(2);
    expect(result.baseSpecies).toBe(4);
    expect(result.baseSpeciesName).toBe('Charmander');
  });

  it('should handle Pokemon with no evolutions', () => {
    const mockResponse = JSON.stringify({
      species: 132,
      speciesName: 'Ditto',
      generation: 3,
      evolutionChain: [{ species: 132, speciesName: 'Ditto', form: 0 }],
      chainLength: 1,
      forwardEvolutions: [],
      preEvolutions: [],
      baseSpecies: 132,
      baseSpeciesName: 'Ditto',
      baseForm: 0,
    });
    mockRawApi.GetSpeciesEvolutions.mockReturnValue(mockResponse);

    const result = api.gameData.getSpeciesEvolutions(132, 3);

    expect(result.species).toBe(132);
    expect(result.chainLength).toBe(1);
    expect(result.forwardEvolutions).toHaveLength(0);
    expect(result.preEvolutions).toHaveLength(0);
    expect(result.baseSpecies).toBe(132); // Ditto is its own base
  });
});
