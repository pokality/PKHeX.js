# PKHeX WASM API Migration Guide

## Overview

The PKHeX WASM API has been migrated from returning JSON strings to returning objects directly. This eliminates the need for manual JSON parsing and provides better TypeScript support.

## Breaking Changes

### Before (Old API)
```typescript
// Methods returned JSON strings that needed parsing
const result = api.GetSaveInfo(handle);
const data = JSON.parse(result);

if (data.error) {
  console.error(data.error);
} else {
  console.log(data.data.gameVersion);
}
```

### After (New API)
```typescript
// Methods return objects directly
const result = api.GetSaveInfo(handle);

if (isError(result)) {
  console.error(result.error);
} else {
  console.log(result.gameVersion);
}
```

## Key Differences

### 1. No JSON Parsing Required
**Old:**
```typescript
const jsonString = api.GetAllPokemon(handle);
const parsed = JSON.parse(jsonString);
const pokemon = parsed.data;
```

**New:**
```typescript
const result = api.GetAllPokemon(handle);
if (isSuccess(result)) {
  const pokemon = result.pokemon;
}
```

### 2. Direct Property Access
**Old:**
```typescript
const info = JSON.parse(api.GetSaveInfo(handle));
console.log(info.data.gameVersion);
```

**New:**
```typescript
const info = api.GetSaveInfo(handle);
if (isSuccess(info)) {
  console.log(info.gameVersion);
}
```

### 3. Type Safety
**Old:**
```typescript
// No type safety, manual type assertions needed
const result: any = JSON.parse(api.GetPokemon(handle, 0, 0));
```

**New:**
```typescript
// Full TypeScript support
const result = api.GetPokemon(handle, 0, 0);
// TypeScript knows the exact shape of the response
```

## Response Format

### Successful Responses
All successful responses include `success: true` plus the data:

```typescript
{
  success: true,
  // ...data properties
}
```

Examples:
```typescript
// Single value
{ success: true, handle: 123 }

// Object
{ success: true, gameVersion: "Diamond", ot: "ASH", ... }

// Array
{ success: true, pokemon: [...] }

// Message
{ success: true, message: "Pokemon modified successfully" }
```

### Error Responses
Error responses include `error` and optionally `code`:

```typescript
{
  error: string;
  code?: string;
}
```

Example:
```typescript
{
  error: "Invalid save file handle",
  code: "INVALID_HANDLE"
}
```

## Helper Functions

### Type Guards

```typescript
import { isError, isSuccess } from 'pkhex';

const result = api.GetSaveInfo(handle);

// Check for errors
if (isError(result)) {
  console.error(result.error, result.code);
  return;
}

// Check for success
if (isSuccess(result)) {
  console.log(result.gameVersion);
}
```

### Unwrap Helper

```typescript
import { unwrap } from 'pkhex/helpers';

try {
  const info = unwrap(api.GetSaveInfo(handle));
  console.log(info.gameVersion);
} catch (error) {
  console.error('Failed to get save info:', error.message);
}
```

## Migration Checklist

- [ ] Remove all `JSON.parse()` calls on API responses
- [ ] Update error handling to check for `error` property instead of parsing
- [ ] Update data access to use direct properties instead of `.data`
- [ ] Add type guards (`isError`, `isSuccess`) for better type safety
- [ ] Update TypeScript types to use `ApiResult<T>` instead of `string`
- [ ] Test all API calls to ensure they work with the new format

## Common Patterns

### Pattern 1: Load and Validate Save
```typescript
const loadResult = api.LoadSave(base64Data);

if (isError(loadResult)) {
  throw new Error(`Failed to load save: ${loadResult.error}`);
}

const handle = loadResult.handle;
```

### Pattern 2: Get Data with Error Handling
```typescript
const result = api.GetAllPokemon(handle);

if (isError(result)) {
  console.error('Error:', result.error);
  return [];
}

return result.pokemon;
```

### Pattern 3: Modify and Check Success
```typescript
const result = api.ModifyPokemon(handle, box, slot, JSON.stringify(mods));

if (isSuccess(result)) {
  console.log('Success:', result.message);
} else {
  console.error('Failed:', result.error);
}
```

## Benefits

1. **No Manual Parsing**: Objects are ready to use immediately
2. **Better Performance**: Eliminates JSON serialization/deserialization overhead
3. **Type Safety**: Full TypeScript support with proper type inference
4. **Cleaner Code**: Less boilerplate, more readable
5. **Better DX**: IDE autocomplete and type checking work perfectly

## Backward Compatibility

If you need to serialize responses back to JSON (e.g., for storage or transmission):

```typescript
const result = api.GetSaveInfo(handle);
const jsonString = JSON.stringify(result);
```

## Support

For issues or questions about the migration, please open an issue on GitHub.
