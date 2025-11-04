using System.Text.Json.Serialization;

namespace PKHeX.Models;

public record SaveFileInfo(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("generation")] string Generation,
    [property: JsonPropertyName("gameVersion")] string GameVersion,
    [property: JsonPropertyName("ot")] string OT,
    [property: JsonPropertyName("tid")] uint TID,
    [property: JsonPropertyName("sid")] uint SID,
    [property: JsonPropertyName("boxCount")] int BoxCount
);

public record SaveFileHandle(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("handle")] int Handle
);

public record SuccessMessage(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("message")] string Message
);

public record ExportSaveResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("base64Data")] string Base64Data
);

public record ShowdownResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("showdownText")] string ShowdownText
);

public record RibbonCountResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("ribbonCount")] int RibbonCount
);

public record RivalNameResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("rivalName")] string RivalName
);

public record BattlePointsResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("battlePoints")] int BattlePoints
);

public record CoinsResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("coins")] int Coins
);

public record RecordsResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("records")] Dictionary<string, int> Records
);

public record EventFlagResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("flagIndex")] int FlagIndex,
    [property: JsonPropertyName("value")] bool Value
);

public record EventConstResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("constIndex")] int ConstIndex,
    [property: JsonPropertyName("value")] int Value
);

public record SecondsPlayedResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("secondsPlayed")] int SecondsPlayed
);

public record SecondsToStartResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("secondsToStart")] uint SecondsToStart
);

public record SecondsToFameResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("secondsToFame")] uint SecondsToFame
);

public record HallOfFameResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("entries")] List<HallOfFameEntry> Entries
);

public record MysteryGiftsResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("gifts")] List<MysteryGiftCard> Gifts
);

public record MysteryGiftCardResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("card")] MysteryGiftCard Card
);

public record MysteryGiftFlagsResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("flags")] bool[] Flags
);

public record SecretBaseResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("secretBase")] SecretBaseData SecretBase
);

public record EntralinkResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("entralinkData")] EntralinkData EntralinkData
);

public record PokePelagoResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("pokePelagoData")] PokePelagoData PokePelagoData
);

public record FestivalPlazaResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("festivalPlazaData")] FestivalPlazaData FestivalPlazaData
);

public record PokeJobsResponse(
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("pokeJobsData")] PokeJobsData PokeJobsData
);
