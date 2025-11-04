namespace PKHeX.Models;

public record SecretBaseData(
    string TrainerName,
    int TrainerID,
    int SecretID,
    int Gender,
    int Language,
    string LocationName,
    int LocationID
);

public record EntralinkData(
    int ForestLevel,
    int MissionsCompleted,
    int WhiteForestCount,
    int BlackCityCount
);

public record PokePelagoData(
    int BeansCount,
    int IsleAevelynDevelopment,
    int IsleAphunDevelopment,
    int IsleEvelupDevelopment,
    int PokemonCount
);

public record FestivalPlazaData(
    int Rank,
    int FestivalCoins,
    int TotalVisitors,
    int FacilityCount
);

public record PokeJobsData(
    int ActiveJobsCount,
    int CompletedJobsCount
);
