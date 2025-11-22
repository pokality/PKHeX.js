using System.Text.Json.Serialization;

namespace PKHeX.Models;

public record ErrorResponse(
    [property: JsonPropertyName("error")] string Message,
    [property: JsonPropertyName("code")] string? Code = null
);
