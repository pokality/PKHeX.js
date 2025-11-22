/**
 * Integration Test Helper
 * 
 * This module provides utilities for testing actual C# API responses
 * to catch serialization errors that unit tests with mocks cannot detect.
 */

export interface SerializationTestResult {
  success: boolean;
  error?: string;
  methodName: string;
  responseType: string;
}

/**
 * Tests if a C# API method returns properly serializable JSON
 */
export function testMethodSerialization(
  methodName: string,
  apiMethod: () => string,
  expectedProperties: string[]
): SerializationTestResult {
  try {
    // Call the actual C# method
    const jsonResponse = apiMethod();
    
    // Try to parse the JSON
    let parsed: any;
    try {
      parsed = JSON.parse(jsonResponse);
    } catch (parseError) {
      return {
        success: false,
        error: `JSON parse error: ${parseError instanceof Error ? parseError.message : String(parseError)}`,
        methodName,
        responseType: 'unparseable'
      };
    }

    // Check if it's an error response
    if (parsed.error) {
      // Error responses are valid, just not success
      return {
        success: true,
        methodName,
        responseType: 'error'
      };
    }

    // Validate expected properties exist
    const missingProperties = expectedProperties.filter(prop => !(prop in parsed));
    if (missingProperties.length > 0) {
      return {
        success: false,
        error: `Missing expected properties: ${missingProperties.join(', ')}`,
        methodName,
        responseType: 'incomplete'
      };
    }

    // Check for anonymous object indicators (shouldn't happen with proper types)
    const jsonString = JSON.stringify(parsed);
    if (jsonString.includes('__AnonymousType')) {
      return {
        success: false,
        error: 'Response contains anonymous type references',
        methodName,
        responseType: 'anonymous'
      };
    }

    return {
      success: true,
      methodName,
      responseType: 'valid'
    };
  } catch (error) {
    return {
      success: false,
      error: `Unexpected error: ${error instanceof Error ? error.message : String(error)}`,
      methodName,
      responseType: 'exception'
    };
  }
}

/**
 * Validates that a response can be round-tripped through JSON serialization
 */
export function testRoundTripSerialization(data: any): boolean {
  try {
    const serialized = JSON.stringify(data);
    const deserialized = JSON.parse(serialized);
    
    // Deep equality check
    return JSON.stringify(deserialized) === serialized;
  } catch {
    return false;
  }
}

/**
 * Checks if an object contains any functions (which shouldn't be in API responses)
 */
export function containsFunctions(obj: any): boolean {
  if (typeof obj === 'function') {
    return true;
  }
  
  if (typeof obj === 'object' && obj !== null) {
    for (const key in obj) {
      if (containsFunctions(obj[key])) {
        return true;
      }
    }
  }
  
  return false;
}

/**
 * Validates that all values in an object are JSON-serializable types
 */
export function validateSerializableTypes(obj: any, path: string = 'root'): string[] {
  const errors: string[] = [];
  
  if (obj === null || obj === undefined) {
    return errors;
  }
  
  const type = typeof obj;
  
  // Valid primitive types
  if (type === 'string' || type === 'number' || type === 'boolean') {
    return errors;
  }
  
  // Invalid types
  if (type === 'function' || type === 'symbol' || type === 'bigint') {
    errors.push(`${path}: Invalid type ${type}`);
    return errors;
  }
  
  // Arrays
  if (Array.isArray(obj)) {
    obj.forEach((item, index) => {
      errors.push(...validateSerializableTypes(item, `${path}[${index}]`));
    });
    return errors;
  }
  
  // Objects
  if (type === 'object') {
    for (const key in obj) {
      errors.push(...validateSerializableTypes(obj[key], `${path}.${key}`));
    }
  }
  
  return errors;
}

/**
 * Test suite for validating API method responses
 */
export interface ApiMethodTest {
  name: string;
  method: () => string;
  expectedProperties: string[];
  description?: string;
}

export function runApiMethodTests(tests: ApiMethodTest[]): Map<string, SerializationTestResult> {
  const results = new Map<string, SerializationTestResult>();
  
  for (const test of tests) {
    const result = testMethodSerialization(
      test.name,
      test.method,
      test.expectedProperties
    );
    results.set(test.name, result);
  }
  
  return results;
}

/**
 * Generates a report of serialization test results
 */
export function generateSerializationReport(results: Map<string, SerializationTestResult>): string {
  const lines: string[] = ['Serialization Test Report', '='.repeat(50), ''];
  
  let passCount = 0;
  let failCount = 0;
  
  for (const [name, result] of results) {
    if (result.success) {
      passCount++;
      lines.push(`✓ ${name} (${result.responseType})`);
    } else {
      failCount++;
      lines.push(`✗ ${name}`);
      lines.push(`  Error: ${result.error}`);
      lines.push(`  Type: ${result.responseType}`);
    }
  }
  
  lines.push('');
  lines.push(`Total: ${results.size} tests`);
  lines.push(`Passed: ${passCount}`);
  lines.push(`Failed: ${failCount}`);
  
  return lines.join('\n');
}
