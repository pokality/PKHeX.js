using System.Text.Json.Serialization;

namespace PKHeX.Models;

public record NamedEntity(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name
);
