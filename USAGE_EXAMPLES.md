# PKHeX WASM Usage Examples

## Quick Start

```typescript
import { loadPKHeX, isError, isSuccess } from 'pkhex';

// Load the WASM module
const api = await loadPKHeX();

// Load a save file
const saveData = await fetch('pokemon.sav').then(r => r.arrayBuffer());
const base64 = btoa(String.fromCharCode(...new Uint8Array(saveData)));

const loadResult = api.LoadSave(base64);
if (isError(loadResult)) {
  console.error('Failed to load:', loadResult.error);
  return;
}

const handle = loadResult.handle;

// Get save information
const info = api.GetSaveInfo(handle);
if (isSuccess(info)) {
  console.log(`Game: ${info.gameVersion}`);
  console.log(`Trainer: ${info.ot}`);
  console.log(`Boxes: ${info.boxCount}`);
}

// Clean up
api.DisposeSave(handle);
```

## Working with Pokemon

### Get All Pokemon
```typescript
const result = api.GetAllPokemon(handle);

if (isSuccess(result)) {
  result.pokemon.forEach(pk => {
    console.log(`${pk.speciesName} (Lv${pk.level}) - Box ${pk.box}, Slot ${pk.slot}`);
  });
}
```

### Get Pokemon Details
```typescript
const pokemon = api.GetPokemon(handle, 0, 0); // Box 0, Slot 0

if (isSuccess(pokemon)) {
  console.log('Species:', pokemon.speciesName);
  console.log('Nickname:', pokemon.nickname);
  console.log('Level:', pokemon.level);
  console.log('Nature:', pokemon.natureName);
  console.log('Ability:', pokemon.abilityName);
  console.log('IVs:', pokemon.ivs);
  console.log('EVs:', pokemon.evs);
  console.log('Shiny:', pokemon.isShiny);
}
```

### Modify Pokemon
```typescript
const modifications = {
  level: 100,
  isShiny: true,
  ivs: [31, 31, 31, 31, 31, 31],
  evs: [252, 0, 0, 252, 4, 0]
};

const result = api.ModifyPokemon(handle, 0, 0, JSON.stringify(modifications));

if (isSuccess(result)) {
  console.log('Modified successfully!');
}
```

### Check Legality
```typescript
const legality = api.CheckLegality(handle, 0, 0);

if (isSuccess(legality)) {
  if (legality.valid) {
    console.log('Pokemon is legal!');
  } else {
    console.log('Legality errors:', legality.errors);
  }
}
```

### Showdown Import/Export
```typescript
// Export to Showdown format
const exportResult = api.ExportShowdown(handle, 0, 0);
if (isSuccess(exportResult)) {
  console.log(exportResult.showdownText);
}

// Import from Showdown format
const showdownText = `
Pikachu @ Light Ball
Ability: Static
Level: 50
EVs: 252 Atk / 4 SpD / 252 Spe
Jolly Nature
- Volt Tackle
- Iron Tail
- Quick Attack
- Thunder Wave
`;

const importResult = api.ImportShowdown(handle, 0, 1, showdownText);
if (isSuccess(importResult)) {
  console.log('Imported successfully!');
}
```

## Trainer Data

### Get Trainer Info
```typescript
const trainer = api.GetTrainerInfo(handle);

if (isSuccess(trainer)) {
  console.log('Name:', trainer.ot);
  console.log('TID:', trainer.tid);
  console.log('SID:', trainer.sid);
  console.log('Money:', trainer.money);
  console.log('Play Time:', `${trainer.playedHours}h ${trainer.playedMinutes}m`);
}
```

### Modify Trainer Info
```typescript
const trainerData = {
  ot: "ASH",
  money: 999999,
  playedHours: 100,
  playedMinutes: 30,
  playedSeconds: 45
};

const result = api.SetTrainerInfo(handle, JSON.stringify(trainerData));
if (isSuccess(result)) {
  console.log('Trainer updated!');
}
```

## Inventory Management

### Get Items
```typescript
const pouches = api.GetPouchItems(handle);

if (isSuccess(pouches)) {
  pouches.pouches.forEach(pouch => {
    console.log(`\n${pouch.pouchType}:`);
    pouch.items.forEach(item => {
      console.log(`  ${item.itemName} x${item.count}`);
    });
  });
}
```

### Add Items
```typescript
const result = api.AddItemToPouch(handle, 1, 99, 0); // Item ID 1, count 99, pouch 0

if (isSuccess(result)) {
  console.log('Item added!');
}
```

## Pokedex

### Get Pokedex Status
```typescript
const pokedex = api.GetPokedex(handle);

if (isSuccess(pokedex)) {
  const caught = pokedex.pokedex.filter(e => e.caught).length;
  const seen = pokedex.pokedex.filter(e => e.seen).length;
  
  console.log(`Caught: ${caught}`);
  console.log(`Seen: ${seen}`);
}
```

### Mark as Caught
```typescript
const result = api.SetPokedexCaught(handle, 25, 0); // Pikachu, form 0

if (isSuccess(result)) {
  console.log('Marked as caught!');
}
```

## Game Progress

### Get Badges
```typescript
const badges = api.GetBadges(handle);

if (isSuccess(badges)) {
  console.log(`Badges: ${badges.badgeCount}/8`);
  badges.badges.forEach((has, i) => {
    console.log(`Badge ${i + 1}: ${has ? '✓' : '✗'}`);
  });
}
```

### Set Badge
```typescript
const result = api.SetBadge(handle, 0, true); // Badge 0, obtained

if (isSuccess(result)) {
  console.log('Badge obtained!');
}
```

## Error Handling

### Using Type Guards
```typescript
const result = api.GetSaveInfo(handle);

if (isError(result)) {
  console.error(`Error [${result.code}]: ${result.error}`);
  return;
}

// TypeScript knows result is successful here
console.log(result.gameVersion);
```

### Using Unwrap Helper
```typescript
import { unwrap } from 'pkhex/helpers';

try {
  const info = unwrap(api.GetSaveInfo(handle));
  console.log(info.gameVersion);
} catch (error) {
  console.error('Failed:', error.message);
}
```

## Complete Example

```typescript
import { loadPKHeX, isError, isSuccess } from 'pkhex';

async function editSave(saveFile: File) {
  // Load WASM
  const api = await loadPKHeX();
  
  // Read file
  const buffer = await saveFile.arrayBuffer();
  const base64 = btoa(String.fromCharCode(...new Uint8Array(buffer)));
  
  // Load save
  const loadResult = api.LoadSave(base64);
  if (isError(loadResult)) {
    throw new Error(loadResult.error);
  }
  
  const handle = loadResult.handle;
  
  try {
    // Get first Pokemon
    const pokemon = api.GetPokemon(handle, 0, 0);
    if (isError(pokemon)) {
      throw new Error(pokemon.error);
    }
    
    console.log('Editing:', pokemon.speciesName);
    
    // Make it shiny and level 100
    const mods = { level: 100, isShiny: true };
    const modResult = api.ModifyPokemon(handle, 0, 0, JSON.stringify(mods));
    
    if (isError(modResult)) {
      throw new Error(modResult.error);
    }
    
    // Export modified save
    const exportResult = api.ExportSave(handle);
    if (isError(exportResult)) {
      throw new Error(exportResult.error);
    }
    
    // Convert back to binary
    const binaryString = atob(exportResult.base64Data);
    const bytes = new Uint8Array(binaryString.length);
    for (let i = 0; i < binaryString.length; i++) {
      bytes[i] = binaryString.charCodeAt(i);
    }
    
    // Download
    const blob = new Blob([bytes], { type: 'application/octet-stream' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'modified.sav';
    a.click();
    
  } finally {
    // Always clean up
    api.DisposeSave(handle);
  }
}
```

## React Example

```typescript
import { useState, useEffect } from 'react';
import { loadPKHeX, type PKHeXApi, isSuccess } from 'pkhex';

function usePKHeX() {
  const [api, setApi] = useState<PKHeXApi | null>(null);
  const [loading, setLoading] = useState(true);
  
  useEffect(() => {
    loadPKHeX().then(api => {
      setApi(api);
      setLoading(false);
    });
  }, []);
  
  return { api, loading };
}

function SaveEditor() {
  const { api, loading } = usePKHeX();
  const [handle, setHandle] = useState<number | null>(null);
  const [pokemon, setPokemon] = useState<any[]>([]);
  
  const loadSave = async (file: File) => {
    if (!api) return;
    
    const buffer = await file.arrayBuffer();
    const base64 = btoa(String.fromCharCode(...new Uint8Array(buffer)));
    
    const result = api.LoadSave(base64);
    if (isSuccess(result)) {
      setHandle(result.handle);
      
      const pkResult = api.GetAllPokemon(result.handle);
      if (isSuccess(pkResult)) {
        setPokemon(pkResult.pokemon);
      }
    }
  };
  
  useEffect(() => {
    return () => {
      if (api && handle) {
        api.DisposeSave(handle);
      }
    };
  }, [api, handle]);
  
  if (loading) return <div>Loading PKHeX...</div>;
  
  return (
    <div>
      <input type="file" onChange={e => e.target.files?.[0] && loadSave(e.target.files[0])} />
      <ul>
        {pokemon.map((pk, i) => (
          <li key={i}>{pk.speciesName} Lv{pk.level}</li>
        ))}
      </ul>
    </div>
  );
}
```

## See Also

- [API Migration Guide](./API_MIGRATION_GUIDE.md) - Migrating from the old JSON string API
- [TypeScript Definitions](./src/index.d.ts) - Full API reference
- [PKHeX.Core Documentation](https://github.com/kwsch/PKHeX) - Underlying library
