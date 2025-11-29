using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;

namespace PKHeX.Api;

// Form Management Operations
public partial class PKHeXApi
{
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetForm(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            var formNames = FormConverter.GetFormList(pk.Species, GameInfo.Strings.Types, GameInfo.Strings.forms, Array.Empty<string>(), pk.Context);
            var formName = pk.Form < formNames.Length ? formNames[pk.Form] : $"Form {pk.Form}";

            uint? formArgument = null;
            uint? formArgumentRemain = null;
            uint? formArgumentElapsed = null;
            uint? formArgumentMaximum = null;

            if (pk is IFormArgument formArg)
            {
                formArgument = formArg.FormArgument;
                formArgumentRemain = formArg.FormArgumentRemain;
                formArgumentElapsed = formArg.FormArgumentElapsed;
                formArgumentMaximum = formArg.FormArgumentMaximum;
            }

            return new FormData(
                pk.Form,
                formName,
                (byte)formNames.Length,
                formArgument,
                formArgumentRemain,
                formArgumentElapsed,
                formArgumentMaximum
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetForm(int handle, int box, int slot, int form)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            var pi = pk.PersonalInfo;
            var maxForm = pi.FormCount - 1;

            if (form < 0 || form > maxForm)
                throw new ValidationException($"Form {form} is out of range (0-{maxForm}) for {GameInfo.Strings.Species[pk.Species]}", "INVALID_FORM");

            pk.Form = (byte)form;
            pk.RefreshChecksum();
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Form updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetFormArgument(int handle, int box, int slot, uint formArgument)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (pk is not IFormArgument formArg)
                throw new ValidationException("This Pokemon does not support form arguments", "UNSUPPORTED_FEATURE");

            formArg.FormArgument = formArgument;
            pk.RefreshChecksum();
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Form argument updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetAvailableForms(int species, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            if (species < 1 || species > 1025)
                throw new ValidationException($"Species {species} is out of range (1-1025)", "INVALID_SPECIES");

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var context = (EntityContext)generation;
            var pt = GameInfo.GetPersonalTable(context);

            if (species >= pt.MaxSpeciesID)
                throw new ValidationException($"Species {species} not available in generation {generation}", "INVALID_SPECIES");

            var formNames = FormConverter.GetFormList((ushort)species, GameInfo.Strings.Types, GameInfo.Strings.forms, Array.Empty<string>(), context);

            var forms = new List<FormInfo>();
            for (int i = 0; i < formNames.Length; i++)
            {
                var name = string.IsNullOrEmpty(formNames[i]) ? (i == 0 ? "Normal" : $"Form {i}") : formNames[i];
                forms.Add(new FormInfo(i, name));
            }

            return new AvailableFormsData(
                species,
                GameInfo.Strings.Species[species],
                generation,
                forms,
                forms.Count
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string ChangeSpeciesAndForm(int handle, int box, int slot, int species, int form)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (species < 1 || species > 1025)
                throw new ValidationException($"Species {species} is out of range (1-1025)", "INVALID_SPECIES");

            var pt = pk.PersonalInfo.GetType() == typeof(PersonalInfo9SV)
                ? PersonalTable.SV
                : GameInfo.GetPersonalTable(pk.Context);

            if (species > pt.MaxSpeciesID)
                throw new ValidationException($"Species {species} not available in this game", "INVALID_SPECIES");

            var newPi = pt.GetFormEntry((ushort)species, (byte)form);
            if (newPi == null || !newPi.IsPresentInGame)
                throw new ValidationException($"Form {form} not available for species {species} in this game", "INVALID_FORM");

            pk.Species = (ushort)species;
            pk.Form = (byte)form;

            // Reset form argument when changing species/form
            if (pk is IFormArgument formArg)
                formArg.FormArgument = 0;

            // Update nickname to species name if not nicknamed
            if (!pk.IsNicknamed)
                pk.ClearNickname();

            pk.RefreshChecksum();
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Species and form updated successfully");
        });
    }
}
