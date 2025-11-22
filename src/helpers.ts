import type { ErrorResponse, SuccessResponse, ApiResult } from './index';

export function isError(response: any): response is ErrorResponse {
  return Boolean(response && typeof response === 'object' && 'error' in response);
}

export function isSuccess<T>(response: ApiResult<T>): response is SuccessResponse & T {
  return response && typeof response === 'object' && 'success' in response && response.success === true;
}

export function unwrap<T>(response: ApiResult<T>): SuccessResponse & T {
  if (isError(response)) {
    throw new Error(`PKHeX API Error [${response.code || 'UNKNOWN'}]: ${response.error}`);
  }
  return response as SuccessResponse & T;
}

export function getError(response: any): string | null {
  return isError(response) ? response.error : null;
}
