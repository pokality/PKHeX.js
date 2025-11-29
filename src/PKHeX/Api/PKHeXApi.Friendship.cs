using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;

namespace PKHeX.Api;

// Friendship and Affection Operations
public partial class PKHeXApi
{
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetFriendship(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            int? affection = null;
            int? fullness = null;
            int? enjoyment = null;

            // Affection is Gen 6-7 only (replaced by friendship in Gen 8+)
            if (pk is IAffection affectionPk)
            {
                affection = affectionPk.Affection;
                fullness = affectionPk.Fullness;
                enjoyment = affectionPk.Enjoyment;
            }

            return new FriendshipData(
                pk.CurrentFriendship,
                pk.OriginalTrainerFriendship,
                pk is IHandlerLanguage ht ? ht.HandlingTrainerFriendship : null,
                affection,
                fullness,
                enjoyment
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetFriendship(int handle, int box, int slot, int friendship)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            pk.CurrentFriendship = (byte)Math.Clamp(friendship, 0, 255);
            pk.RefreshChecksum();
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Friendship updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetOriginalTrainerFriendship(int handle, int box, int slot, int friendship)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            pk.OriginalTrainerFriendship = (byte)Math.Clamp(friendship, 0, 255);
            pk.RefreshChecksum();
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Original trainer friendship updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetHandlingTrainerFriendship(int handle, int box, int slot, int friendship)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (pk is not IHandlerLanguage htPk)
                throw new ValidationException("This Pokemon does not support handling trainer friendship", "UNSUPPORTED_FEATURE");

            htPk.HandlingTrainerFriendship = (byte)Math.Clamp(friendship, 0, 255);
            pk.RefreshChecksum();
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Handling trainer friendship updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetAffection(int handle, int box, int slot, int affection)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (pk is not IAffection affectionPk)
                throw new ValidationException("This Pokemon does not support affection (Gen 6-7 only)", "UNSUPPORTED_FEATURE");

            affectionPk.Affection = (byte)Math.Clamp(affection, 0, 255);
            pk.RefreshChecksum();
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Affection updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetFullness(int handle, int box, int slot, int fullness)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (pk is not IAffection affectionPk)
                throw new ValidationException("This Pokemon does not support fullness (Gen 6-7 only)", "UNSUPPORTED_FEATURE");

            affectionPk.Fullness = (byte)Math.Clamp(fullness, 0, 255);
            pk.RefreshChecksum();
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Fullness updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetEnjoyment(int handle, int box, int slot, int enjoyment)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (pk is not IAffection affectionPk)
                throw new ValidationException("This Pokemon does not support enjoyment (Gen 6-7 only)", "UNSUPPORTED_FEATURE");

            affectionPk.Enjoyment = (byte)Math.Clamp(enjoyment, 0, 255);
            pk.RefreshChecksum();
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Enjoyment updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string MaximizeFriendship(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            pk.CurrentFriendship = 255;
            pk.OriginalTrainerFriendship = 255;

            if (pk is IHandlerLanguage htPk)
                htPk.HandlingTrainerFriendship = 255;

            if (pk is IAffection affectionPk)
            {
                affectionPk.Affection = 255;
                affectionPk.Fullness = 255;
                affectionPk.Enjoyment = 255;
            }

            pk.RefreshChecksum();
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Friendship and affection maximized");
        });
    }
}
