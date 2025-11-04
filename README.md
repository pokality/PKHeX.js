# PKHeX.js

Provides WebAssembly bindings for PKHeX.Core. Allows editing Pokémon save files in JavaScript environments (Browser, NodeJS, etc). If you find a feature of PKHeX is
missing, feel free to send a PR or open an issue.

## Installation

You can install the latest version of PKHeX like this:

```bash
npm install pkhex
```

## Usage

```typescript
// Get PKHeX API
import setupPKHeX from 'pkhex';
const pkhex = await setupPKHeX();

// Load a save file
const saveData = await fetch('save.sav').then(r => r.arrayBuffer());
const base64Data = btoa(String.fromCharCode(...new Uint8Array(saveData)));
const result = await pkhex.loadSave(base64Data);

if (result.success) {
  const { handle } = result;

  // Get save info
  const { ot, gameVersion } = await PKHeXApi.getSaveInfo(handle);
  console.log(`Trainer: ${ot}, Game: ${gameVersion}`);
  
  // Get all Pokémon
  const mons = await PKHeXApi.getAllPokemon(handle);
  console.log(`Found ${mons.length} Pokémon`);
  
  // Export modified save
  const { base64Data } = await PKHeXApi.exportSave(handle);
  const saveBytes = Uint8Array.from(atob(base64Data), c => c.charCodeAt(0));
  
  // Free memory after you're done
  await PKHeXApi.disposeSave(handle);
}
```

## API Documentation

API documentation is built autoamtically [through GitHub pages](https://monokrome.github.io/PKHeX.js).

## Features

- Load and save Pokémon save files (Generations 1-9)
- Read and modify Pokémon data
- Edit trainer information
- Manage items and Pokédex
- Legality checking
- Showdown format import/export

## Updating PKHeX

The original PKHeX.Core is compiled into WASM from a git submodule at
lib/PKHeX. To create a PR for updating (or downgrading) PKHeX, you can
change the version of the submodule to a version tag, change package.json
for PKHeX.js to match that version, commit the changes, test a build
locally with scripts/build.sh, and create a PR.

## License

This project uses PKHeX.Core which is licensed under GPLv3.

## Credits

Built on top of [PKHeX](https://github.com/kwsch/PKHeX) by Kurt.
