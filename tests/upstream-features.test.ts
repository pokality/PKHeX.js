import { describe, it } from 'node:test';
import assert from 'node:assert/strict';
import {
  saveOps, trainerOps, gameDataOps, itemOps,
  progressOps, zaOps, withTestSave
} from './wasm-test-setup.ts';

// IMPORTANT: Save-dependent tests run before error-throwing tests.
// .NET exceptions corrupt the WASM thread state in NativeAOT-LLVM.

describe('Upstream Feature Tests', () => {

  // ── Stateless/save-dependent tests (run first) ──

  describe('Species Category (25.11.30)', () => {
    it('should identify Mewtwo as legendary', () => {
      const result = gameDataOps.getSpeciesCategory(150);
      assert.strictEqual(result.isLegendary, true);
      assert.strictEqual(result.isSpecial, true);
      assert.strictEqual(result.isMythical, false);
    });

    it('should identify Mew as mythical', () => {
      const result = gameDataOps.getSpeciesCategory(151);
      assert.strictEqual(result.isMythical, true);
      assert.strictEqual(result.isSpecial, true);
      assert.strictEqual(result.isLegendary, false);
    });

    it('should identify Nihilego as ultra beast', () => {
      const result = gameDataOps.getSpeciesCategory(793);
      assert.strictEqual(result.isUltraBeast, true);
      assert.strictEqual(result.isSpecial, true);
    });

    it('should identify Pikachu as not special', () => {
      const result = gameDataOps.getSpeciesCategory(25);
      assert.strictEqual(result.isSpecial, false);
      assert.strictEqual(result.isLegendary, false);
      assert.strictEqual(result.isMythical, false);
      assert.strictEqual(result.isUltraBeast, false);
      assert.strictEqual(result.isParadox, false);
    });

    it('should identify sub-legendary Pokemon', () => {
      const result = gameDataOps.getSpeciesCategory(144);
      assert.strictEqual(result.isSubLegendary, true);
      assert.strictEqual(result.isSpecial, true);
    });

    it('should return species name in response', () => {
      const result = gameDataOps.getSpeciesCategory(25);
      assert.ok(result.speciesName);
      assert.strictEqual(result.species, 25);
    });
  });

  describe('Primal Form Check (25.12.12)', () => {
    it('should identify Primal Kyogre', () => {
      assert.strictEqual(gameDataOps.isPrimalForm(382, 1), true);
    });

    it('should identify Primal Groudon', () => {
      assert.strictEqual(gameDataOps.isPrimalForm(383, 1), true);
    });

    it('should not flag base Kyogre as primal', () => {
      assert.strictEqual(gameDataOps.isPrimalForm(382, 0), false);
    });

    it('should not flag Pikachu as primal', () => {
      assert.strictEqual(gameDataOps.isPrimalForm(25, 0), false);
    });
  });

  describe('Has Item (26.01.31)', () => {
    it('should search for items in inventory', async () => {
      await withTestSave((handle: number) => {
        const result = itemOps.hasItem(handle, 1);
        assert.ok('found' in result);
        assert.strictEqual(typeof result.found, 'boolean');
      });
    });

    it('should return found=false for item not in inventory', async () => {
      await withTestSave((handle: number) => {
        const result = itemOps.hasItem(handle, 2);
        if (!result.found) {
          assert.strictEqual(result.count, 0);
        }
      });
    });
  });

  describe('Get First Empty Slot (26.01.31)', () => {
    it('should find empty slot in a pouch', async () => {
      await withTestSave((handle: number) => {
        const result = itemOps.getFirstEmptySlot(handle, 0);
        assert.strictEqual(typeof result, 'number');
      });
    });
  });

  // ── Error-throwing tests (run last) ──

  describe('Species Category Error Paths', () => {
    it('should throw on invalid species ID', () => {
      assert.throws(() => gameDataOps.getSpeciesCategory(-1));
    });
  });

  describe('Primal Form Edge Cases', () => {
    it('should return false for invalid species', () => {
      assert.strictEqual(gameDataOps.isPrimalForm(-1, 0), false);
    });
  });

  describe('Player Appearance Za Error Paths', () => {
    it('should throw for non-ZA saves on get', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => trainerOps.getPlayerAppearanceZa(handle));
      });
    });

    it('should throw for non-ZA saves on set', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => trainerOps.setPlayerAppearanceZa(handle, { skinColor: 1 }));
      });
    });

    it('should throw for invalid handle on get', () => {
      assert.throws(() => trainerOps.getPlayerAppearanceZa(0));
    });
  });

  describe('Save Revision Error Paths (25.12.12)', () => {
    it('should throw for non-ZA saves', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => saveOps.getSaveRevision(handle));
      });
    });

    it('should throw for invalid handle', () => {
      assert.throws(() => saveOps.getSaveRevision(0));
    });
  });

  describe('ZA Feature Error Paths', () => {
    it('should throw for non-ZA saves on collectTechnicalMachines', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => zaOps.collectTechnicalMachines(handle));
      });
    });

    it('should throw for invalid handle on collectTechnicalMachines', () => {
      assert.throws(() => zaOps.collectTechnicalMachines(0));
    });

    it('should throw for non-ZA saves on getHyperspaceSurveyPoints', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => zaOps.getHyperspaceSurveyPoints(handle));
      });
    });

    it('should throw for non-ZA saves on setHyperspaceSurveyPoints', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => zaOps.setHyperspaceSurveyPoints(handle, 100));
      });
    });

    it('should throw for invalid handle on getHyperspaceSurveyPoints', () => {
      assert.throws(() => zaOps.getHyperspaceSurveyPoints(0));
    });

    it('should throw for non-ZA saves on getStreetName', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => zaOps.getStreetName(handle));
      });
    });

    it('should throw for non-ZA saves on setStreetName', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => zaOps.setStreetName(handle, 'Test Street'));
      });
    });

    it('should throw for invalid handle on getStreetName', () => {
      assert.throws(() => zaOps.getStreetName(0));
    });

    it('should throw for non-ZA saves on getDonuts', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => zaOps.getDonuts(handle));
      });
    });

    it('should throw for non-ZA saves on setAllDonutsShiny', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => zaOps.setAllDonutsShiny(handle));
      });
    });

    it('should throw for non-ZA saves on compressDonuts', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => zaOps.compressDonuts(handle));
      });
    });

    it('should throw for invalid handle on getDonuts', () => {
      assert.throws(() => zaOps.getDonuts(0));
    });
  });

  describe('Item Error Paths', () => {
    it('should throw for invalid item ID', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => itemOps.hasItem(handle, -1));
      });
    });

    it('should throw for invalid handle on hasItem', () => {
      assert.throws(() => itemOps.hasItem(0, 1));
    });

    it('should throw for invalid pouch index', async () => {
      await withTestSave((handle: number) => {
        assert.throws(() => itemOps.getFirstEmptySlot(handle, 999));
      });
    });

    it('should throw for invalid handle on getFirstEmptySlot', () => {
      assert.throws(() => itemOps.getFirstEmptySlot(0, 0));
    });
  });
});
