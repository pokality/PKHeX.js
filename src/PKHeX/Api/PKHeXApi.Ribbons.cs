using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;
using static PKHeX.Models.ErrorCodes;

namespace PKHeX.Api;

// Ribbon Operations
public partial class PKHeXApi
{
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetRibbons(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            var ribbonInfo = RibbonInfo.GetRibbonInfo(pk);
            var ribbonList = ribbonInfo.Select(r => new
            {
                name = r.Name,
                hasRibbon = r.HasRibbon,
                ribbonCount = r.RibbonCount,
                type = r.Type.ToString()
            }).ToList();

            return ribbonList;
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetRibbonCount(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            var ribbonInfo = RibbonInfo.GetRibbonInfo(pk);
            var count = ribbonInfo.Count(r => r.HasRibbon || r.RibbonCount > 0);

            return new RibbonCountResponse(true, count);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetRibbon(int handle, int box, int slot, string ribbonName, bool value)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (string.IsNullOrWhiteSpace(ribbonName))
                throw new ValidationException("Ribbon name cannot be empty", "EMPTY_RIBBON_NAME");

            var property = pk.GetType().GetProperty(ribbonName);
            if (property == null)
                throw new ValidationException($"Ribbon '{ribbonName}' not found on this Pokemon", "INVALID_RIBBON");

            if (property.PropertyType == typeof(bool))
            {
                property.SetValue(pk, value);
            }
            else if (property.PropertyType == typeof(byte))
            {
                property.SetValue(pk, value ? (byte)1 : (byte)0);
            }
            else
            {
                throw new ValidationException($"Ribbon '{ribbonName}' has unsupported type", "INVALID_RIBBON_TYPE");
            }

            pk.RefreshChecksum();
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Ribbon set successfully");
        });
    }
}
