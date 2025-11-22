using System.Text.Json.Serialization;

namespace PKHeX.Models;

public record NamedEntity(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name
);

public record NameResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("name")] string Name
);

public record SpeciesListResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("species")] List<NamedEntity> Species
);

public record MovesListResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("moves")] List<NamedEntity> Moves
);

public record AbilitiesListResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("abilities")] List<NamedEntity> Abilities
);

public record ItemsListResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("items")] List<NamedEntity> Items
);

public record NaturesListResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("natures")] List<NamedEntity> Natures
);

public record TypesListResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("types")] List<NamedEntity> Types
);
