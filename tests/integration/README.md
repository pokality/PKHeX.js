# Integration Tests

## Overview

Integration tests verify the TypeScript API definitions and helper functions work correctly.

## Running Tests

```bash
# Run integration tests
npm test

# Run in watch mode
npm run test:watch

# Run C# tests
npm run test:csharp

# Run all tests
npm run test:all
```

## Test Structure

- `basic.test.ts` - Tests for type guards and helper functions
- Future: WASM interop tests (requires browser or Node.js WASM environment)

## WASM Integration Testing

Full WASM integration tests require:
1. Building the WASM module (`npm run build`)
2. Setting up a WASM-compatible test environment
3. Loading the .NET runtime in Node.js or browser

For now, the C# test suite (`tests/PKHeX.Tests/`) provides comprehensive API testing.

## Adding Tests

Create new test files in this directory following the pattern:

```typescript
import { describe, it, expect } from 'vitest';

describe('Feature Name', () => {
  it('should do something', () => {
    expect(true).toBe(true);
  });
});
```
