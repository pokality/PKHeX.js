using System.Text.Json.Serialization;

namespace PKHeX.Models;

public record MailMessage(
    [property: JsonPropertyName("index")] int Index,
    [property: JsonPropertyName("authorName")] string AuthorName,
    [property: JsonPropertyName("authorTID")] int AuthorTID,
    [property: JsonPropertyName("mailType")] int MailType,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("isHeld")] bool IsHeld,
    [property: JsonPropertyName("heldItem")] int HeldItem
);

public record MysteryGiftCard(
    [property: JsonPropertyName("index")] int Index,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("cardTitle")] string CardTitle,
    [property: JsonPropertyName("isItem")] bool IsItem,
    [property: JsonPropertyName("isPokemon")] bool IsPokemon,
    [property: JsonPropertyName("isBP")] bool IsBP,
    [property: JsonPropertyName("itemId")] int ItemId,
    [property: JsonPropertyName("species")] int Species,
    [property: JsonPropertyName("level")] int Level,
    [property: JsonPropertyName("isShiny")] bool IsShiny,
    [property: JsonPropertyName("isEgg")] bool IsEgg
);

