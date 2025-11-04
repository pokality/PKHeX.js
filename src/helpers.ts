/**
 * Helper functions for working with PKHeX WASM API responses
 */

import type { ErrorResponse, SuccessResponse, ApiResult } from './index';

/**
 * Type guard to check if response is an error
 * @param response API response
 * @returns True if response is an error
 */
export function isError(response: any): response is ErrorResponse {
  return Boolean(response && typeof response === 'object' && 'error' in response);
}

/**
 * Type guard to check if response is successful
 * @param response API response
 * @returns True if response is successful
 */
export function isSuccess<T>(response: ApiResult<T>): response is SuccessResponse & T {
  return response && typeof response === 'object' && 'success' in response && response.success === true;
}

/**
 * Unwrap a successful response or throw an error
 * @param response API response
 * @returns The data from a successful response
 * @throws Error if response is an error
 */
export function unwrap<T>(response: ApiResult<T>): SuccessResponse & T {
  if (isError(response)) {
    throw new Error(`PKHeX API Error [${response.code || 'UNKNOWN'}]: ${response.error}`);
  }
  return response as SuccessResponse & T;
}

/**
 * Get error message from response, or null if successful
 * @param response API response
 * @returns Error message or null
 */
export function getError(response: any): string | null {
  return isError(response) ? response.error : null;
}
