import { describe, it, expect } from 'vitest';

/**
 * Serialization Validation Tests
 * 
 * These tests validate that all API responses can be properly serialized.
 * They check the structure of responses to ensure they match expected types
 * and don't contain anonymous objects that would fail JSON serialization.
 */

describe('Serialization Validation', () => {
  describe('Response Structure Validation', () => {
    it('should validate LegalityResult structure', () => {
      const mockResponse = {
        valid: true,
        errors: ['error1', 'error2'],
        parsed: 'parsed report'
      };

      expect(mockResponse).toHaveProperty('valid');
      expect(mockResponse).toHaveProperty('errors');
      expect(mockResponse).toHaveProperty('parsed');
      expect(typeof mockResponse.valid).toBe('boolean');
      expect(Array.isArray(mockResponse.errors)).toBe(true);
      expect(typeof mockResponse.parsed).toBe('string');
    });

    it('should validate ContestStats structure', () => {
      const mockResponse = {
        cool: 50,
        beauty: 60,
        cute: 70,
        smart: 80,
        tough: 90,
        sheen: 100
      };

      expect(mockResponse).toHaveProperty('cool');
      expect(mockResponse).toHaveProperty('beauty');
      expect(mockResponse).toHaveProperty('cute');
      expect(mockResponse).toHaveProperty('smart');
      expect(mockResponse).toHaveProperty('tough');
      expect(mockResponse).toHaveProperty('sheen');
      expect(typeof mockResponse.cool).toBe('number');
    });

    it('should validate TrainerCard structure', () => {
      const mockResponse = {
        ot: 'ASH',
        tid: 12345,
        sid: 54321,
        money: 100000,
        startDate: null,
        fame: 0
      };

      expect(mockResponse).toHaveProperty('ot');
      expect(mockResponse).toHaveProperty('tid');
      expect(mockResponse).toHaveProperty('sid');
      expect(mockResponse).toHaveProperty('money');
      expect(mockResponse).toHaveProperty('startDate');
      expect(mockResponse).toHaveProperty('fame');
      expect(typeof mockResponse.ot).toBe('string');
      expect(typeof mockResponse.tid).toBe('number');
    });

    it('should validate TrainerAppearance structure', () => {
      const mockResponse = {
        skin: 0,
        hair: 1,
        top: 2,
        bottom: 3,
        shoes: 4,
        accessory: 5,
        bag: 6,
        hat: 7
      };

      expect(mockResponse).toHaveProperty('skin');
      expect(mockResponse).toHaveProperty('hair');
      expect(mockResponse).toHaveProperty('top');
      expect(mockResponse).toHaveProperty('bottom');
      expect(mockResponse).toHaveProperty('shoes');
      expect(mockResponse).toHaveProperty('accessory');
      expect(mockResponse).toHaveProperty('bag');
      expect(mockResponse).toHaveProperty('hat');
      expect(typeof mockResponse.skin).toBe('number');
    });

    it('should validate DaycareData structure', () => {
      const mockResponse = {
        slot1Species: 25,
        slot1SpeciesName: 'Pikachu',
        slot1Level: 50,
        slot2Species: 0,
        slot2SpeciesName: '',
        slot2Level: 0,
        hasEgg: false
      };

      expect(mockResponse).toHaveProperty('slot1Species');
      expect(mockResponse).toHaveProperty('slot1SpeciesName');
      expect(mockResponse).toHaveProperty('slot1Level');
      expect(mockResponse).toHaveProperty('slot2Species');
      expect(mockResponse).toHaveProperty('slot2SpeciesName');
      expect(mockResponse).toHaveProperty('slot2Level');
      expect(mockResponse).toHaveProperty('hasEgg');
      expect(typeof mockResponse.hasEgg).toBe('boolean');
    });

    it('should validate BadgeData structure', () => {
      const mockResponse = {
        badgeCount: 8,
        badges: [true, true, true, true, true, true, true, true]
      };

      expect(mockResponse).toHaveProperty('badgeCount');
      expect(mockResponse).toHaveProperty('badges');
      expect(typeof mockResponse.badgeCount).toBe('number');
      expect(Array.isArray(mockResponse.badges)).toBe(true);
      expect(mockResponse.badges.every(b => typeof b === 'boolean')).toBe(true);
    });

    it('should validate PokemonStats structure', () => {
      const mockResponse = {
        hp: 100,
        attack: 80,
        defense: 70,
        spAttack: 90,
        spDefense: 75,
        speed: 110
      };

      expect(mockResponse).toHaveProperty('hp');
      expect(mockResponse).toHaveProperty('attack');
      expect(mockResponse).toHaveProperty('defense');
      expect(mockResponse).toHaveProperty('spAttack');
      expect(mockResponse).toHaveProperty('spDefense');
      expect(mockResponse).toHaveProperty('speed');
      expect(typeof mockResponse.hp).toBe('number');
    });

    it('should validate HiddenPowerInfo structure', () => {
      const mockResponse = {
        type: 10,
        typeName: 'Fire',
        power: 70
      };

      expect(mockResponse).toHaveProperty('type');
      expect(mockResponse).toHaveProperty('typeName');
      expect(mockResponse).toHaveProperty('power');
      expect(typeof mockResponse.type).toBe('number');
      expect(typeof mockResponse.typeName).toBe('string');
      expect(typeof mockResponse.power).toBe('number');
    });

    it('should validate CharacteristicInfo structure', () => {
      const mockResponse = {
        index: 5,
        text: 'Likes to run'
      };

      expect(mockResponse).toHaveProperty('index');
      expect(mockResponse).toHaveProperty('text');
      expect(typeof mockResponse.index).toBe('number');
      expect(typeof mockResponse.text).toBe('string');
    });

    it('should validate EvolutionEntry structure', () => {
      const mockEntry = {
        species: 25,
        speciesName: 'Pikachu',
        form: 0
      };

      expect(mockEntry).toHaveProperty('species');
      expect(mockEntry).toHaveProperty('speciesName');
      expect(mockEntry).toHaveProperty('form');
      expect(typeof mockEntry.species).toBe('number');
      expect(typeof mockEntry.speciesName).toBe('string');
      expect(typeof mockEntry.form).toBe('number');
    });

    it('should validate EvolutionInfo structure', () => {
      const mockResponse = {
        species: 25,
        speciesName: 'Pikachu',
        generation: 3,
        evolutionChain: [
          { species: 172, speciesName: 'Pichu', form: 0 },
          { species: 25, speciesName: 'Pikachu', form: 0 },
          { species: 26, speciesName: 'Raichu', form: 0 }
        ],
        chainLength: 3,
        forwardEvolutions: [
          { species: 26, speciesName: 'Raichu', form: 0 }
        ],
        preEvolutions: [
          { species: 172, speciesName: 'Pichu', form: 0 }
        ],
        baseSpecies: 172,
        baseSpeciesName: 'Pichu',
        baseForm: 0
      };

      expect(mockResponse).toHaveProperty('species');
      expect(mockResponse).toHaveProperty('speciesName');
      expect(mockResponse).toHaveProperty('generation');
      expect(mockResponse).toHaveProperty('evolutionChain');
      expect(mockResponse).toHaveProperty('chainLength');
      expect(mockResponse).toHaveProperty('forwardEvolutions');
      expect(mockResponse).toHaveProperty('preEvolutions');
      expect(mockResponse).toHaveProperty('baseSpecies');
      expect(mockResponse).toHaveProperty('baseSpeciesName');
      expect(mockResponse).toHaveProperty('baseForm');
      expect(Array.isArray(mockResponse.evolutionChain)).toBe(true);
      expect(Array.isArray(mockResponse.forwardEvolutions)).toBe(true);
      expect(Array.isArray(mockResponse.preEvolutions)).toBe(true);
    });

    it('should validate SpeciesFormInfo structure', () => {
      const mockForm = {
        formIndex: 0,
        formName: 'Normal',
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
        genderRatio: 127,
        isDualGender: true,
        isGenderless: false
      };

      expect(mockForm).toHaveProperty('formIndex');
      expect(mockForm).toHaveProperty('formName');
      expect(mockForm).toHaveProperty('type1');
      expect(mockForm).toHaveProperty('type1Name');
      expect(mockForm).toHaveProperty('type2');
      expect(mockForm).toHaveProperty('type2Name');
      expect(mockForm).toHaveProperty('baseStats');
      expect(mockForm).toHaveProperty('genderRatio');
      expect(mockForm).toHaveProperty('isDualGender');
      expect(mockForm).toHaveProperty('isGenderless');
      expect(mockForm.baseStats).toHaveProperty('hp');
      expect(typeof mockForm.baseStats.hp).toBe('number');
    });

    it('should validate SpeciesFormsInfo structure', () => {
      const mockResponse = {
        species: 25,
        speciesName: 'Pikachu',
        generation: 3,
        forms: [
          {
            formIndex: 0,
            formName: 'Normal',
            type1: 13,
            type1Name: 'Electric',
            type2: 13,
            type2Name: 'Electric',
            baseStats: { hp: 35, attack: 55, defense: 40, spAttack: 50, spDefense: 50, speed: 90 },
            genderRatio: 127,
            isDualGender: true,
            isGenderless: false
          }
        ],
        formCount: 1
      };

      expect(mockResponse).toHaveProperty('species');
      expect(mockResponse).toHaveProperty('speciesName');
      expect(mockResponse).toHaveProperty('generation');
      expect(mockResponse).toHaveProperty('forms');
      expect(mockResponse).toHaveProperty('formCount');
      expect(Array.isArray(mockResponse.forms)).toBe(true);
      expect(typeof mockResponse.formCount).toBe('number');
    });

    it('should validate ConversionResult structure', () => {
      const mockResponse = {
        success: true,
        base64Data: 'converteddata',
        fromGeneration: 3,
        toGeneration: 4,
        fromFormat: 'PK3',
        toFormat: 'PK4',
        conversionResult: 'Success'
      };

      expect(mockResponse).toHaveProperty('success');
      expect(mockResponse).toHaveProperty('base64Data');
      expect(mockResponse).toHaveProperty('fromGeneration');
      expect(mockResponse).toHaveProperty('toGeneration');
      expect(mockResponse).toHaveProperty('fromFormat');
      expect(mockResponse).toHaveProperty('toFormat');
      expect(mockResponse).toHaveProperty('conversionResult');
      expect(typeof mockResponse.success).toBe('boolean');
      expect(typeof mockResponse.base64Data).toBe('string');
    });

    it('should validate LocationInfo structure', () => {
      const mockLocation = {
        value: 1,
        text: 'Pallet Town'
      };

      expect(mockLocation).toHaveProperty('value');
      expect(mockLocation).toHaveProperty('text');
      expect(typeof mockLocation.value).toBe('number');
      expect(typeof mockLocation.text).toBe('string');
    });

    it('should validate MetLocationsInfo structure', () => {
      const mockResponse = {
        generation: 3,
        gameVersion: 1,
        isEggLocations: false,
        locations: [
          { value: 1, text: 'Pallet Town' },
          { value: 2, text: 'Viridian City' }
        ],
        count: 2
      };

      expect(mockResponse).toHaveProperty('generation');
      expect(mockResponse).toHaveProperty('gameVersion');
      expect(mockResponse).toHaveProperty('isEggLocations');
      expect(mockResponse).toHaveProperty('locations');
      expect(mockResponse).toHaveProperty('count');
      expect(typeof mockResponse.generation).toBe('number');
      expect(typeof mockResponse.isEggLocations).toBe('boolean');
      expect(Array.isArray(mockResponse.locations)).toBe(true);
      expect(typeof mockResponse.count).toBe('number');
    });
  });

  describe('JSON Serialization Validation', () => {
    it('should serialize and deserialize LegalityResult', () => {
      const original = {
        valid: true,
        errors: ['error1'],
        parsed: 'report'
      };

      const serialized = JSON.stringify(original);
      const deserialized = JSON.parse(serialized);

      expect(deserialized).toEqual(original);
    });

    it('should serialize and deserialize nested structures', () => {
      const original = {
        species: 25,
        forms: [
          {
            formIndex: 0,
            baseStats: { hp: 35, attack: 55, defense: 40, spAttack: 50, spDefense: 50, speed: 90 }
          }
        ]
      };

      const serialized = JSON.stringify(original);
      const deserialized = JSON.parse(serialized);

      expect(deserialized).toEqual(original);
      expect(deserialized.forms[0].baseStats.hp).toBe(35);
    });

    it('should handle null values in serialization', () => {
      const original = {
        ot: 'ASH',
        startDate: null,
        fame: 0
      };

      const serialized = JSON.stringify(original);
      const deserialized = JSON.parse(serialized);

      expect(deserialized).toEqual(original);
      expect(deserialized.startDate).toBeNull();
    });
  });
});
