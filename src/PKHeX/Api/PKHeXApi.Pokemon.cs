using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;
using static PKHeX.Models.ErrorCodes;

namespace PKHeX.Api;

// Box Pokemon Operations
public partial class PKHeXApi
{
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetAllPokemon(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pokemonList = new List<PokemonSummary>();
            var boxData = save.BoxData;

            for (int i = 0; i < boxData.Count; i++)
            {
                var pk = boxData[i];
                if (pk.Species == 0)
                    continue;

                save.GetBoxSlotFromIndex(i, out int box, out int slot);
                pokemonList.Add(new PokemonSummary(
                    Box: box,
                    Slot: slot,
                    Species: pk.Species,
                    SpeciesName: GameInfo.Strings.Species[pk.Species],
                    Level: pk.CurrentLevel,
                    IsEgg: pk.IsEgg,
                    IsShiny: pk.IsShiny
                ));
            }

            return pokemonList;
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPokemon(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);
            return CreatePokemonDetailObject(pk);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string ModifyPokemon(int handle, int box, int slot, string modificationsJson)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            var mods = JsonSerializer.Deserialize<PokemonModifications>(modificationsJson, JsonOptions);
            if (mods == null)
                throw new ValidationException("Invalid modifications JSON", INVALID_JSON);

            ApplyModifications(pk, mods, save);
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Pokemon modified successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetPokemon(int handle, int box, int slot, string base64PkmData)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            ApiHelpers.ValidateBox(save, box);
            ApiHelpers.ValidateSlot(save, slot);

            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            var pk = save.GetDecryptedPKM(data);
            if (pk.Species == 0)
                throw new ValidationException("Invalid Pokemon data", INVALID_PKM_DATA);

            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Pokemon set successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string DeletePokemon(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            ApiHelpers.ValidateBox(save, box);
            ApiHelpers.ValidateSlot(save, slot);

            save.SetBoxSlotAtIndex(save.BlankPKM, box, slot);

            return new SuccessMessage(true, "Pokemon deleted successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string MovePokemon(int handle, int fromBox, int fromSlot, int toBox, int toSlot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            ApiHelpers.ValidateBox(save, fromBox);
            ApiHelpers.ValidateSlot(save, fromSlot);
            ApiHelpers.ValidateBox(save, toBox);
            ApiHelpers.ValidateSlot(save, toSlot);

            var sourcePk = save.GetBoxSlotAtIndex(fromBox, fromSlot);
            if (sourcePk.Species == 0)
                throw new ValidationException($"No Pokemon in source box {fromBox} slot {fromSlot}", EMPTY_SLOT);

            var destPk = save.GetBoxSlotAtIndex(toBox, toSlot);

            save.SetBoxSlotAtIndex(sourcePk, toBox, toSlot);
            save.SetBoxSlotAtIndex(destPk.Species == 0 ? save.BlankPKM : destPk, fromBox, fromSlot);

            return new SuccessMessage(true, "Pokemon moved successfully");
        });
    }
}
