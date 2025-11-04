/**
 * Basic integration tests for PKHeX WASM API
 * Tests type definitions and helper functions
 */

import { describe, it, expect } from 'vitest';
import { isError, isSuccess, unwrap } from '../../src/helpers';

describe('PKHeX API Type Guards', () => {
  describe('isError', () => {
    it('should identify error responses', () => {
      const errorResponse = { error: 'Test error', code: 'TEST_CODE' };
      expect(isError(errorResponse)).toBe(true);
    });

    it('should reject success responses', () => {
      const successResponse = { success: true, data: 'test' };
      expect(isError(successResponse)).toBe(false);
    });

    it('should handle null/undefined', () => {
      expect(isError(null)).toBe(false);
      expect(isError(undefined)).toBe(false);
    });
  });

  describe('isSuccess', () => {
    it('should identify success responses', () => {
      const successResponse = { success: true, handle: 123 };
      expect(isSuccess(successResponse)).toBe(true);
    });

    it('should reject error responses', () => {
      const errorResponse = { error: 'Test error' };
      expect(isSuccess(errorResponse)).toBe(false);
    });

    it('should reject responses without success property', () => {
      const response = { data: 'test' };
      expect(isSuccess(response)).toBe(false);
    });

    it('should reject responses with success: false', () => {
      const response = { success: false };
      expect(isSuccess(response)).toBe(false);
    });
  });

  describe('unwrap', () => {
    it('should return data from success response', () => {
      const successResponse = { success: true, handle: 123, data: 'test' };
      const result = unwrap(successResponse);
      expect(result).toEqual(successResponse);
      expect(result.handle).toBe(123);
    });

    it('should throw on error response', () => {
      const errorResponse = { error: 'Test error', code: 'TEST_CODE' };
      expect(() => unwrap(errorResponse)).toThrow('PKHeX API Error [TEST_CODE]: Test error');
    });

    it('should throw with UNKNOWN code if no code provided', () => {
      const errorResponse = { error: 'Test error' };
      expect(() => unwrap(errorResponse)).toThrow('PKHeX API Error [UNKNOWN]: Test error');
    });
  });
});

describe('API Response Structure', () => {
  it('should have correct success response structure', () => {
    const response = { success: true, handle: 123 };
    
    expect(response).toHaveProperty('success');
    expect(response.success).toBe(true);
    expect(response).toHaveProperty('handle');
    expect(typeof response.handle).toBe('number');
  });

  it('should have correct error response structure', () => {
    const response = { error: 'Test error', code: 'TEST_CODE' };
    
    expect(response).toHaveProperty('error');
    expect(typeof response.error).toBe('string');
    expect(response).toHaveProperty('code');
    expect(typeof response.code).toBe('string');
  });
});
