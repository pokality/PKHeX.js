import { describe, it, expect, beforeAll } from 'vitest';
import { initializeWASM, getWASM, withTestSave } from './wasm-test-setup';
import { createPKHeXApiWrapper } from '../src/api-wrapper';

/**
 * Integration Tests
 * 
 * These tests use the actual WASM module instead of mocks.
 * They catch serialization errors that unit tests miss.
 */

describe('Integration Tests', () => {
  let rawApi: any;
  let api: any;

  beforeAll(async () => {
    const context = await initializeWASM();
    if (!context.isReady) {
      throw new Error('Failed to initialize WASM for integration tests');
    }
    rawApi = context.rawApi;
    api = createPKHeXApiWrapper(rawApi);
  }, 60000); // 60 second timeout for WASM initialization

  describe('Serialization Validation', () => {
    it('should serialize GetTrainerCard error response', () => {
      // Test with invalid handle to get error response
      const jsonResponse = rawApi.GetTrainerCard(-1);
      
      // Should be valid JSON
      expect(() => JSON.parse(jsonResponse)).not.toThrow();
      
      const parsed = JSON.parse(jsonResponse);
      
      // Should be an error response
      expect(parsed).toHaveProperty('error');
      expect(typeof parsed.error).toBe('string');
    });

    it('should serialize GetTrainerAppearance error response', () => {
      const jsonResponse = rawApi.GetTrainerAppearance(-1);
      const parsed = JSON.parse(jsonResponse);
      
      expect(parsed).toHaveProperty('error');
      expect(typeof parsed.error).toBe('string');
    });

    it('should serialize GetBadges error response', () => {
      const jsonResponse = rawApi.GetBadges(-1);
      const parsed = JSON.parse(jsonResponse);
      
      expect(parsed).toHaveProperty('error');
      expect(typeof parsed.error).toBe('string');
    });

    it('should serialize GetDaycare error response', () => {
      const jsonResponse = rawApi.GetDaycare(-1);
      const parsed = JSON.parse(jsonResponse);
      
      expect(parsed).toHaveProperty('error');
      expect(typeof parsed.error).toBe('string');
    });
  });

  describe('API Wrapper Integration', () => {
    it('should handle GetTrainerCard through wrapper', () => {
      const result = api.save.trainer.getCard(-1);
      
      // Should not have parse errors
      expect(result).toBeDefined();
      expect(result).toHaveProperty('error');
      expect(typeof result.error).toBe('string');
    });

    it('should handle GetTrainerAppearance through wrapper', () => {
      const result = api.save.trainer.getAppearance(-1);
      
      expect(result).toBeDefined();
      expect(result).toHaveProperty('error');
      expect(typeof result.error).toBe('string');
    });
  });

  describe('Error Handling', () => {
    it('should handle invalid handle gracefully', () => {
      const jsonResponse = rawApi.GetTrainerCard(-1);
      const parsed = JSON.parse(jsonResponse);
      
      expect(parsed).toHaveProperty('error');
      expect(typeof parsed.error).toBe('string');
    });

    it('should serialize error responses correctly', () => {
      const jsonResponse = rawApi.GetTrainerCard(999999);
      
      // Should be valid JSON even for errors
      expect(() => JSON.parse(jsonResponse)).not.toThrow();
      
      const parsed = JSON.parse(jsonResponse);
      expect(parsed).toHaveProperty('error');
    });
  });

  describe('Complex Type Serialization', () => {
    it('should serialize nested objects in GetSpeciesForms', () => {
      const jsonResponse = rawApi.GetSpeciesForms(25, 3); // Pikachu, Gen 3
      const parsed = JSON.parse(jsonResponse);
      
      if (parsed.error) {
        expect(typeof parsed.error).toBe('string');
      } else {
        expect(parsed).toHaveProperty('species');
        expect(parsed).toHaveProperty('speciesName');
        expect(parsed).toHaveProperty('forms');
        expect(Array.isArray(parsed.forms)).toBe(true);
        
        if (parsed.forms.length > 0) {
          const form = parsed.forms[0];
          expect(form).toHaveProperty('formIndex');
          expect(form).toHaveProperty('baseStats');
          expect(form.baseStats).toHaveProperty('hp');
          expect(typeof form.baseStats.hp).toBe('number');
        }
      }
    });

    it('should serialize evolution chains in GetSpeciesEvolutions', () => {
      const jsonResponse = rawApi.GetSpeciesEvolutions(25, 3); // Pikachu, Gen 3
      const parsed = JSON.parse(jsonResponse);
      
      if (parsed.error) {
        expect(typeof parsed.error).toBe('string');
      } else {
        expect(parsed).toHaveProperty('species');
        expect(parsed).toHaveProperty('evolutionChain');
        expect(Array.isArray(parsed.evolutionChain)).toBe(true);
        
        if (parsed.evolutionChain.length > 0) {
          const entry = parsed.evolutionChain[0];
          expect(entry).toHaveProperty('species');
          expect(entry).toHaveProperty('speciesName');
          expect(entry).toHaveProperty('form');
        }
      }
    });
  });
});
