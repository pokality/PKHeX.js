# PKHeX WASM Library

[![CI](https://github.com/monokrome/PKHeX.js/actions/workflows/ci.yml/badge.svg)](https://github.com/yourusername/pkhex-wasm/actions/workflows/ci.yml)
[![Release](https://github.com/monokrome/PKHeX.js/actions/workflows/release.yml/badge.svg)](https://github.com/yourusername/pkhex-wasm/actions/workflows/release.yml)
[![npm version](https://badge.fury.io/js/pkhex-wasm.svg)](https://www.npmjs.com/package/pkhex-wasm)
[![codecov](https://codecov.io/gh/monokrome/PKHeX.js/branch/main/graph/badge.svg)](https://codecov.io/gh/yourusername/pkhex-wasm)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)

A WebAssembly library that wraps [PKHeX.Core](https://github.com/kwsch/PKHeX) to provide comprehensive Pokemon save file editing functionality for JavaScript/TypeScript applications. Edit Pokemon save files directly in the browser with full access to PKHeX's powerful save editing capabilities.

## Features

- **Complete Save File Support**: Load and edit save files from all Pokemon generations (Gen 1-9)
- **Pokemon Management**: View, modify, import/export, and validate Pokemon data
- **Trainer Data**: Edit trainer information, appearance, and progress
- **Inventory Management**: Modify items, Pokedex, badges, and game progress
- **Legality Checking**: Validate Pokemon legality using PKHeX's comprehensive rule engine
- **Showdown Format**: Import and export Pokemon in Showdown battle simulator format
- **Type-Safe API**: Full TypeScript definitions for all API methods
- **Browser-Native**: Runs entirely in the browser with no server required

## Prerequisites

### For Building

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- `wasm-tools` workload for .NET:
  ```bash
  dotnet workload install wasm-tools
  ```

### For Using

- Modern web browser with WebAssembly support (Chrome 57+, Firefox 52+, Safari 11+, Edge 16+)
- JavaScript/TypeScript runtime environment

## Installation

### From NPM (Coming Soon)

```bash
npm install pkhex
```

### From Source

1. Clone the repository with submodules:
   ```bash
   git clone --recursive https://github.com/monokrome/PKHeX.js.git
   cd pkhex
   ```

2. Build the WASM library:
   ```bash
   dotnet publish src/PKHeX/PKHeX.csproj -c Release
   ```

3. The compiled WASM files will be in the `dist/` directory

## Build Process

### Standard Build

```bash
dotnet publish src/PKHeX/PKHeX.csproj -c Release
```

This produces:
- `dist/dotnet.js` - .NET WebAssembly runtime loader
- `dist/dotnet.wasm` - .NET runtime WebAssembly binary
- `dist/PKHeX.dll` - Compiled library assembly
- Additional supporting files in `dist/_framework/`

### Build Configuration

The project is configured with:
- **Target Framework**: .NET 9.0
- **Runtime**: browser-wasm
- **IL Trimming**: Enabled for size optimization
- **JSExport**: Enabled for JavaScript interoperability
- **Exception Handling**: Enabled for proper error reporting

### Bundle Size

Actual bundle sizes (Release build with IL trimming enabled):
- `dotnet.native.wasm`: 2.8 MB (compressed: ~900 KB with Brotli)
- `PKHeX.dll`: 176 KB (compressed: ~50 KB with Brotli)
- `PKHeX.Core.dll`: 16 MB (compressed: ~4.5 MB with Brotli)
- Supporting runtime DLLs: ~3 MB (compressed: ~900 KB with Brotli)
- **Total**: ~22 MB uncompressed, ~6-8 MB with Brotli compression

Note: The large size is primarily due to PKHeX.Core's comprehensive game data and legality checking logic. With proper HTTP compression (Brotli or gzip), the actual download size is significantly reduced.

## API Overview

The PKHeX WASM API returns objects directly - **no JSON parsing needed!** All methods return either:
- **Success**: `{ success: true, ...data }` 
- **Error**: `{ error: string, code?: string }`

Use the provided type guards for clean error handling:
- `isSuccess(result)` - Check if operation succeeded
- `isError(result)` - Check if operation failed

Full TypeScript definitions are included for excellent IDE support and type safety.

## Usage

### Loading the WASM Module

```typescript
import { loadPKHeX, isError, isSuccess } from 'pkhex';

// Initialize the WASM module
const api = await loadPKHeX();
```

### Loading a Save File

```typescript
// Read save file as ArrayBuffer
const fileInput = document.getElementById('saveFile') as HTMLInputElement;
const file = fileInput.files[0];
const arrayBuffer = await file.arrayBuffer();

// Convert to base64
const uint8Array = new Uint8Array(arrayBuffer);
const base64Data = btoa(String.fromCharCode(...uint8Array));

// Load the save file - returns object directly, no JSON parsing needed!
const result = api.LoadSave(base64Data);

if (isError(result)) {
  console.error('Failed to load:', result.error);
  return;
}

const handle = result.handle;
console.log('Save loaded with handle:', handle);
```

### Getting Save File Information

```typescript
const result = api.GetSaveInfo(handle);

if (isSuccess(result)) {
  console.log('Generation:', result.generation);
  console.log('Game Version:', result.gameVersion);
  console.log('Trainer:', result.ot);
  console.log('TID:', result.tid);
  console.log('Boxes:', result.boxCount);
}
```

### Modifying Trainer Data

```typescript
// Get current trainer info
const trainerResult = api.GetTrainerInfo(handle);

if (isError(trainerResult)) {
  console.error('Error:', trainerResult.error);
  return;
}

// Modify trainer data
const modifiedTrainer = {
  ot: trainerResult.ot,
  money: 999999,
  playedHours: 100,
  playedMinutes: trainerResult.playedMinutes,
  playedSeconds: trainerResult.playedSeconds
};

// Save changes
const setResult = api.SetTrainerInfo(handle, JSON.stringify(modifiedTrainer));

if (isSuccess(setResult)) {
  console.log('Trainer updated:', setResult.message);
}
```

### Modifying Pokemon

```typescript
// Get Pokemon from box 0, slot 0
const pokemon = api.GetPokemon(handle, 0, 0);

if (isError(pokemon)) {
  console.error('Error:', pokemon.error);
  return;
}

console.log('Pokemon:', pokemon.speciesName);
console.log('Level:', pokemon.level);

// Modify the Pokemon
const modifications = {
  level: 100,
  isShiny: true,
  ivs: [31, 31, 31, 31, 31, 31], // Max IVs
  evs: [252, 252, 4, 0, 0, 0]    // Attack/HP focused
};

const modifyResult = api.ModifyPokemon(handle, 0, 0, JSON.stringify(modifications));

if (isSuccess(modifyResult)) {
  console.log('Pokemon modified:', modifyResult.message);
}
```

### Importing from Showdown Format

```typescript
const showdownText = `
Charizard @ Life Orb
Ability: Solar Power
Level: 100
Shiny: Yes
EVs: 252 SpA / 4 SpD / 252 Spe
Timid Nature
IVs: 0 Atk
- Flamethrower
- Solar Beam
- Air Slash
- Roost
`.trim();

const importResult = api.ImportShowdown(handle, 1, 0, showdownText);

if (isSuccess(importResult)) {
  console.log('Imported:', importResult.message);
}
```

### Checking Pokemon Legality

```typescript
const legalityResult = api.CheckLegality(handle, 0, 0);

if (isSuccess(legalityResult)) {
  if (legalityResult.valid) {
    console.log('Pokemon is legal!');
  } else {
    console.log('Legality issues:');
    legalityResult.errors.forEach(error => console.log('  -', error));
  }
}
```

### Exporting Modified Save

```typescript
const exportResult = api.ExportSave(handle);

if (isError(exportResult)) {
  console.error('Export failed:', exportResult.error);
  return;
}

// Convert base64 to ArrayBuffer
const binaryString = atob(exportResult.base64Data);
const bytes = new Uint8Array(binaryString.length);
for (let i = 0; i < binaryString.length; i++) {
  bytes[i] = binaryString.charCodeAt(i);
}

// Download the file
const blob = new Blob([bytes], { type: 'application/octet-stream' });
const url = URL.createObjectURL(blob);
const a = document.createElement('a');
a.href = url;
a.download = 'modified-save.sav';
a.click();
URL.revokeObjectURL(url);
```

### Proper Session Management

**IMPORTANT**: Always dispose of sessions when done to free memory.

```typescript
let handle = null;

try {
  // Load save file
  const loadResult = api.LoadSave(base64Data);
  
  if (isError(loadResult)) {
    throw new Error(loadResult.error);
  }
  
  handle = loadResult.handle;
  
  // Perform operations
  await modifyTrainerInfo(api, handle);
  await modifyPokemon(api, handle);
  
  // Export modified save
  const exportResult = api.ExportSave(handle);
  
  if (isSuccess(exportResult)) {
    // ... handle export data
  }
  
} catch (error) {
  console.error('Error:', error);
} finally {
  // Always dispose the session
  if (handle) {
    api.DisposeSave(handle);
    console.log('Session disposed');
  }
}
```

## Supported Save Formats

The library supports save files from all Pokemon generations:

### Generation 1 (Red/Blue/Yellow)
- `.sav` - Game Boy save files
- Stadium save files

### Generation 2 (Gold/Silver/Crystal)
- `.sav` - Game Boy Color save files
- Stadium 2 save files

### Generation 3 (Ruby/Sapphire/Emerald/FireRed/LeafGreen)
- `.sav` - Game Boy Advance save files
- Colosseum/XD save files

### Generation 4 (Diamond/Pearl/Platinum/HeartGold/SoulSilver)
- `.sav` - Nintendo DS save files
- Battle Revolution save files

### Generation 5 (Black/White/Black 2/White 2)
- `.sav` - Nintendo DS save files

### Generation 6 (X/Y/Omega Ruby/Alpha Sapphire)
- `.sav` - Nintendo 3DS save files (decrypted)

### Generation 7 (Sun/Moon/Ultra Sun/Ultra Moon/Let's Go)
- `.sav` - Nintendo 3DS save files (decrypted)
- `.bin` - Let's Go save files

### Generation 8 (Sword/Shield/Brilliant Diamond/Shining Pearl/Legends Arceus)
- `.sav` - Nintendo Switch save files (decrypted)

### Generation 9 (Scarlet/Violet)
- `.sav` - Nintendo Switch save files (decrypted)

**Note**: Console save files must be decrypted before use. Use tools like Checkpoint (3DS) or JKSV (Switch) to extract decrypted saves.

## API Reference

### Save File Management

#### `LoadSave(base64Data: string): string`
Load a save file from base64-encoded data. Returns a session ID for subsequent operations.

**Parameters:**
- `base64Data`: Base64-encoded save file data

**Returns:** JSON string with `{ sessionId: string }`

#### `GetSaveInfo(sessionId: string): string`
Get information about a loaded save file.

**Returns:** JSON string with save file metadata (generation, version, trainer name, etc.)

#### `ExportSave(sessionId: string): string`
Export the modified save file as base64-encoded data.

**Returns:** JSON string with `{ base64Data: string }`

#### `DisposeSave(sessionId: string): string`
Dispose a session and free memory. Always call this when done with a save file.

**Returns:** JSON string with success message

### Pokemon Operations

#### `GetAllPokemon(sessionId: string): string`
Get all non-empty Pokemon from boxes.

**Returns:** JSON array of Pokemon summaries

#### `GetParty(sessionId: string): string`
Get party Pokemon (up to 6).

**Returns:** JSON array of Pokemon summaries

#### `GetPokemon(sessionId: string, box: number, slot: number): string`
Get detailed Pokemon data from a box slot.

**Parameters:**
- `box`: Box number (0-based)
- `slot`: Slot number (0-based, 0-29 for most games)

**Returns:** JSON object with detailed Pokemon data

#### `ModifyPokemon(sessionId: string, box: number, slot: number, modificationsJson: string): string`
Modify a Pokemon's attributes.

**Parameters:**
- `modificationsJson`: JSON string of modifications (species, level, IVs, EVs, etc.)

**Returns:** JSON string with success message

#### `DeletePokemon(sessionId: string, box: number, slot: number): string`
Delete a Pokemon from a box slot.

#### `MovePokemon(sessionId: string, fromBox: number, fromSlot: number, toBox: number, toSlot: number): string`
Move or swap Pokemon between slots.

### Legality and Validation

#### `CheckLegality(sessionId: string, box: number, slot: number): string`
Check if a Pokemon is legal according to game rules.

**Returns:** JSON object with `{ valid: boolean, errors: string[], parsed: string }`

#### `LegalizePokemon(sessionId: string, box: number, slot: number): string`
Apply basic legalization (heal HP/PP, refresh checksum).

### Showdown Format

#### `ExportShowdown(sessionId: string, box: number, slot: number): string`
Export a Pokemon in Showdown battle simulator format.

**Returns:** JSON object with `{ showdownText: string }`

#### `ImportShowdown(sessionId: string, box: number, slot: number, showdownText: string): string`
Import a Pokemon from Showdown format text.

### Trainer Data

#### `GetTrainerInfo(sessionId: string): string`
Get trainer information (name, IDs, money, play time).

#### `SetTrainerInfo(sessionId: string, trainerDataJson: string): string`
Update trainer information.

#### `GetTrainerCard(sessionId: string): string`
Get trainer card data.

#### `GetTrainerAppearance(sessionId: string): string`
Get trainer appearance customization (Gen 6+).

#### `SetTrainerAppearance(sessionId: string, appearanceJson: string): string`
Update trainer appearance.

### Inventory Management

#### `GetPouchItems(sessionId: string): string`
Get all items organized by pouch type.

**Returns:** JSON array of pouches with items

#### `AddItemToPouch(sessionId: string, itemId: number, count: number, pouchIndex: number): string`
Add an item to inventory.

#### `RemoveItemFromPouch(sessionId: string, itemId: number, count: number): string`
Remove an item from inventory.

### Pokedex

#### `GetPokedex(sessionId: string): string`
Get Pokedex completion data.

**Returns:** JSON array of species with seen/caught status

#### `SetPokedexSeen(sessionId: string, species: number, form: number): string`
Mark a species as seen.

#### `SetPokedexCaught(sessionId: string, species: number, form: number): string`
Mark a species as caught.

### Progress and Statistics

#### `GetBadges(sessionId: string): string`
Get badge data.

#### `SetBadge(sessionId: string, badgeIndex: number, value: boolean): string`
Set badge status.

#### `GetEventFlag(sessionId: string, flagIndex: number): string`
Get an event flag value.

#### `SetEventFlag(sessionId: string, flagIndex: number, value: boolean): string`
Set an event flag value.

#### `GetHallOfFame(sessionId: string): string`
Get Hall of Fame entries.

For a complete API reference, see the [TypeScript definitions](src/index.d.ts).

## Error Handling

All API methods return JSON strings. Successful responses have a `data` property, while errors have an `error` property:

```typescript
// Success response
{
  "data": { /* result data */ }
}

// Error response
{
  "error": "Error message",
  "code": "ERROR_CODE"
}
```

Use the `isError` and `isSuccess` type guards for clean error handling:

```typescript
import { isError, isSuccess, unwrap } from 'pkhex';

// Option 1: Type guards
const result = api.GetPokemon(handle, 0, 0);

if (isError(result)) {
  console.error('Failed:', result.error);
  return;
}

// TypeScript knows result is successful here
console.log('Pokemon:', result.speciesName);

// Option 2: Unwrap helper (throws on error)
try {
  const pokemon = unwrap(api.GetPokemon(handle, 0, 0));
  console.log('Pokemon:', pokemon.speciesName);
} catch (error) {
  console.error('Failed:', error.message);
}
```

## Documentation

- **[Usage Examples](USAGE_EXAMPLES.md)** - Comprehensive examples for all features
- **[API Migration Guide](API_MIGRATION_GUIDE.md)** - Migrating from older JSON string API
- **[API Reference](docs/index.html)** - Full TypeScript API documentation (generated with TypeDoc)
- **[Type Definitions](src/index.d.ts)** - Complete TypeScript definitions

## Examples

See the [USAGE_EXAMPLES.md](USAGE_EXAMPLES.md) for complete usage examples including:
- Loading and saving files
- Modifying trainer data
- Editing Pokemon
- Importing from Showdown format
- Checking legality
- Proper session management
- React integration

## Development

### Project Structure

```
pkhex-wasm/
├── src/
│   ├── PKHeX/           # C# WASM library source
│   │   ├── Api/             # JSExport API layer
│   │   ├── Core/            # Session management
│   │   ├── Models/          # Data models
│   │   ├── Services/        # Business logic (not implemented)
│   │   └── Helpers/         # Utility functions
│   ├── index.d.ts           # TypeScript definitions
│   └── example.ts           # Usage examples
├── lib/
│   └── PKHeX/               # PKHeX.Core submodule
├── dist/                    # Build output
└── README.md
```

### Building from Source

1. Ensure prerequisites are installed
2. Clone with submodules: `git clone --recursive`
3. Build: `dotnet publish src/PKHeX/PKHeX.csproj -c Release`
4. Output will be in `dist/` directory

### Testing

The library includes comprehensive error handling and validation. Test with various save files to ensure compatibility.

## Limitations

- Save files must be decrypted (use Checkpoint for 3DS, JKSV for Switch)
- Some generation-specific features may not be available for all save types
- Large save files (Gen 8+) may take longer to load
- Browser memory limits apply to concurrent sessions

## Contributing

Contributions are welcome! Please:
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Submit a pull request

## License

This project wraps PKHeX.Core, which is licensed under the GPLv3. This wrapper library is also licensed under GPLv3.

## Credits

- [PKHeX](https://github.com/kwsch/PKHeX) by Kurt (Kaphotics) - The core save editing library
- .NET Team - WebAssembly support and runtime

## Support

For issues and questions:
- Open an issue on GitHub
- Check existing issues for solutions
- Refer to PKHeX documentation for save file specifics

## Changelog

### Version 1.0.0 (Initial Release)
- Complete save file support for Gen 1-9
- Full Pokemon editing capabilities
- Trainer data management
- Inventory and Pokedex editing
- Legality checking
- Showdown format import/export
- TypeScript definitions
- Comprehensive documentation
