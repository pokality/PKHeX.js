using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;
using static PKHeX.Models.ErrorCodes;

namespace PKHeX.Api;

// Showdown Operations
public partial class PKHeXApi
{
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string ExportShowdown(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            var showdownText = ShowdownParsing.GetShowdownText(pk);
            return new ShowdownResponse(true, showdownText);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string ImportShowdown(int handle, int box, int slot, string showdownText)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            ApiHelpers.ValidateBox(save, box);
            ApiHelpers.ValidateSlot(save, slot);

            if (string.IsNullOrWhiteSpace(showdownText))
                throw new ValidationException("Showdown text cannot be empty", "EMPTY_SHOWDOWN_TEXT");

            if (!ShowdownParsing.TryParseAnyLanguage(showdownText.AsSpan(), out var set))
                throw new ValidationException("Failed to parse Showdown text", "INVALID_SHOWDOWN_FORMAT");

            if (set.Species == 0)
                throw new ValidationException("Invalid species in Showdown text", INVALID_SPECIES);

            var pk = save.BlankPKM;
            ApplyShowdownSet(pk, set, save);

            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Pokemon imported from Showdown format successfully");
        });
    }
}
