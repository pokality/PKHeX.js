using System.Text.Json.Serialization;

namespace PKHeX.Models;

public record TrainerInfo(
    [property: JsonPropertyName("ot")] string OT,
    [property: JsonPropertyName("tid")] uint TID,
    [property: JsonPropertyName("sid")] uint SID,
    [property: JsonPropertyName("gender")] int Gender,
    [property: JsonPropertyName("language")] int Language,
    [property: JsonPropertyName("money")] uint Money,
    [property: JsonPropertyName("playedHours")] int PlayedHours,
    [property: JsonPropertyName("playedMinutes")] int PlayedMinutes,
    [property: JsonPropertyName("playedSeconds")] int PlayedSeconds
);

public record TrainerCard(
    [property: JsonPropertyName("ot")] string OT,
    [property: JsonPropertyName("tid")] uint TID,
    [property: JsonPropertyName("sid")] uint SID,
    [property: JsonPropertyName("money")] uint Money,
    [property: JsonPropertyName("startDate")] string? StartDate,
    [property: JsonPropertyName("fame")] int Fame
);

public record TrainerAppearance(
    [property: JsonPropertyName("skin")] int Skin,
    [property: JsonPropertyName("hair")] int Hair,
    [property: JsonPropertyName("top")] int Top,
    [property: JsonPropertyName("bottom")] int Bottom,
    [property: JsonPropertyName("shoes")] int Shoes,
    [property: JsonPropertyName("accessory")] int Accessory,
    [property: JsonPropertyName("bag")] int Bag,
    [property: JsonPropertyName("hat")] int Hat
);

public record BadgeData(
    [property: JsonPropertyName("badgeCount")] int BadgeCount,
    [property: JsonPropertyName("badges")] bool[] Badges
);
