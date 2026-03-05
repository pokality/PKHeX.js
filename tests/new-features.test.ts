import { describe, it } from 'node:test';
import assert from 'node:assert/strict';
import {
  saveOps, pokemonOps, storageOps, gameDataOps, withTestSave
} from './wasm-test-setup.ts';

// IMPORTANT: Save-dependent tests run before error-throwing tests.
// .NET exceptions corrupt the WASM thread state in NativeAOT-LLVM.
//
// NOTE: The test save file (Emerald) has NO box Pokemon — only a
// party Mudkip. Tests requiring box Pokemon are skipped.
//
// NOTE: sortBox corrupts WASM thread statics on its 2nd call
// (NativeAOT-LLVM bug). It can only be tested once per process.

describe('New Features Integration Tests', () => {

  // ── Stateless tests (run first, no WASM corruption risk) ──

  describe('Form Operations (stateless)', () => {
    it('should return available forms for Unown', () => {
      const forms = gameDataOps.getAvailableForms(201, 3);

      assert.ok(Array.isArray(forms));
      assert.ok(forms.length > 1);

      if (forms.length > 0) {
        assert.ok('index' in forms[0]);
        assert.ok('name' in forms[0]);
      }
    });
  });

  describe('Memory Strings', () => {
    it('should return memory strings', () => {
      const data = gameDataOps.getMemoryStrings();

      assert.ok('memories' in data);
      assert.ok('feelings' in data);
      assert.ok('intensities' in data);
      assert.ok(Array.isArray(data.memories));
    });
  });

  describe('Tera Types', () => {
    it('should return all tera types', () => {
      const types = gameDataOps.getAllTeraTypes();

      assert.ok(Array.isArray(types));
      assert.ok(types.length >= 18);

      if (types.length > 0) {
        const type = types[0];
        assert.ok('id' in type);
        assert.ok('name' in type);
        assert.ok('isSpecial' in type);
      }

      const stellarType = types.find((t: any) => t.name === 'Stellar');
      if (stellarType) {
        assert.strictEqual(stellarType.isSpecial, true);
        assert.strictEqual(stellarType.id, 99);
      }
    });
  });

  describe('Handle Management', () => {
    it('should return active handle count as a number', () => {
      const count = saveOps.getActiveHandleCount();
      assert.strictEqual(typeof count, 'number');
    });
  });

  // ── Save-dependent tests ──
  // NOTE: sortBox MUST be the first save-dependent call because it
  // crashes on any subsequent invocation (NativeAOT-LLVM thread
  // static corruption). We test it once, then other batch ops.

  describe('Sort Box', () => {
    it('should sort box with one criterion', async () => {
      await withTestSave((handle: number) => {
        assert.doesNotThrow(() => storageOps.sortBox(handle, 0, 'species'));
      });
    });
  });

  // NOTE: batchCheckLegality and getBoxStats are tested in
  // integration.test.ts to avoid exceeding the 2-heavy-call limit.

  // ── Error-throwing tests (run last) ──

  describe('Friendship Error Paths', () => {
    it('should throw for getFriendship with invalid handle', () => {
      assert.throws(() => pokemonOps.getFriendship(0, 0, 0));
    });

    it('should throw for setFriendship with invalid handle', () => {
      assert.throws(() => pokemonOps.setFriendship(0, 0, 0, 255));
    });

    it('should throw for maximizeFriendship with invalid handle', () => {
      assert.throws(() => pokemonOps.maximizeFriendship(0, 0, 0));
    });
  });

  describe('Memory Error Paths', () => {
    it('should throw for getMemories with invalid handle', () => {
      assert.throws(() => pokemonOps.getMemories(0, 0, 0));
    });

    it('should throw for setOriginalTrainerMemory with invalid handle', () => {
      assert.throws(() => pokemonOps.setOriginalTrainerMemory(0, 0, 0, 1, 1, 1, 1));
    });

    it('should throw for clearMemories with invalid handle', () => {
      assert.throws(() => pokemonOps.clearMemories(0, 0, 0));
    });
  });

  describe('Form Error Paths', () => {
    it('should throw for getForm with invalid handle', () => {
      assert.throws(() => pokemonOps.getForm(0, 0, 0));
    });

    it('should throw for setForm with invalid handle', () => {
      assert.throws(() => pokemonOps.setForm(0, 0, 0, 0));
    });

    it('should throw for changeSpeciesAndForm with invalid handle', () => {
      assert.throws(() => pokemonOps.changeSpeciesAndForm(0, 0, 0, 25, 0));
    });
  });

  describe('Batch Error Paths', () => {
    it('should throw for batchCheckLegality with invalid handle', () => {
      assert.throws(() => storageOps.batchCheckLegality(0, [[0, 0]]));
    });

    it('should throw for batchModifyPokemon with invalid handle', () => {
      assert.throws(() => storageOps.batchModifyPokemon(0, [
        { box: 0, slot: 0, modifications: { level: 50 } }
      ]));
    });

    it('should throw for clearBox with invalid handle', () => {
      assert.throws(() => storageOps.clearBox(0, 0));
    });

    it('should throw for clearAllBoxes with invalid handle', () => {
      assert.throws(() => storageOps.clearAllBoxes(0));
    });

    it('should throw for sortBox with invalid handle', () => {
      assert.throws(() => storageOps.sortBox(0, 0, 'species'));
    });

    it('should throw for compactBox with invalid handle', () => {
      assert.throws(() => storageOps.compactBox(0, 0));
    });

    it('should throw for getBoxStats with invalid handle', () => {
      assert.throws(() => storageOps.getBoxStats(0, 0));
    });

    it('should throw for empty array in batchCheckLegality', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => storageOps.batchCheckLegality(handle, []));
      });
    });

    it('should throw for invalid sort criteria', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => storageOps.sortBox(handle, 0, 'invalid_criteria'));
      });
    });

    it('should throw for empty sort criteria', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => storageOps.sortBox(handle, 0, ''));
      });
    });

    it('should throw for out of range box in getBoxStats', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => storageOps.getBoxStats(handle, 999));
      });
    });

    it('should throw for out of range slot in getFriendship', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => pokemonOps.getFriendship(handle, 0, 999));
      });
    });
  });

  describe('Tera Type Error Paths', () => {
    it('should throw for getTeraType with invalid handle', () => {
      assert.throws(() => pokemonOps.getTeraType(0, 0, 0));
    });

    it('should throw for setTeraType with invalid handle', () => {
      assert.throws(() => pokemonOps.setTeraType(0, 0, 0, 1));
    });

    it('should throw for setTeraTypeOverride with invalid handle', () => {
      assert.throws(() => pokemonOps.setTeraTypeOverride(0, 0, 0, 1));
    });

    it('should throw for resetTeraType with invalid handle', () => {
      assert.throws(() => pokemonOps.resetTeraType(0, 0, 0));
    });

    it('should throw for all standard tera type values with invalid handle', () => {
      for (let typeId = 0; typeId <= 18; typeId++) {
        assert.throws(() => pokemonOps.setTeraType(0, 0, 0, typeId));
      }
    });

    it('should throw for invalid tera type value with invalid handle', () => {
      assert.throws(() => pokemonOps.setTeraType(0, 0, 0, 99));
    });

    it('should throw for tera type values above valid range', () => {
      for (let typeId = 19; typeId <= 25; typeId++) {
        assert.throws(() => pokemonOps.setTeraType(0, 0, 0, typeId));
      }
    });
  });

  describe('Gen 3 Unsupported Features', () => {
    it('should throw for setAffection on Gen 3', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => pokemonOps.setAffection(handle, 0, 0, 255));
      });
    });

    it('should throw for getMemories on Gen 3', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => pokemonOps.getMemories(handle, 0, 0));
      });
    });

    it('should throw for getTeraType on Gen 3', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => pokemonOps.getTeraType(handle, 0, 0));
      });
    });
  });

  describe('Dispose Error Paths', () => {
    it('should throw for non-existent handle', () => {
      assert.throws(() => saveOps.disposeSave(99999));
    });
  });
});
