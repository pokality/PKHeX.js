using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;

namespace PKHeX.Api;

// Legality Operations
public partial class PKHeXApi
{
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string CheckLegality(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            var analysis = new LegalityAnalysis(pk);
            var errorList = new List<string>();
            var localizer = LegalityLocalizationContext.Create(analysis);

            foreach (var r in analysis.Results)
            {
                if (!r.Valid)
                    errorList.Add(localizer.Humanize(r));
            }

            return new
            {
                valid = analysis.Valid,
                errors = errorList.ToArray(),
                parsed = analysis.Report()
            };
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string LegalizePokemon(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            pk.SetMoveset();
            pk.Heal();
            pk.RefreshChecksum();

            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Pokemon legalized successfully");
        });
    }
}
