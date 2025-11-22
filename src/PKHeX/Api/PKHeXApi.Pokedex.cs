using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;
using static PKHeX.Models.ErrorCodes;

namespace PKHeX.Api;

// Pokedex Operations
public partial class PKHeXApi
{
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPokedex(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            var pokedexList = new List<PokedexEntry>();

            for (ushort species = 1; species <= save.MaxSpeciesID; species++)
            {
                var seen = save.GetSeen(species);
                var caught = save.GetCaught(species);

                if (seen || caught)
                {
                    pokedexList.Add(new PokedexEntry(
                        (int)species,
                        GameInfo.Strings.Species[species],
                        seen,
                        caught
                    ));
                }
            }

            return pokedexList;
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetPokedexSeen(int handle, int species, int form)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (species < 0 || species > save.MaxSpeciesID)
                throw new ValidationException($"Species {species} is out of range (0-{save.MaxSpeciesID})", INVALID_SPECIES);

            save.SetSeen((ushort)species, true);

            return new SuccessMessage(true, "Pokedex seen status updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetPokedexCaught(int handle, int species, int form)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (species < 0 || species > save.MaxSpeciesID)
                throw new ValidationException($"Species {species} is out of range (0-{save.MaxSpeciesID})", INVALID_SPECIES);

            save.SetCaught((ushort)species, true);

            return new SuccessMessage(true, "Pokedex caught status updated successfully");
        });
    }
}
