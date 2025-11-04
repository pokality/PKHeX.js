using System.Text.Json.Serialization;

namespace PKHeX.Models;

public record PokemonSummary(
    [property: JsonPropertyName("box")] int Box,
    [property: JsonPropertyName("slot")] int Slot,
    [property: JsonPropertyName("species")] int Species,
    [property: JsonPropertyName("speciesName")] string SpeciesName,
    [property: JsonPropertyName("level")] int Level,
    [property: JsonPropertyName("isEgg")] bool IsEgg,
    [property: JsonPropertyName("isShiny")] bool IsShiny
);

public record PokemonDetail(
    [property: JsonPropertyName("species")] int Species,
    [property: JsonPropertyName("speciesName")] string SpeciesName,
    [property: JsonPropertyName("nickname")] string Nickname,
    [property: JsonPropertyName("level")] int Level,
    [property: JsonPropertyName("nature")] int Nature,
    [property: JsonPropertyName("natureName")] string NatureName,
    [property: JsonPropertyName("ability")] int Ability,
    [property: JsonPropertyName("abilityName")] string AbilityName,
    [property: JsonPropertyName("heldItem")] int HeldItem,
    [property: JsonPropertyName("heldItemName")] string HeldItemName,
    [property: JsonPropertyName("moves")] int[] Moves,
    [property: JsonPropertyName("moveNames")] string[] MoveNames,
    [property: JsonPropertyName("ivs")] int[] IVs,
    [property: JsonPropertyName("evs")] int[] EVs,
    [property: JsonPropertyName("stats")] int[] Stats,
    [property: JsonPropertyName("gender")] int Gender,
    [property: JsonPropertyName("isShiny")] bool IsShiny,
    [property: JsonPropertyName("isEgg")] bool IsEgg,
    [property: JsonPropertyName("ot_Name")] string OT_Name,
    [property: JsonPropertyName("ot_Gender")] int OT_Gender,
    [property: JsonPropertyName("pid")] uint PID,
    [property: JsonPropertyName("ball")] int Ball,
    [property: JsonPropertyName("metLevel")] int MetLevel,
    [property: JsonPropertyName("metLocation")] int MetLocation,
    [property: JsonPropertyName("metLocationName")] string MetLocationName
);

public record PokemonModifications(
    [property: JsonPropertyName("species")] int? Species,
    [property: JsonPropertyName("nickname")] string? Nickname,
    [property: JsonPropertyName("level")] int? Level,
    [property: JsonPropertyName("nature")] int? Nature,
    [property: JsonPropertyName("ability")] int? Ability,
    [property: JsonPropertyName("heldItem")] int? HeldItem,
    [property: JsonPropertyName("moves")] int[]? Moves,
    [property: JsonPropertyName("ivs")] int[]? IVs,
    [property: JsonPropertyName("evs")] int[]? EVs,
    [property: JsonPropertyName("gender")] int? Gender,
    [property: JsonPropertyName("isShiny")] bool? IsShiny,
    [property: JsonPropertyName("ot_Name")] string? OT_Name,
    [property: JsonPropertyName("ball")] int? Ball
);

public record LegalityResult(
    [property: JsonPropertyName("valid")] bool Valid,
    [property: JsonPropertyName("errors")] string[] Errors,
    [property: JsonPropertyName("parsed")] string Parsed
);

public record RibbonData(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("hasRibbon")] bool HasRibbon,
    [property: JsonPropertyName("ribbonCount")] byte RibbonCount,
    [property: JsonPropertyName("type")] string Type
);

public record ContestStats(
    [property: JsonPropertyName("cool")] byte Cool,
    [property: JsonPropertyName("beauty")] byte Beauty,
    [property: JsonPropertyName("cute")] byte Cute,
    [property: JsonPropertyName("smart")] byte Smart,
    [property: JsonPropertyName("tough")] byte Tough,
    [property: JsonPropertyName("sheen")] byte Sheen
);
