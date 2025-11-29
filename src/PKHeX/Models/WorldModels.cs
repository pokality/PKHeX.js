using System.Text.Json.Serialization;

namespace PKHeX.Models;

public record SecretBaseData(
    [property: JsonPropertyName("trainerName")] string TrainerName,
    [property: JsonPropertyName("trainerId")] int TrainerID,
    [property: JsonPropertyName("secretId")] int SecretID,
    [property: JsonPropertyName("gender")] int Gender,
    [property: JsonPropertyName("language")] int Language,
    [property: JsonPropertyName("locationName")] string LocationName,
    [property: JsonPropertyName("locationId")] int LocationID
);

public record EntralinkData(
    [property: JsonPropertyName("forestLevel")] int ForestLevel,
    [property: JsonPropertyName("missionsCompleted")] int MissionsCompleted,
    [property: JsonPropertyName("whiteForestCount")] int WhiteForestCount,
    [property: JsonPropertyName("blackCityCount")] int BlackCityCount
);

public record PokePelagoData(
    [property: JsonPropertyName("beansCount")] int BeansCount,
    [property: JsonPropertyName("isleAevelynDevelopment")] int IsleAevelynDevelopment,
    [property: JsonPropertyName("isleAphunDevelopment")] int IsleAphunDevelopment,
    [property: JsonPropertyName("isleEvelupDevelopment")] int IsleEvelupDevelopment,
    [property: JsonPropertyName("pokemonCount")] int PokemonCount
);

public record FestivalPlazaData(
    [property: JsonPropertyName("rank")] int Rank,
    [property: JsonPropertyName("festivalCoins")] int FestivalCoins,
    [property: JsonPropertyName("totalVisitors")] int TotalVisitors,
    [property: JsonPropertyName("facilityCount")] int FacilityCount
);

public record PokeJobsData(
    [property: JsonPropertyName("activeJobsCount")] int ActiveJobsCount,
    [property: JsonPropertyName("completedJobsCount")] int CompletedJobsCount
);
