using System.Text.Json.Serialization;

namespace PKHeX.Models;

public record PokemonStats(
    [property: JsonPropertyName("hp")] int HP,
    [property: JsonPropertyName("attack")] int Attack,
    [property: JsonPropertyName("defense")] int Defense,
    [property: JsonPropertyName("spAttack")] int SpAttack,
    [property: JsonPropertyName("spDefense")] int SpDefense,
    [property: JsonPropertyName("speed")] int Speed
);

public record HiddenPowerInfo(
    [property: JsonPropertyName("type")] int Type,
    [property: JsonPropertyName("typeName")] string TypeName,
    [property: JsonPropertyName("power")] int Power
);

public record CharacteristicInfo(
    [property: JsonPropertyName("index")] int Index,
    [property: JsonPropertyName("text")] string Text
);

public record EvolutionEntry(
    [property: JsonPropertyName("species")] int Species,
    [property: JsonPropertyName("speciesName")] string SpeciesName,
    [property: JsonPropertyName("form")] byte Form
);

public record EvolutionInfo(
    [property: JsonPropertyName("species")] int Species,
    [property: JsonPropertyName("speciesName")] string SpeciesName,
    [property: JsonPropertyName("generation")] int Generation,
    [property: JsonPropertyName("evolutionChain")] List<EvolutionEntry> EvolutionChain,
    [property: JsonPropertyName("chainLength")] int ChainLength,
    [property: JsonPropertyName("forwardEvolutions")] List<EvolutionEntry> ForwardEvolutions,
    [property: JsonPropertyName("preEvolutions")] List<EvolutionEntry> PreEvolutions,
    [property: JsonPropertyName("baseSpecies")] int BaseSpecies,
    [property: JsonPropertyName("baseSpeciesName")] string BaseSpeciesName,
    [property: JsonPropertyName("baseForm")] byte BaseForm
);

public record SpeciesFormInfo(
    [property: JsonPropertyName("formIndex")] int FormIndex,
    [property: JsonPropertyName("formName")] string FormName,
    [property: JsonPropertyName("type1")] int Type1,
    [property: JsonPropertyName("type1Name")] string Type1Name,
    [property: JsonPropertyName("type2")] int Type2,
    [property: JsonPropertyName("type2Name")] string Type2Name,
    [property: JsonPropertyName("baseStats")] PokemonStats BaseStats,
    [property: JsonPropertyName("genderRatio")] int GenderRatio,
    [property: JsonPropertyName("isDualGender")] bool IsDualGender,
    [property: JsonPropertyName("isGenderless")] bool IsGenderless
);

public record SpeciesFormsInfo(
    [property: JsonPropertyName("species")] int Species,
    [property: JsonPropertyName("speciesName")] string SpeciesName,
    [property: JsonPropertyName("generation")] int Generation,
    [property: JsonPropertyName("forms")] List<SpeciesFormInfo> Forms,
    [property: JsonPropertyName("formCount")] int FormCount
);

public record ConversionResult(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("base64Data")] string Base64Data,
    [property: JsonPropertyName("fromGeneration")] int FromGeneration,
    [property: JsonPropertyName("toGeneration")] int ToGeneration,
    [property: JsonPropertyName("fromFormat")] string FromFormat,
    [property: JsonPropertyName("toFormat")] string ToFormat,
    [property: JsonPropertyName("conversionResult")] string ConversionResultText
);

public record LocationInfo(
    [property: JsonPropertyName("value")] int Value,
    [property: JsonPropertyName("text")] string Text
);

public record MetLocationsInfo(
    [property: JsonPropertyName("generation")] int Generation,
    [property: JsonPropertyName("gameVersion")] int GameVersion,
    [property: JsonPropertyName("isEggLocations")] bool IsEggLocations,
    [property: JsonPropertyName("locations")] List<LocationInfo> Locations,
    [property: JsonPropertyName("count")] int Count
);
