using PKHeX.Core;
using PkhexWorld;
using PkhexWorld.wit.exports.pokality.pkhex;

namespace PkhexWorld.wit.exports.pokality.pkhex;

public class PartyOpsImpl : IPartyOps
{
    public static List<ITypes.PokemonSummary> GetParty(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        var party = new List<ITypes.PokemonSummary>();

        for (int i = 0; i < save.PartyData.Count; i++)
        {
            var pk = save.PartyData[i];
            if (pk.Species == 0)
                continue;

            party.Add(new ITypes.PokemonSummary(
                box: -1,
                slot: i,
                species: pk.Species,
                speciesName: GameInfo.Strings.Species[pk.Species],
                level: pk.CurrentLevel,
                isEgg: pk.IsEgg,
                isShiny: pk.IsShiny
            ));
        }

        return party;
    }

    public static ITypes.PokemonDetail GetPartySlot(uint handle, int slot)
    {
        var save = ImplHelpers.GetSave(handle);

        if (slot < 0 || slot > 5)
            throw ImplHelpers.Validation($"Party slot must be between 0 and 5, got {slot}");
        if (slot >= save.PartyCount)
            throw ImplHelpers.Validation($"Party slot {slot} is empty (party count: {save.PartyCount})");

        var pk = save.GetPartySlotAtIndex(slot);
        if (pk.Species == 0)
            throw ImplHelpers.Validation($"No Pokemon in party slot {slot}");

        return ImplHelpers.CreatePokemonDetail(pk);
    }
}
