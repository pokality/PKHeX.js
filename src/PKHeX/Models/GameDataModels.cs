using System.Text.Json.Serialization;

namespace PKHeX.Models;

public record SpeciesCategoryResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("species")] int Species,
    [property: JsonPropertyName("speciesName")] string SpeciesName,
    [property: JsonPropertyName("isLegendary")] bool IsLegendary,
    [property: JsonPropertyName("isSubLegendary")] bool IsSubLegendary,
    [property: JsonPropertyName("isMythical")] bool IsMythical,
    [property: JsonPropertyName("isUltraBeast")] bool IsUltraBeast,
    [property: JsonPropertyName("isParadox")] bool IsParadox,
    [property: JsonPropertyName("isSpecial")] bool IsSpecial
);

public record PrimalFormResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("species")] int Species,
    [property: JsonPropertyName("speciesName")] string SpeciesName,
    [property: JsonPropertyName("form")] int Form,
    [property: JsonPropertyName("isPrimal")] bool IsPrimal
);

public record SaveRevisionResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("revision")] int Revision,
    [property: JsonPropertyName("revisionName")] string RevisionName
);

public record TmsCollectedResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("tmsCollected")] int TmsCollected,
    [property: JsonPropertyName("message")] string Message
);

public record SurveyPointsResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("points")] uint Points
);

public record DonutEntry(
    [property: JsonPropertyName("index")] int Index,
    [property: JsonPropertyName("donutId")] ushort DonutId,
    [property: JsonPropertyName("calories")] ushort Calories,
    [property: JsonPropertyName("stars")] byte Stars,
    [property: JsonPropertyName("levelBoost")] byte LevelBoost,
    [property: JsonPropertyName("berryName")] ushort BerryName
);

public record DonutPocketResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("donuts")] List<DonutEntry> Donuts,
    [property: JsonPropertyName("count")] int Count,
    [property: JsonPropertyName("maxCount")] int MaxCount
);

public record DonutsShinyResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("count")] int Count,
    [property: JsonPropertyName("message")] string Message
);

public record PlayerAppearance9aResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("skinColor")] uint SkinColor,
    [property: JsonPropertyName("lipColor")] uint LipColor,
    [property: JsonPropertyName("darkCircles")] uint DarkCircles,
    [property: JsonPropertyName("eyeColor")] uint EyeColor,
    [property: JsonPropertyName("eyebrowColor")] uint EyebrowColor,
    [property: JsonPropertyName("eyebrowShape")] uint EyebrowShape,
    [property: JsonPropertyName("eyelashColor")] uint EyelashColor,
    [property: JsonPropertyName("eyelashShape")] uint EyelashShape,
    [property: JsonPropertyName("beautySpotFirst")] uint BeautySpotFirst,
    [property: JsonPropertyName("beautySpotSecond")] uint BeautySpotSecond,
    [property: JsonPropertyName("freckles")] uint Freckles,
    [property: JsonPropertyName("hairColor")] uint HairColor,
    [property: JsonPropertyName("colorBlocking")] uint ColorBlocking,
    [property: JsonPropertyName("balayageFadeFirst")] uint BalayageFadeFirst,
    [property: JsonPropertyName("balayageFadeSecond")] uint BalayageFadeSecond,
    [property: JsonPropertyName("faceShape")] uint FaceShape,
    [property: JsonPropertyName("bangs")] uint Bangs,
    [property: JsonPropertyName("hairColorMode")] uint HairColorMode
);
