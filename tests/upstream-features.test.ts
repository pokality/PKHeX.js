import { describe, it, expect, beforeAll } from 'vitest';
import { initializeWASM, withTestSave } from './wasm-test-setup';
import { createPKHeXApiWrapper } from '../src/api-wrapper';

describe('Upstream Feature Tests', () => {
  let rawApi: any;
  let api: any;

  beforeAll(async () => {
    const context = await initializeWASM();
    if (!context.isReady) {
      throw new Error('Failed to initialize WASM for upstream feature tests');
    }
    rawApi = context.rawApi;
    api = createPKHeXApiWrapper(rawApi);
  }, 60000);

  // ========================================================================
  // 25.11.30: SpeciesCategory
  // ========================================================================
  describe('Species Category (25.11.30)', () => {
    it('should identify Mewtwo as legendary', () => {
      const result = api.gameData.getSpeciesCategory(150);
      expect(result).not.toHaveProperty('error');
      expect(result.isLegendary).toBe(true);
      expect(result.isSpecial).toBe(true);
      expect(result.isMythical).toBe(false);
    });

    it('should identify Mew as mythical', () => {
      const result = api.gameData.getSpeciesCategory(151);
      expect(result).not.toHaveProperty('error');
      expect(result.isMythical).toBe(true);
      expect(result.isSpecial).toBe(true);
      expect(result.isLegendary).toBe(false);
    });

    it('should identify Nihilego as ultra beast', () => {
      const result = api.gameData.getSpeciesCategory(793);
      expect(result).not.toHaveProperty('error');
      expect(result.isUltraBeast).toBe(true);
      expect(result.isSpecial).toBe(true);
    });

    it('should identify Pikachu as not special', () => {
      const result = api.gameData.getSpeciesCategory(25);
      expect(result).not.toHaveProperty('error');
      expect(result.isSpecial).toBe(false);
      expect(result.isLegendary).toBe(false);
      expect(result.isMythical).toBe(false);
      expect(result.isUltraBeast).toBe(false);
      expect(result.isParadox).toBe(false);
    });

    it('should identify sub-legendary Pokemon', () => {
      // Articuno (144) is sub-legendary
      const result = api.gameData.getSpeciesCategory(144);
      expect(result).not.toHaveProperty('error');
      expect(result.isSubLegendary).toBe(true);
      expect(result.isSpecial).toBe(true);
    });

    it('should return species name in response', () => {
      const result = api.gameData.getSpeciesCategory(25);
      expect(result).not.toHaveProperty('error');
      expect(result.speciesName).toBeTruthy();
      expect(result.species).toBe(25);
    });

    it('should error on invalid species ID', () => {
      const result = api.gameData.getSpeciesCategory(-1);
      expect(result).toHaveProperty('error');
    });

    it('should work via raw API', () => {
      const jsonResponse = rawApi.GetSpeciesCategory(150);
      const parsed = JSON.parse(jsonResponse);
      expect(parsed.success).toBe(true);
      expect(parsed.isLegendary).toBe(true);
    });
  });

  // ========================================================================
  // 25.11.30: PlayerAppearance9a (ZA-specific, error path only)
  // ========================================================================
  describe('Player Appearance 9a (25.11.30)', () => {
    it('should error for non-ZA saves on get', async () => {
      await withTestSave(rawApi, (handle) => {
        const result = api.save.trainer.getPlayerAppearance9a(handle);
        expect(result).toHaveProperty('error');
        expect(result.code).toBe('UNSUPPORTED_GENERATION');
      });
    });

    it('should error for non-ZA saves on set', async () => {
      await withTestSave(rawApi, (handle) => {
        const result = api.save.trainer.setPlayerAppearance9a(handle, { skinColor: 1 });
        expect(result).toHaveProperty('error');
        expect(result.code).toBe('UNSUPPORTED_GENERATION');
      });
    });

    it('should error for invalid handle on get', () => {
      const result = api.save.trainer.getPlayerAppearance9a(-1);
      expect(result).toHaveProperty('error');
    });

    it('should serialize raw API error response', () => {
      const jsonResponse = rawApi.GetPlayerAppearance9a(-1);
      const parsed = JSON.parse(jsonResponse);
      expect(parsed).toHaveProperty('error');
    });
  });
});
