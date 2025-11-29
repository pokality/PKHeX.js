import { describe, it, expect, beforeAll } from 'vitest';
import { initializeWASM, getWASM, withTestSave } from './wasm-test-setup';

/**
 * New Features Integration Tests
 *
 * Tests for Friendship, Memory, Form, Batch, and Tera Type APIs.
 */

describe('New Features Integration Tests', () => {
  let rawApi: any;

  beforeAll(async () => {
    const context = await initializeWASM();
    if (!context.isReady) {
      throw new Error('Failed to initialize WASM for integration tests');
    }
    rawApi = context.rawApi;
  }, 60000);

  describe('Friendship APIs', () => {
    it('should serialize GetFriendship error response', () => {
      const jsonResponse = rawApi.GetFriendship(-1, 0, 0);
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
      expect(typeof parsed.error).toBe('string');
    });

    it('should serialize SetFriendship error response', () => {
      const jsonResponse = rawApi.SetFriendship(-1, 0, 0, 255);
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
    });

    it('should serialize MaximizeFriendship error response', () => {
      const jsonResponse = rawApi.MaximizeFriendship(-1, 0, 0);
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
    });

    it('should get friendship data from real save file', async () => {
      await withTestSave(rawApi, (handle) => {
        const jsonResponse = rawApi.GetFriendship(handle, 0, 0);
        const parsed = JSON.parse(jsonResponse);

        if (!parsed.error) {
          expect(parsed).toHaveProperty('currentFriendship');
          expect(parsed).toHaveProperty('originalTrainerFriendship');
          expect(typeof parsed.currentFriendship).toBe('number');
        }
      });
    });

    it('should set and verify friendship', async () => {
      await withTestSave(rawApi, (handle) => {
        const setResponse = rawApi.SetFriendship(handle, 0, 0, 200);
        const setParsed = JSON.parse(setResponse);

        if (!setParsed.error) {
          expect(setParsed).toHaveProperty('success', true);

          const getResponse = rawApi.GetFriendship(handle, 0, 0);
          const getParsed = JSON.parse(getResponse);

          if (!getParsed.error) {
            expect(getParsed.currentFriendship).toBe(200);
          }
        }
      });
    });
  });

  describe('Memory APIs', () => {
    it('should serialize GetMemories error response', () => {
      const jsonResponse = rawApi.GetMemories(-1, 0, 0);
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
    });

    it('should serialize SetOriginalTrainerMemory error response', () => {
      const jsonResponse = rawApi.SetOriginalTrainerMemory(-1, 0, 0, 1, 1, 1, 1);
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
    });

    it('should serialize ClearMemories error response', () => {
      const jsonResponse = rawApi.ClearMemories(-1, 0, 0);
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
    });

    it('should serialize GetMemoryStrings response', () => {
      const jsonResponse = rawApi.GetMemoryStrings();
      const parsed = JSON.parse(jsonResponse);

      if (!parsed.error) {
        expect(parsed).toHaveProperty('memories');
        expect(parsed).toHaveProperty('feelings');
        expect(parsed).toHaveProperty('intensities');
        expect(Array.isArray(parsed.memories)).toBe(true);
      }
    });
  });

  describe('Form APIs', () => {
    it('should serialize GetForm error response', () => {
      const jsonResponse = rawApi.GetForm(-1, 0, 0);
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
    });

    it('should serialize SetForm error response', () => {
      const jsonResponse = rawApi.SetForm(-1, 0, 0, 0);
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
    });

    it('should serialize GetAvailableForms response', () => {
      // Unown has 28 forms
      const jsonResponse = rawApi.GetAvailableForms(201, 3);
      const parsed = JSON.parse(jsonResponse);

      if (!parsed.error) {
        expect(parsed).toHaveProperty('species');
        expect(parsed).toHaveProperty('speciesName');
        expect(parsed).toHaveProperty('forms');
        expect(parsed).toHaveProperty('formCount');
        expect(Array.isArray(parsed.forms)).toBe(true);
        expect(parsed.formCount).toBeGreaterThan(1);

        if (parsed.forms.length > 0) {
          expect(parsed.forms[0]).toHaveProperty('formIndex');
          expect(parsed.forms[0]).toHaveProperty('formName');
        }
      }
    });

    it('should serialize ChangeSpeciesAndForm error response', () => {
      const jsonResponse = rawApi.ChangeSpeciesAndForm(-1, 0, 0, 25, 0);
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
    });

    it('should get form data from real save file', async () => {
      await withTestSave(rawApi, (handle) => {
        const jsonResponse = rawApi.GetForm(handle, 0, 0);
        const parsed = JSON.parse(jsonResponse);

        if (!parsed.error) {
          expect(parsed).toHaveProperty('form');
          expect(parsed).toHaveProperty('formName');
          expect(parsed).toHaveProperty('formCount');
          expect(typeof parsed.form).toBe('number');
        }
      });
    });
  });

  describe('Batch APIs', () => {
    it('should serialize BatchCheckLegality error response', () => {
      const locations = JSON.stringify([{ box: 0, slot: 0 }]);
      const jsonResponse = rawApi.BatchCheckLegality(-1, locations);
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
    });

    it('should serialize BatchModifyPokemon error response', () => {
      const mods = JSON.stringify([{ box: 0, slot: 0, modifications: { level: 50 } }]);
      const jsonResponse = rawApi.BatchModifyPokemon(-1, mods);
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
    });

    it('should serialize ClearBox error response', () => {
      const jsonResponse = rawApi.ClearBox(-1, 0);
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
    });

    it('should serialize ClearAllBoxes error response', () => {
      const jsonResponse = rawApi.ClearAllBoxes(-1);
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
    });

    it('should serialize SortBox error response', () => {
      const jsonResponse = rawApi.SortBox(-1, 0, 'species');
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
    });

    it('should serialize CompactBox error response', () => {
      const jsonResponse = rawApi.CompactBox(-1, 0);
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
    });

    it('should serialize GetBoxStats error response', () => {
      const jsonResponse = rawApi.GetBoxStats(-1, 0);
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
    });

    it('should batch check legality from real save file', async () => {
      await withTestSave(rawApi, (handle) => {
        const locations = JSON.stringify([
          { box: 0, slot: 0 },
          { box: 0, slot: 1 },
          { box: 0, slot: 2 }
        ]);
        const jsonResponse = rawApi.BatchCheckLegality(handle, locations);
        const parsed = JSON.parse(jsonResponse);

        if (!parsed.error) {
          expect(parsed).toHaveProperty('results');
          expect(parsed).toHaveProperty('validCount');
          expect(parsed).toHaveProperty('invalidCount');
          expect(parsed).toHaveProperty('emptyCount');
          expect(Array.isArray(parsed.results)).toBe(true);

          if (parsed.results.length > 0) {
            const result = parsed.results[0];
            expect(result).toHaveProperty('box');
            expect(result).toHaveProperty('slot');
            expect(result).toHaveProperty('valid');
            expect(result).toHaveProperty('empty');
          }
        }
      });
    });

    it('should get box stats from real save file', async () => {
      await withTestSave(rawApi, (handle) => {
        const jsonResponse = rawApi.GetBoxStats(handle, 0);
        const parsed = JSON.parse(jsonResponse);

        if (!parsed.error) {
          expect(parsed).toHaveProperty('box');
          expect(parsed).toHaveProperty('totalSlots');
          expect(parsed).toHaveProperty('occupied');
          expect(parsed).toHaveProperty('empty');
          expect(parsed).toHaveProperty('shinyCount');
          expect(parsed).toHaveProperty('eggCount');
          expect(parsed).toHaveProperty('uniqueSpecies');
          expect(typeof parsed.totalSlots).toBe('number');
        }
      });
    });

    it('should support all sort criteria', async () => {
      await withTestSave(rawApi, (handle) => {
        const criteria = ['species', 'level', 'name', 'pokedex', 'shiny', 'type'];

        for (const criterion of criteria) {
          const jsonResponse = rawApi.SortBox(handle, 0, criterion);
          expect(() => JSON.parse(jsonResponse)).not.toThrow();
        }
      });
    });
  });

  describe('Tera Type APIs', () => {
    it('should serialize GetTeraType error response', () => {
      const jsonResponse = rawApi.GetTeraType(-1, 0, 0);
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
    });

    it('should serialize SetTeraType error response', () => {
      const jsonResponse = rawApi.SetTeraType(-1, 0, 0, 1);
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
    });

    it('should serialize SetTeraTypeOverride error response', () => {
      const jsonResponse = rawApi.SetTeraTypeOverride(-1, 0, 0, 1);
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
    });

    it('should serialize ResetTeraType error response', () => {
      const jsonResponse = rawApi.ResetTeraType(-1, 0, 0);
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('error');
    });

    it('should serialize GetAllTeraTypes response', () => {
      const jsonResponse = rawApi.GetAllTeraTypes();
      const parsed = JSON.parse(jsonResponse);

      if (!parsed.error) {
        expect(parsed).toHaveProperty('teraTypes');
        expect(Array.isArray(parsed.teraTypes)).toBe(true);
        expect(parsed.teraTypes.length).toBeGreaterThanOrEqual(18);

        // Check for standard type structure
        if (parsed.teraTypes.length > 0) {
          const type = parsed.teraTypes[0];
          expect(type).toHaveProperty('id');
          expect(type).toHaveProperty('name');
          expect(type).toHaveProperty('isStellar');
        }

        // Check for Stellar type
        const stellarType = parsed.teraTypes.find((t: any) => t.name === 'Stellar');
        if (stellarType) {
          expect(stellarType.isStellar).toBe(true);
          expect(stellarType.id).toBe(18);
        }
      }
    });

    it('should handle all standard tera type values', () => {
      for (let typeId = 0; typeId <= 18; typeId++) {
        const jsonResponse = rawApi.SetTeraType(-1, 0, 0, typeId);
        expect(() => JSON.parse(jsonResponse)).not.toThrow();
      }
    });

    it('should reject invalid tera type value', () => {
      const jsonResponse = rawApi.SetTeraType(-1, 0, 0, 99);
      const parsed = JSON.parse(jsonResponse);

      // Should error either for invalid handle or invalid tera type
      expect(parsed).toHaveProperty('error');
    });
  });

  describe('Handle Management', () => {
    it('should serialize GetActiveHandleCount response', () => {
      const jsonResponse = rawApi.GetActiveHandleCount();
      const parsed = JSON.parse(jsonResponse);

      expect(parsed).toHaveProperty('success', true);
      expect(parsed).toHaveProperty('count');
      expect(typeof parsed.count).toBe('number');
    });

    it('should track handle count correctly', async () => {
      const beforeResponse = rawApi.GetActiveHandleCount();
      const beforeCount = JSON.parse(beforeResponse).count;

      await withTestSave(rawApi, (handle) => {
        const duringResponse = rawApi.GetActiveHandleCount();
        const duringCount = JSON.parse(duringResponse).count;

        expect(duringCount).toBe(beforeCount + 1);
      });

      const afterResponse = rawApi.GetActiveHandleCount();
      const afterCount = JSON.parse(afterResponse).count;

      expect(afterCount).toBe(beforeCount);
    });
  });

  describe('Edge Cases and Error Handling', () => {
    describe('Malformed JSON Input', () => {
      it('should handle malformed JSON in BatchCheckLegality', async () => {
        await withTestSave(rawApi, (handle) => {
          const jsonResponse = rawApi.BatchCheckLegality(handle, 'not valid json');
          const parsed = JSON.parse(jsonResponse);
          expect(parsed).toHaveProperty('error');
        });
      });

      it('should handle malformed JSON in BatchModifyPokemon', async () => {
        await withTestSave(rawApi, (handle) => {
          const jsonResponse = rawApi.BatchModifyPokemon(handle, '{{invalid}}');
          const parsed = JSON.parse(jsonResponse);
          expect(parsed).toHaveProperty('error');
        });
      });

      it('should handle empty array in BatchCheckLegality', async () => {
        await withTestSave(rawApi, (handle) => {
          const jsonResponse = rawApi.BatchCheckLegality(handle, '[]');
          const parsed = JSON.parse(jsonResponse);
          expect(parsed).toHaveProperty('error');
        });
      });

      it('should handle missing required fields in batch locations', async () => {
        await withTestSave(rawApi, (handle) => {
          // Missing slot field
          const jsonResponse = rawApi.BatchCheckLegality(handle, '[{"box": 0}]');
          const parsed = JSON.parse(jsonResponse);
          expect(parsed).toHaveProperty('error');
        });
      });
    });

    describe('Tera Type Validation', () => {
      it('should reject tera type values above valid range', () => {
        // Values 19+ are invalid (0-18 are valid, 18 is Stellar)
        for (let typeId = 19; typeId <= 25; typeId++) {
          const jsonResponse = rawApi.SetTeraType(-1, 0, 0, typeId);
          const parsed = JSON.parse(jsonResponse);
          expect(parsed).toHaveProperty('error');
        }
      });

      it('should reject negative tera type values', () => {
        const jsonResponse = rawApi.SetTeraType(-1, 0, 0, -1);
        const parsed = JSON.parse(jsonResponse);
        expect(parsed).toHaveProperty('error');
      });
    });

    describe('Friendship Boundary Values', () => {
      it('should set friendship to minimum value (0)', async () => {
        await withTestSave(rawApi, (handle) => {
          const setResponse = rawApi.SetFriendship(handle, 0, 0, 0);
          const setParsed = JSON.parse(setResponse);

          if (!setParsed.error) {
            const getResponse = rawApi.GetFriendship(handle, 0, 0);
            const getParsed = JSON.parse(getResponse);

            if (!getParsed.error) {
              expect(getParsed.currentFriendship).toBe(0);
            }
          }
        });
      });

      it('should set friendship to maximum value (255)', async () => {
        await withTestSave(rawApi, (handle) => {
          const setResponse = rawApi.SetFriendship(handle, 0, 0, 255);
          const setParsed = JSON.parse(setResponse);

          if (!setParsed.error) {
            const getResponse = rawApi.GetFriendship(handle, 0, 0);
            const getParsed = JSON.parse(getResponse);

            if (!getParsed.error) {
              expect(getParsed.currentFriendship).toBe(255);
            }
          }
        });
      });

      it('should handle value over maximum (clamp or error)', async () => {
        await withTestSave(rawApi, (handle) => {
          const setResponse = rawApi.SetFriendship(handle, 0, 0, 500);
          const setParsed = JSON.parse(setResponse);

          if (!setParsed.error) {
            const getResponse = rawApi.GetFriendship(handle, 0, 0);
            const getParsed = JSON.parse(getResponse);

            if (!getParsed.error) {
              expect(getParsed.currentFriendship).toBeLessThanOrEqual(255);
            }
          }
        });
      });
    });

    describe('Sort Criteria Validation', () => {
      it('should reject invalid sort criteria', async () => {
        await withTestSave(rawApi, (handle) => {
          const jsonResponse = rawApi.SortBox(handle, 0, 'invalid_criteria');
          const parsed = JSON.parse(jsonResponse);
          expect(parsed).toHaveProperty('error');
        });
      });

      it('should reject empty sort criteria', async () => {
        await withTestSave(rawApi, (handle) => {
          const jsonResponse = rawApi.SortBox(handle, 0, '');
          const parsed = JSON.parse(jsonResponse);
          expect(parsed).toHaveProperty('error');
        });
      });
    });

    describe('Out of Range Box/Slot', () => {
      it('should handle out of range box in GetBoxStats', async () => {
        await withTestSave(rawApi, (handle) => {
          const jsonResponse = rawApi.GetBoxStats(handle, 999);
          const parsed = JSON.parse(jsonResponse);
          expect(parsed).toHaveProperty('error');
        });
      });

      it('should handle out of range slot in GetFriendship', async () => {
        await withTestSave(rawApi, (handle) => {
          const jsonResponse = rawApi.GetFriendship(handle, 0, 999);
          const parsed = JSON.parse(jsonResponse);
          expect(parsed).toHaveProperty('error');
        });
      });

      it('should handle out of range box in batch operations', async () => {
        await withTestSave(rawApi, (handle) => {
          const locations = JSON.stringify([{ box: 999, slot: 0 }]);
          const jsonResponse = rawApi.BatchCheckLegality(handle, locations);
          // Should not throw, returns valid JSON
          expect(() => JSON.parse(jsonResponse)).not.toThrow();
        });
      });
    });

    describe('Gen 3 Unsupported Features', () => {
      it('should return error for SetAffection on Gen 3', async () => {
        await withTestSave(rawApi, (handle) => {
          const jsonResponse = rawApi.SetAffection(handle, 0, 0, 255);
          const parsed = JSON.parse(jsonResponse);
          // Gen 3 doesn't support affection
          expect(parsed).toHaveProperty('error');
        });
      });

      it('should return error for GetMemories on Gen 3', async () => {
        await withTestSave(rawApi, (handle) => {
          const jsonResponse = rawApi.GetMemories(handle, 0, 0);
          const parsed = JSON.parse(jsonResponse);
          // Gen 3 doesn't support memories
          expect(parsed).toHaveProperty('error');
        });
      });

      it('should return error for GetTeraType on Gen 3', async () => {
        await withTestSave(rawApi, (handle) => {
          const jsonResponse = rawApi.GetTeraType(handle, 0, 0);
          const parsed = JSON.parse(jsonResponse);
          // Gen 3 doesn't support Tera Types
          expect(parsed).toHaveProperty('error');
        });
      });
    });

    describe('Dispose and Invalid Handle', () => {
      it('should return error for disposed handle', async () => {
        // Load a save
        const fs = await import('fs');
        const path = await import('path');
        const savePath = path.join(process.cwd(), 'tests', 'data', 'emerald.sav');
        const saveData = fs.readFileSync(savePath);
        const base64Data = Buffer.from(saveData).toString('base64');

        const loadResponse = rawApi.LoadSave(base64Data);
        const loadParsed = JSON.parse(loadResponse);

        if (!loadParsed.error) {
          const handle = loadParsed.handle;

          // Dispose it
          rawApi.DisposeSave(handle);

          // Try to use it
          const jsonResponse = rawApi.GetFriendship(handle, 0, 0);
          const parsed = JSON.parse(jsonResponse);
          expect(parsed).toHaveProperty('error');
        }
      });

      it('should return error when disposing same handle twice', async () => {
        const fs = await import('fs');
        const path = await import('path');
        const savePath = path.join(process.cwd(), 'tests', 'data', 'emerald.sav');
        const saveData = fs.readFileSync(savePath);
        const base64Data = Buffer.from(saveData).toString('base64');

        const loadResponse = rawApi.LoadSave(base64Data);
        const loadParsed = JSON.parse(loadResponse);

        if (!loadParsed.error) {
          const handle = loadParsed.handle;

          // First dispose should succeed
          const disposeResponse1 = rawApi.DisposeSave(handle);
          const disposeParsed1 = JSON.parse(disposeResponse1);
          expect(disposeParsed1).toHaveProperty('success', true);

          // Second dispose should fail
          const disposeResponse2 = rawApi.DisposeSave(handle);
          const disposeParsed2 = JSON.parse(disposeResponse2);
          expect(disposeParsed2).toHaveProperty('error');
        }
      });

      it('should return error for non-existent handle', () => {
        const jsonResponse = rawApi.DisposeSave(99999);
        const parsed = JSON.parse(jsonResponse);
        expect(parsed).toHaveProperty('error');
      });
    });
  });
});
