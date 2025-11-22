/**
 * Example: Pokemon Modifications with Type Safety
 * 
 * This example shows how to use the typed PokemonModifications interface
 * instead of raw JSON strings for better developer experience.
 */

import setupPKHeX, { PokemonModifications } from '../dist/index.js';

async function demonstrateTypedModifications() {
  const api = await setupPKHeX();

  // ✅ GOOD: Type-safe modifications with IntelliSense support
  const modifications: PokemonModifications = {
    species: 25,        // Pikachu - IDE shows this should be SpeciesID (number)
    nickname: 'Sparky', // IDE shows max length varies by generation
    level: 50,          // IDE shows valid range: 1-100
    nature: 5,          // Modest - IDE shows this should be 0-24
    ability: 9,         // Static
    heldItem: 229,      // Leftovers
    moves: [85, 87, 129, 113], // IDE shows max 4 moves
    ivs: [31, 0, 31, 31, 31, 31], // IDE shows format: [HP, Atk, Def, SpA, SpD, Spe] (0-31)
    evs: [4, 0, 0, 252, 252, 0],  // IDE shows max 252 each, 510 total
    gender: 0,          // Male - IDE shows 0=Male, 1=Female, 2=Genderless
    isShiny: true,      // IDE shows this is boolean
    ot_Name: 'Ash',     // Original Trainer name
    ball: 4,            // Great Ball
  };

  // Type-safe API calls
  const result1 = api.pkm.modify('base64data==', 3, modifications);
  const result2 = api.save.pokemon.modify(1, 0, 0, modifications);

  // ❌ BAD: Old way with raw JSON strings (no type safety, no IntelliSense)
  // const result = api.pkm.modify('base64data==', 3, '{"level":50,"species":25}');
  // ^ No IDE support, easy to make typos, no validation

  // ✅ GOOD: Partial modifications (all fields optional)
  const levelOnly: PokemonModifications = {
    level: 100, // Only change level, leave everything else unchanged
  };

  // ✅ GOOD: Complex modifications with full type checking
  const competitivePokemon: PokemonModifications = {
    species: 150,       // Mewtwo
    level: 50,          // VGC level
    nature: 5,          // Modest (+SpA, -Atk)
    ability: 24,        // Pressure
    heldItem: 229,      // Leftovers
    moves: [94, 105, 129, 347], // Psychic, Recover, Swift, Calm Mind
    ivs: [31, 0, 31, 31, 31, 31], // Perfect except Attack (not needed)
    evs: [252, 0, 4, 252, 0, 0],  // 252 HP / 4 Def / 252 SpA
    gender: 2,          // Genderless
    isShiny: false,     // Not shiny
    ball: 1,            // Master Ball
  };

  console.log('✅ Type-safe Pokemon modifications completed!');
  console.log('Benefits:');
  console.log('- IntelliSense support for all fields');
  console.log('- Type checking prevents errors');
  console.log('- Documentation in IDE tooltips');
  console.log('- No need to remember JSON field names');
  console.log('- Compile-time validation');
}

// Example of what developers see in their IDE:
/*
When typing `modifications.`, IDE shows:
- species?: SpeciesID          // National Pokedex number (1-1025)
- nickname?: string            // Pokemon nickname (max length varies by generation)
- level?: number               // Pokemon level (1-100)
- nature?: number              // Nature ID (0-24)
- ability?: number             // Ability ID
- heldItem?: number            // Held item ID
- moves?: number[]             // Array of move IDs (max 4 moves)
- ivs?: number[]               // Individual Values [HP, Atk, Def, SpA, SpD, Spe] (0-31)
- evs?: number[]               // Effort Values [HP, Atk, Def, SpA, SpD, Spe] (0-252 each, max 510 total)
- gender?: number              // Gender (0=Male, 1=Female, 2=Genderless)
- isShiny?: boolean            // Whether Pokemon should be shiny
- ot_Name?: string             // Original Trainer name
- ball?: number                // Pokeball ID
*/

export { demonstrateTypedModifications };