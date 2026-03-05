import { describe, it } from 'node:test';
import assert from 'node:assert/strict';
import {
  saveOps, pokemonOps, trainerOps, storageOps, itemOps,
  progressOps, gameDataOps, zaOps, withTestSave
} from './wasm-test-setup.ts';

// IMPORTANT: Tests using the shared save handle MUST run before
// error-throwing tests. .NET exceptions in NativeAOT-LLVM corrupt
// the WASM thread state, making subsequent export calls unreliable.
//
// NOTE: sortBox corrupts WASM thread statics on its 2nd call
// (NativeAOT-LLVM bug). It is tested once in new-features.test.ts,
// NOT in this file.

describe('Integration Tests', () => {

  // ── Stateless tests (no WASM corruption risk) ──

  describe('Complex Type Returns', () => {
    it('should return nested objects from getSpeciesForms', () => {
      const result = gameDataOps.getSpeciesForms(25, 3);

      assert.ok('species' in result);
      assert.ok('speciesName' in result);
      assert.ok('forms' in result);
      assert.ok(Array.isArray(result.forms));

      if (result.forms.length > 0) {
        const form = result.forms[0];
        assert.ok('formIndex' in form);
        assert.ok('baseStats' in form);
        assert.ok('hp' in form.baseStats);
        assert.strictEqual(typeof form.baseStats.hp, 'number');
      }
    });

    it('should return evolution chains from getSpeciesEvolutions', () => {
      const result = gameDataOps.getSpeciesEvolutions(25, 3);

      assert.ok('species' in result);
      assert.ok('chain' in result);
      assert.ok(Array.isArray(result.chain));

      if (result.chain.length > 0) {
        const entry = result.chain[0];
        assert.ok('species' in entry);
        assert.ok('speciesName' in entry);
        assert.ok('form' in entry);
      }
    });
  });

  describe('Handle Management', () => {
    it('should return active handle count as a number', () => {
      const count = saveOps.getActiveHandleCount();
      assert.strictEqual(typeof count, 'number');
    });
  });

  // ── Save-dependent tests (limited WASM calls to avoid corruption) ──
  // NOTE: Batch operations run FIRST because they crash after too many
  // prior WASM export calls (NativeAOT-LLVM thread static corruption).

  describe('Batch Operations (save-dependent)', () => {
    it('should batch check legality from real save file', async () => {
      await withTestSave((handle: number) => {
        const locations: Array<[number, number]> = [[0, 0], [0, 1], [0, 2]];
        const result = storageOps.batchCheckLegality(handle, locations);

        assert.ok('results' in result);
        assert.ok('validCount' in result);
        assert.ok('invalidCount' in result);
        assert.ok('emptyCount' in result);
        assert.ok(Array.isArray(result.results));

        if (result.results.length > 0) {
          const entry = result.results[0];
          assert.ok('box' in entry);
          assert.ok('slot' in entry);
          assert.ok('valid' in entry);
          assert.ok('empty' in entry);
        }
      });
    });

    it('should get box stats from real save file', async () => {
      await withTestSave((handle: number) => {
        const stats = storageOps.getBoxStats(handle, 0);

        assert.ok('box' in stats);
        assert.ok('totalSlots' in stats);
        assert.ok('occupied' in stats);
        assert.ok('empty' in stats);
        assert.ok('shinyCount' in stats);
        assert.ok('eggCount' in stats);
        assert.ok('uniqueSpecies' in stats);
        assert.strictEqual(typeof stats.totalSlots, 'number');
      });
    });
  });

  describe('Inventory Operations (save-dependent)', () => {
    it('should get pouches and add items', async () => {
      // Combined into one test to minimize WASM export calls.
      // NativeAOT-LLVM thread statics corrupt after ~4 heavy calls.
      // removeItemFromPouch is skipped (would be the 5th heavy call).
      await withTestSave((handle: number) => {
        const pouches = itemOps.getPouchItems(handle);
        assert.ok(Array.isArray(pouches));

        if (pouches.length > 0) {
          const pouch = pouches[0];
          assert.ok('pouchType' in pouch);
          assert.ok('pouchIndex' in pouch);
          assert.ok('items' in pouch);
          assert.ok('maxSlots' in pouch);
          assert.ok(Array.isArray(pouch.items));

          if (pouch.items.length > 0) {
            const item = pouch.items[0];
            assert.ok('itemId' in item);
            assert.ok('itemName' in item);
            assert.ok('count' in item);
            assert.strictEqual(typeof item.itemId, 'number');
            assert.strictEqual(typeof item.count, 'number');
          }
        }

        itemOps.addItemToPouch(handle, 1, 5, 0);
      });
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

  // ── Error-throwing tests (run last — .NET exceptions corrupt WASM state) ──

  describe('Error Handling', () => {
    it('should throw for invalid handle on getTrainerCard', () => {
      assert.throws(() => trainerOps.getTrainerCard(0));
    });

    it('should throw for non-existent handle on getTrainerCard', () => {
      assert.throws(() => trainerOps.getTrainerCard(999999));
    });

    it('should throw for invalid handle on getBadges', () => {
      assert.throws(() => progressOps.getBadges(0));
    });

    it('should throw for invalid handle on getDaycare', () => {
      assert.throws(() => storageOps.getDaycare(0));
    });
  });

  describe('Save Progress Error Paths', () => {
    it('should throw for invalid handle on collectColorfulScrews', () => {
      assert.throws(() => zaOps.collectColorfulScrews(0));
    });

    it('should throw for invalid handle on getColorfulScrewLocations', () => {
      assert.throws(() => zaOps.getColorfulScrewLocations(0, false));
      assert.throws(() => zaOps.getColorfulScrewLocations(0, true));
    });

    it('should throw for invalid handle on getInfiniteRoyalePoints', () => {
      assert.throws(() => zaOps.getInfiniteRoyalePoints(0));
    });

    it('should throw for invalid handle on setInfiniteRoyalePoints', () => {
      const maxUint32 = 4294967295;
      assert.throws(() => zaOps.setInfiniteRoyalePoints(0, maxUint32, maxUint32));
    });
  });

  describe('Save Configuration Error Paths', () => {
    it('should throw for invalid handle on setTextSpeed', () => {
      assert.throws(() => zaOps.setTextSpeed(0, 3));
    });

    it('should throw for invalid handle on getTextSpeed', () => {
      assert.throws(() => zaOps.getTextSpeed(0));
    });

    it('should throw for all text speed values with invalid handle', () => {
      for (let speed = 0; speed <= 3; speed++) {
        assert.throws(() => zaOps.setTextSpeed(0, speed));
      }
    });
  });

  describe('Save Features Error Paths', () => {
    it('should throw for invalid handle on unlockFashionCategory', () => {
      assert.throws(() => zaOps.unlockFashionCategory(0, 'tops'));
    });

    it('should throw for all fashion categories with invalid handle', () => {
      const categories = [
        'tops', 'bottoms', 'allinone', 'headwear', 'eyewear',
        'gloves', 'legwear', 'footwear', 'satchels', 'earrings'
      ];

      for (const category of categories) {
        assert.throws(() => zaOps.unlockFashionCategory(0, category));
      }
    });

    it('should throw for invalid handle on unlockAllFashion', () => {
      assert.throws(() => zaOps.unlockAllFashion(0));
    });

    it('should throw for invalid handle on unlockAllHairMakeup', () => {
      assert.throws(() => zaOps.unlockAllHairMakeup(0));
    });
  });

  describe('Inventory Error Paths', () => {
    it('should throw for invalid handle on getPouchItems', () => {
      assert.throws(() => itemOps.getPouchItems(0));
    });

    it('should throw for invalid handle on addItemToPouch', () => {
      assert.throws(() => itemOps.addItemToPouch(0, 1, 1, 0));
    });

    it('should throw for invalid handle on removeItemFromPouch', () => {
      assert.throws(() => itemOps.removeItemFromPouch(0, 1, 1));
    });

    it('should throw for invalid item IDs', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => itemOps.addItemToPouch(handle, 999999, 1, 0));
      });
    });

    it('should throw for invalid pouch index', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => itemOps.addItemToPouch(handle, 1, 1, 999));
      });
    });
  });

  describe('Box Error Paths', () => {
    it('should throw for invalid handle on clearBox', () => {
      assert.throws(() => storageOps.clearBox(0, 0));
    });

    it('should throw for invalid handle on clearAllBoxes', () => {
      assert.throws(() => storageOps.clearAllBoxes(0));
    });

    it('should throw for invalid handle on compactBox', () => {
      assert.throws(() => storageOps.compactBox(0, 0));
    });

    it('should throw for invalid handle on sortBox', () => {
      assert.throws(() => storageOps.sortBox(0, 0, 'species'));
    });

    it('should reject invalid sort criteria', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => storageOps.sortBox(handle, 0, 'invalid_criteria'));
      });
    });

    it('should reject empty sort criteria', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => storageOps.sortBox(handle, 0, ''));
      });
    });

    it('should throw for out of range box in getBoxStats', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => storageOps.getBoxStats(handle, 999));
      });
    });
  });
});
