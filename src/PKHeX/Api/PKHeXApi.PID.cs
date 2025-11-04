using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;
using static PKHeX.Models.ErrorCodes;

namespace PKHeX.Api;

// PID Operations
public partial class PKHeXApi
{
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GeneratePID(int handle, int box, int slot, int nature, bool shiny)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (nature < 0 || nature > 24)
                throw new ValidationException($"Nature {nature} is out of range (0-24)", INVALID_NATURE);

            var rnd = new Random();
            var targetNature = (Nature)nature;
            var newPID = EntityPID.GetRandomPID(rnd, pk.Species, pk.Gender, save.Version, targetNature, pk.Form, pk.PID);

            if (shiny)
            {
                newPID = ShinyUtil.GetShinyPID(save.TID16, save.SID16, newPID, 0);
            }

            pk.PID = newPID;
            pk.Nature = targetNature;
            pk.RefreshAbility(pk.AbilityNumber >> 1);
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, $"Generated new PID: {newPID}");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetPID(int handle, int box, int slot, int pid)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (pid < 0)
                throw new ValidationException("PID must be non-negative", INVALID_PID);

            pk.PID = (uint)pid;
            pk.RefreshAbility(pk.AbilityNumber >> 1);
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, $"PID set to {pid}");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetShiny(int handle, int box, int slot, int shinyType)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (shinyType < 0 || shinyType > 5)
                throw new ValidationException($"Shiny type {shinyType} is out of range (0-5)", INVALID_SHINY_TYPE);

            var type = (Shiny)shinyType;

            if (type == Shiny.Never)
            {
                var changed = pk.SetUnshiny();
                if (changed)
                {
                    save.SetBoxSlotAtIndex(pk, box, slot);
                    return new SuccessMessage(true, "Pokemon is no longer shiny");
                }
                return new SuccessMessage(true, "Pokemon was already not shiny");
            }

            var wasChanged = CommonEdits.SetShiny(pk, type);
            if (wasChanged)
            {
                save.SetBoxSlotAtIndex(pk, box, slot);
                var typeName = type switch
                {
                    Shiny.AlwaysStar => "star shiny",
                    Shiny.AlwaysSquare => "square shiny",
                    _ => "shiny"
                };
                return new SuccessMessage(true, $"Pokemon is now {typeName}");
            }

            return new SuccessMessage(true, "Pokemon was already shiny");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPIDInfo(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            var isShiny = pk.IsShiny;
            var shinyType = pk.ShinyXor == 0 ? "Square" : pk.IsShiny ? "Star" : "None";
            var nature = pk.Nature;
            var gender = pk.Gender;
            var genderName = gender == 0 ? "Male" : gender == 1 ? "Female" : "Genderless";

            return new PIDInfo(
                true,
                pk.PID,
                isShiny,
                shinyType,
                (int)nature,
                nature.ToString(),
                gender,
                genderName
            );
        });
    }
}
