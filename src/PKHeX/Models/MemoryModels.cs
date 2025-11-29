using System.Text.Json.Serialization;

namespace PKHeX.Models;

public record MemoryInfo(
    [property: JsonPropertyName("memoryId")] int MemoryId,
    [property: JsonPropertyName("intensity")] int Intensity,
    [property: JsonPropertyName("feeling")] int Feeling,
    [property: JsonPropertyName("variable")] int Variable,
    [property: JsonPropertyName("memoryText")] string MemoryText
);

public record MemoriesData(
    [property: JsonPropertyName("originalTrainerMemory")] MemoryInfo OriginalTrainerMemory,
    [property: JsonPropertyName("handlingTrainerMemory")] MemoryInfo? HandlingTrainerMemory
);

public record MemoryStringsData(
    [property: JsonPropertyName("memories")] List<NamedEntity> Memories,
    [property: JsonPropertyName("feelings")] List<NamedEntity> Feelings,
    [property: JsonPropertyName("intensities")] List<NamedEntity> Intensities
);
