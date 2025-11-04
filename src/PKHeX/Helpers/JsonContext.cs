using System.Text.Json.Serialization;
using PKHeX.Models;

namespace PKHeX.Helpers;

[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.Never)]
[JsonSerializable(typeof(SaveFileHandle))]
[JsonSerializable(typeof(SaveFileInfo))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(PokemonSummary))]
[JsonSerializable(typeof(PokemonDetail))]
[JsonSerializable(typeof(TrainerInfo))]
[JsonSerializable(typeof(LegalityResult))]
[JsonSerializable(typeof(RibbonData))]
[JsonSerializable(typeof(ContestStats))]
[JsonSerializable(typeof(TrainerCard))]
[JsonSerializable(typeof(TrainerAppearance))]
[JsonSerializable(typeof(BadgeData))]
[JsonSerializable(typeof(BoxInfo))]
[JsonSerializable(typeof(DaycareData))]
[JsonSerializable(typeof(ItemSlot))]
[JsonSerializable(typeof(PouchData))]
[JsonSerializable(typeof(PokedexEntry))]
[JsonSerializable(typeof(HallOfFameEntry))]
[JsonSerializable(typeof(MailMessage))]
[JsonSerializable(typeof(MysteryGiftCard))]
[JsonSerializable(typeof(NamedEntity))]
[JsonSerializable(typeof(List<PokemonSummary>))]
[JsonSerializable(typeof(List<RibbonData>))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(List<BoxInfo>))]
[JsonSerializable(typeof(List<PouchData>))]
[JsonSerializable(typeof(List<PokedexEntry>))]
[JsonSerializable(typeof(List<HallOfFameEntry>))]
[JsonSerializable(typeof(List<MailMessage>))]
[JsonSerializable(typeof(List<MysteryGiftCard>))]
[JsonSerializable(typeof(List<NamedEntity>))]
[JsonSerializable(typeof(List<bool>))]
[JsonSerializable(typeof(Dictionary<string, object>))]
public partial class JsonContext : JsonSerializerContext
{
}
