using System.Text.Json.Serialization;

namespace PKHeX.Models;

public record BoxSlotLocation(
    [property: JsonPropertyName("box")] int Box,
    [property: JsonPropertyName("slot")] int Slot
);

public record BatchLegalityResult(
    [property: JsonPropertyName("box")] int Box,
    [property: JsonPropertyName("slot")] int Slot,
    [property: JsonPropertyName("valid")] bool Valid,
    [property: JsonPropertyName("empty")] bool Empty,
    [property: JsonPropertyName("errors")] string[]? Errors,
    [property: JsonPropertyName("species")] int? Species
);

public record BatchLegalityResponse(
    [property: JsonPropertyName("results")] List<BatchLegalityResult> Results,
    [property: JsonPropertyName("validCount")] int ValidCount,
    [property: JsonPropertyName("invalidCount")] int InvalidCount,
    [property: JsonPropertyName("emptyCount")] int EmptyCount
);

public record BatchModification(
    [property: JsonPropertyName("box")] int Box,
    [property: JsonPropertyName("slot")] int Slot,
    [property: JsonPropertyName("modifications")] PokemonModifications Modifications
);

public record BatchOperationResult(
    [property: JsonPropertyName("box")] int Box,
    [property: JsonPropertyName("slot")] int Slot,
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("error")] string? Error
);

public record BatchOperationResponse(
    [property: JsonPropertyName("results")] List<BatchOperationResult> Results,
    [property: JsonPropertyName("successCount")] int SuccessCount,
    [property: JsonPropertyName("failCount")] int FailCount
);

public record BatchClearResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("clearedCount")] int ClearedCount,
    [property: JsonPropertyName("message")] string Message
);

public record BoxStatsData(
    [property: JsonPropertyName("box")] int Box,
    [property: JsonPropertyName("totalSlots")] int TotalSlots,
    [property: JsonPropertyName("occupied")] int Occupied,
    [property: JsonPropertyName("empty")] int Empty,
    [property: JsonPropertyName("shinyCount")] int ShinyCount,
    [property: JsonPropertyName("eggCount")] int EggCount,
    [property: JsonPropertyName("uniqueSpecies")] int UniqueSpecies
);
