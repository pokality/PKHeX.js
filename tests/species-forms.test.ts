import { describe, it, expect, beforeEach, vi } from 'vitest';
import { createPKHeXApiWrapper } from '../src/api-wrapper';

describe('Species Forms API', () => {
  let mockRawApi: any;
  let api: ReturnType<typeof createPKHeXApiWrapper>;

  beforeEach(() => {
    // Create a minimal mock with all required methods
    mockRawApi = {
      // Add all the methods that the wrapper expects
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
    };
    api = createPKHeXApiWrapper(mockRawApi);
  });

  it('should get forms for Pikachu in Gen 3', () => {
    const mockResponse = JSON.stringify({
      species: 25,
      speciesName: 'Pikachu',
      generation: 3,
      formCount: 1,
      forms: [{
        formIndex: 0,
        formName: 'Form 0',
        type1: 13,
        type1Name: 'Electric',
        type2: 13,
        type2Name: 'Electric',
        baseStats: {
          hp: 35,
          attack: 55,
          defense: 40,
          spAttack: 50,
          spDefense: 50,
          speed: 90
        },
        genderRatio: 63,
        isDualGender: true,
        isGenderless: false
      }]
    });
    mockRawApi.GetSpeciesForms.mockReturnValue(mockResponse);

    const result = api.gameData.getSpeciesForms(25, 3);

    expect(mockRawApi.GetSpeciesForms).toHaveBeenCalledWith(25, 3);
    expect(result.species).toBe(25);
    expect(result.speciesName).toBe('Pikachu');
    expect(result.generation).toBe(3);
    expect(result.formCount).toBe(1);
    expect(result.forms).toHaveLength(1);

    const firstForm = result.forms[0];
    expect(firstForm.formIndex).toBe(0);
    expect(firstForm.type1).toBe(13);
    expect(firstForm.type1Name).toBe('Electric');
    expect(firstForm.baseStats.hp).toBe(35);
    expect(firstForm.baseStats.attack).toBe(55);
  });

  it('should get forms for Deoxys in Gen 3 (multiple forms)', () => {
    const mockResponse = JSON.stringify({
      species: 386,
      speciesName: 'Deoxys',
      generation: 3,
      formCount: 4,
      forms: [
        { formIndex: 0, formName: 'Normal', type1: 14, type1Name: 'Psychic', type2: 14, type2Name: 'Psychic', baseStats: { hp: 50, attack: 150, defense: 50, spAttack: 150, spDefense: 50, speed: 150 }, genderRatio: 255, isDualGender: false, isGenderless: true },
        { formIndex: 1, formName: 'Attack', type1: 14, type1Name: 'Psychic', type2: 14, type2Name: 'Psychic', baseStats: { hp: 50, attack: 180, defense: 20, spAttack: 180, spDefense: 20, speed: 150 }, genderRatio: 255, isDualGender: false, isGenderless: true },
        { formIndex: 2, formName: 'Defense', type1: 14, type1Name: 'Psychic', type2: 14, type2Name: 'Psychic', baseStats: { hp: 50, attack: 70, defense: 160, spAttack: 70, spDefense: 160, speed: 90 }, genderRatio: 255, isDualGender: false, isGenderless: true },
        { formIndex: 3, formName: 'Speed', type1: 14, type1Name: 'Psychic', type2: 14, type2Name: 'Psychic', baseStats: { hp: 50, attack: 95, defense: 90, spAttack: 95, spDefense: 90, speed: 180 }, genderRatio: 255, isDualGender: false, isGenderless: true }
      ]
    });
    mockRawApi.GetSpeciesForms.mockReturnValue(mockResponse);

    const result = api.gameData.getSpeciesForms(386, 3);

    expect(result.species).toBe(386);
    expect(result.speciesName).toBe('Deoxys');
    expect(result.formCount).toBe(4);
    expect(result.forms).toHaveLength(4);
  });

  it('should include gender information', () => {
    const mockResponse = JSON.stringify({
      species: 25,
      speciesName: 'Pikachu',
      generation: 3,
      formCount: 1,
      forms: [{
        formIndex: 0,
        formName: 'Form 0',
        type1: 13,
        type1Name: 'Electric',
        type2: 13,
        type2Name: 'Electric',
        baseStats: { hp: 35, attack: 55, defense: 40, spAttack: 50, spDefense: 50, speed: 90 },
        genderRatio: 63,
        isDualGender: true,
        isGenderless: false
      }]
    });
    mockRawApi.GetSpeciesForms.mockReturnValue(mockResponse);

    const result = api.gameData.getSpeciesForms(25, 3);
    const firstForm = result.forms[0];

    expect(firstForm.genderRatio).toBe(63);
    expect(firstForm.isDualGender).toBe(true);
    expect(firstForm.isGenderless).toBe(false);
  });

  it('should include base stats', () => {
    const mockResponse = JSON.stringify({
      species: 25,
      speciesName: 'Pikachu',
      generation: 3,
      formCount: 1,
      forms: [{
        formIndex: 0,
        formName: 'Form 0',
        type1: 13,
        type1Name: 'Electric',
        type2: 13,
        type2Name: 'Electric',
        baseStats: {
          hp: 35,
          attack: 55,
          defense: 40,
          spAttack: 50,
          spDefense: 50,
          speed: 90
        },
        genderRatio: 63,
        isDualGender: true,
        isGenderless: false
      }]
    });
    mockRawApi.GetSpeciesForms.mockReturnValue(mockResponse);

    const result = api.gameData.getSpeciesForms(25, 3);
    const firstForm = result.forms[0];

    expect(firstForm.baseStats).toBeDefined();
    expect(firstForm.baseStats.hp).toBe(35);
    expect(firstForm.baseStats.attack).toBe(55);
    expect(firstForm.baseStats.defense).toBe(40);
    expect(firstForm.baseStats.spAttack).toBe(50);
    expect(firstForm.baseStats.spDefense).toBe(50);
    expect(firstForm.baseStats.speed).toBe(90);
  });
});
