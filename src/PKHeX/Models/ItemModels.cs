using System.Text.Json.Serialization;

namespace PKHeX.Models;

public record ItemSlot(
    [property: JsonPropertyName("itemId")] int ItemId,
    [property: JsonPropertyName("itemName")] string ItemName,
    [property: JsonPropertyName("count")] int Count
);

public record PouchData(
    [property: JsonPropertyName("pouchType")] string PouchType,
    [property: JsonPropertyName("pouchIndex")] int PouchIndex,
    [property: JsonPropertyName("items")] List<ItemSlot> Items,
    [property: JsonPropertyName("totalSlots")] int TotalSlots
);
