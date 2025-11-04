using System.Text.Json.Serialization;

namespace PKHeX.Models;

public record HallOfFameEntry(
    [property: JsonPropertyName("index")] int Index,
    [property: JsonPropertyName("timestamp")] string Timestamp,
    [property: JsonPropertyName("team")] PokemonSummary[] Team
);
