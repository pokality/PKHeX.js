using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;
using static PKHeX.Models.ErrorCodes;

namespace PKHeX.Api;

// Party Operations
public partial class PKHeXApi
{
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetParty(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var partyList = new List<PokemonSummary>();
            var partyData = save.PartyData;

            for (int i = 0; i < partyData.Count; i++)
            {
                var pk = partyData[i];
                if (pk == null || pk.Species == 0)
                    continue;

                partyList.Add(new PokemonSummary(
                    -1,
                    i,
                    pk.Species,
                    GameInfo.Strings.Species[pk.Species],
                    pk.CurrentLevel,
                    pk.IsEgg,
                    pk.IsShiny
                ));
            }

            return partyList;
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPartySlot(int handle, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (slot < 0 || slot >= 6)
                throw new ValidationException($"Party slot {slot} is out of range (0-5)", INVALID_SLOT);

            if (slot >= save.PartyCount)
                throw new ValidationException($"Party slot {slot} is empty", EMPTY_SLOT);

            var pk = save.GetPartySlotAtIndex(slot);
            if (pk.Species == 0)
                throw new ValidationException($"No Pokemon in party slot {slot}", EMPTY_SLOT);

            return CreatePokemonDetailObject(pk);
        });
    }
}
