using System.Text.Json.Serialization;

namespace PKHeX.Models;

public record FriendshipData(
    [property: JsonPropertyName("currentFriendship")] int CurrentFriendship,
    [property: JsonPropertyName("originalTrainerFriendship")] int OriginalTrainerFriendship,
    [property: JsonPropertyName("handlingTrainerFriendship")] int? HandlingTrainerFriendship,
    [property: JsonPropertyName("affection")] int? Affection,
    [property: JsonPropertyName("fullness")] int? Fullness,
    [property: JsonPropertyName("enjoyment")] int? Enjoyment
);
