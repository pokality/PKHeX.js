using System.Text.Json.Serialization;

namespace PKHeX.Models;

public record PokedexEntry(
    [property: JsonPropertyName("species")] int Species,
    [property: JsonPropertyName("speciesName")] string SpeciesName,
    [property: JsonPropertyName("seen")] bool Seen,
    [property: JsonPropertyName("caught")] bool Caught
);
