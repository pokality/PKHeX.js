using System.Text.Json.Serialization;

namespace PKHeX.Models;

public record TeraTypeData(
    [property: JsonPropertyName("teraType")] int TeraType,
    [property: JsonPropertyName("teraTypeName")] string TeraTypeName,
    [property: JsonPropertyName("teraTypeOverride")] int TeraTypeOverride,
    [property: JsonPropertyName("teraTypeOverrideName")] string TeraTypeOverrideName,
    [property: JsonPropertyName("isOverridden")] bool IsOverridden
);

public record TeraTypeInfo(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("isStellar")] bool IsStellar
);

public record TeraTypesListResponse(
    [property: JsonPropertyName("teraTypes")] List<TeraTypeInfo> TeraTypes
);
