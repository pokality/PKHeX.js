using System.Text.Json.Serialization;

namespace PKHeX.Models;

public record BoxInfo(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("wallpaper")] int Wallpaper
);

public record DaycareData(
    [property: JsonPropertyName("slot1Species")] int Slot1Species,
    [property: JsonPropertyName("slot1SpeciesName")] string Slot1SpeciesName,
    [property: JsonPropertyName("slot1Level")] int Slot1Level,
    [property: JsonPropertyName("slot2Species")] int Slot2Species,
    [property: JsonPropertyName("slot2SpeciesName")] string Slot2SpeciesName,
    [property: JsonPropertyName("slot2Level")] int Slot2Level,
    [property: JsonPropertyName("hasEgg")] bool HasEgg
);
