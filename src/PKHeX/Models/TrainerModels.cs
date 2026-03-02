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

public record TrainerInfoResponse(
    [property: JsonPropertyName("success")] bool Success,
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

public record PlayerAppearance9aInput(
    [property: JsonPropertyName("skinColor")] uint? SkinColor = null,
    [property: JsonPropertyName("lipColor")] uint? LipColor = null,
    [property: JsonPropertyName("darkCircles")] uint? DarkCircles = null,
    [property: JsonPropertyName("eyeColor")] uint? EyeColor = null,
    [property: JsonPropertyName("eyebrowColor")] uint? EyebrowColor = null,
    [property: JsonPropertyName("eyebrowShape")] uint? EyebrowShape = null,
    [property: JsonPropertyName("eyelashColor")] uint? EyelashColor = null,
    [property: JsonPropertyName("eyelashShape")] uint? EyelashShape = null,
    [property: JsonPropertyName("beautySpotFirst")] uint? BeautySpotFirst = null,
    [property: JsonPropertyName("beautySpotSecond")] uint? BeautySpotSecond = null,
    [property: JsonPropertyName("freckles")] uint? Freckles = null,
    [property: JsonPropertyName("hairColor")] uint? HairColor = null,
    [property: JsonPropertyName("colorBlocking")] uint? ColorBlocking = null,
    [property: JsonPropertyName("balayageFadeFirst")] uint? BalayageFadeFirst = null,
    [property: JsonPropertyName("balayageFadeSecond")] uint? BalayageFadeSecond = null,
    [property: JsonPropertyName("faceShape")] uint? FaceShape = null,
    [property: JsonPropertyName("bangs")] uint? Bangs = null,
    [property: JsonPropertyName("hairColorMode")] uint? HairColorMode = null
);

public record BadgeData(
    [property: JsonPropertyName("badgeCount")] int BadgeCount,
    [property: JsonPropertyName("badges")] bool[] Badges
);

public record PIDInfo(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("pid")] uint PID,
    [property: JsonPropertyName("isShiny")] bool IsShiny,
    [property: JsonPropertyName("shinyType")] string ShinyType,
    [property: JsonPropertyName("nature")] int Nature,
    [property: JsonPropertyName("natureName")] string NatureName,
    [property: JsonPropertyName("gender")] int Gender,
    [property: JsonPropertyName("genderName")] string GenderName
);

public record PKMDataResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("base64Data")] string Base64Data
);
