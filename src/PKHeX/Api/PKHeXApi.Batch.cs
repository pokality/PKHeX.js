using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;
using static PKHeX.Models.ErrorCodes;

namespace PKHeX.Api;

// Batch Operations
public partial class PKHeXApi
{
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string BatchCheckLegality(int handle, string locationsJson)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            var locations = JsonSerializer.Deserialize<BoxSlotLocation[]>(locationsJson, JsonContext.Default.Options);
            if (locations == null || locations.Length == 0)
                throw new ValidationException("Locations array cannot be empty", INVALID_JSON);

            var results = new List<BatchLegalityResult>();

            foreach (var loc in locations)
            {
                try
                {
                    var pk = save.GetBoxSlotAtIndex(loc.Box, loc.Slot);
                    if (pk.Species == 0)
                    {
                        results.Add(new BatchLegalityResult(loc.Box, loc.Slot, false, true, null, null));
                        continue;
                    }

                    var analysis = new LegalityAnalysis(pk);
                    var errorList = new List<string>();
                    var localizer = LegalityLocalizationContext.Create(analysis);

                    foreach (var r in analysis.Results)
                    {
                        if (!r.Valid)
                            errorList.Add(localizer.Humanize(r));
                    }

                    results.Add(new BatchLegalityResult(
                        loc.Box,
                        loc.Slot,
                        analysis.Valid,
                        false,
                        errorList.Count > 0 ? errorList.ToArray() : null,
                        pk.Species
                    ));
                }
                catch
                {
                    results.Add(new BatchLegalityResult(loc.Box, loc.Slot, false, false, new[] { "Failed to check legality" }, null));
                }
            }

            var validCount = results.Count(r => r.Valid);
            var invalidCount = results.Count(r => !r.Valid && !r.Empty);
            var emptyCount = results.Count(r => r.Empty);

            return new BatchLegalityResponse(results, validCount, invalidCount, emptyCount);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string BatchModifyPokemon(int handle, string modificationsJson)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            var mods = JsonSerializer.Deserialize<BatchModification[]>(modificationsJson, JsonContext.Default.Options);
            if (mods == null || mods.Length == 0)
                throw new ValidationException("Modifications array cannot be empty", INVALID_JSON);

            var results = new List<BatchOperationResult>();
            int successCount = 0;
            int failCount = 0;

            foreach (var mod in mods)
            {
                try
                {
                    var pk = save.GetBoxSlotAtIndex(mod.Box, mod.Slot);
                    if (pk.Species == 0)
                    {
                        results.Add(new BatchOperationResult(mod.Box, mod.Slot, false, "Empty slot"));
                        failCount++;
                        continue;
                    }

                    ApplyModifications(pk, mod.Modifications);
                    save.SetBoxSlotAtIndex(pk, mod.Box, mod.Slot);

                    results.Add(new BatchOperationResult(mod.Box, mod.Slot, true, null));
                    successCount++;
                }
                catch (Exception ex)
                {
                    results.Add(new BatchOperationResult(mod.Box, mod.Slot, false, ex.Message));
                    failCount++;
                }
            }

            return new BatchOperationResponse(results, successCount, failCount);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string ClearBox(int handle, int box)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (box < 0 || box >= save.BoxCount)
                throw new ValidationException($"Box {box} is out of range (0-{save.BoxCount - 1})", "INVALID_BOX");

            var slotsPerBox = save.BoxSlotCount;
            int clearedCount = 0;

            for (int slot = 0; slot < slotsPerBox; slot++)
            {
                var pk = save.GetBoxSlotAtIndex(box, slot);
                if (pk.Species != 0)
                {
                    save.SetBoxSlotAtIndex(save.BlankPKM, box, slot);
                    clearedCount++;
                }
            }

            return new BatchClearResponse(true, clearedCount, $"Cleared {clearedCount} Pokemon from box {box}");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string ClearAllBoxes(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            int clearedCount = 0;

            for (int box = 0; box < save.BoxCount; box++)
            {
                for (int slot = 0; slot < save.BoxSlotCount; slot++)
                {
                    var pk = save.GetBoxSlotAtIndex(box, slot);
                    if (pk.Species != 0)
                    {
                        save.SetBoxSlotAtIndex(save.BlankPKM, box, slot);
                        clearedCount++;
                    }
                }
            }

            return new BatchClearResponse(true, clearedCount, $"Cleared {clearedCount} Pokemon from all boxes");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SortBox(int handle, int box, string sortBy)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (box < 0 || box >= save.BoxCount)
                throw new ValidationException($"Box {box} is out of range (0-{save.BoxCount - 1})", "INVALID_BOX");

            var pokemon = new List<PKM>();
            var slotsPerBox = save.BoxSlotCount;

            // Collect all non-empty Pokemon from the box
            for (int slot = 0; slot < slotsPerBox; slot++)
            {
                var pk = save.GetBoxSlotAtIndex(box, slot);
                if (pk.Species != 0)
                    pokemon.Add(pk);
            }

            // Sort based on criteria
            IOrderedEnumerable<PKM> sorted = sortBy?.ToLowerInvariant() switch
            {
                "species" => pokemon.OrderBy(p => p.Species).ThenBy(p => p.Form),
                "level" => pokemon.OrderByDescending(p => p.CurrentLevel).ThenBy(p => p.Species),
                "name" => pokemon.OrderBy(p => GameInfo.Strings.Species[p.Species]),
                "pokedex" or "national" => pokemon.OrderBy(p => p.Species).ThenBy(p => p.Form),
                "shiny" => pokemon.OrderByDescending(p => p.IsShiny).ThenBy(p => p.Species),
                "type" => pokemon.OrderBy(p => p.PersonalInfo.Type1).ThenBy(p => p.PersonalInfo.Type2).ThenBy(p => p.Species),
                _ => pokemon.OrderBy(p => p.Species).ThenBy(p => p.Form)
            };

            var sortedList = sorted.ToList();

            // Clear the box first
            for (int slot = 0; slot < slotsPerBox; slot++)
                save.SetBoxSlotAtIndex(save.BlankPKM, box, slot);

            // Place sorted Pokemon back
            for (int i = 0; i < sortedList.Count && i < slotsPerBox; i++)
                save.SetBoxSlotAtIndex(sortedList[i], box, i);

            return new SuccessMessage(true, $"Sorted {sortedList.Count} Pokemon in box {box} by {sortBy ?? "species"}");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string CompactBox(int handle, int box)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (box < 0 || box >= save.BoxCount)
                throw new ValidationException($"Box {box} is out of range (0-{save.BoxCount - 1})", "INVALID_BOX");

            var pokemon = new List<PKM>();
            var slotsPerBox = save.BoxSlotCount;

            // Collect all non-empty Pokemon from the box
            for (int slot = 0; slot < slotsPerBox; slot++)
            {
                var pk = save.GetBoxSlotAtIndex(box, slot);
                if (pk.Species != 0)
                    pokemon.Add(pk);
            }

            // Clear the box
            for (int slot = 0; slot < slotsPerBox; slot++)
                save.SetBoxSlotAtIndex(save.BlankPKM, box, slot);

            // Place Pokemon back compacted at the start
            for (int i = 0; i < pokemon.Count; i++)
                save.SetBoxSlotAtIndex(pokemon[i], box, i);

            return new SuccessMessage(true, $"Compacted {pokemon.Count} Pokemon in box {box}");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetBoxStats(int handle, int box)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (box < 0 || box >= save.BoxCount)
                throw new ValidationException($"Box {box} is out of range (0-{save.BoxCount - 1})", "INVALID_BOX");

            var slotsPerBox = save.BoxSlotCount;
            int occupied = 0;
            int shinyCount = 0;
            int eggCount = 0;
            var speciesCounts = new Dictionary<int, int>();

            for (int slot = 0; slot < slotsPerBox; slot++)
            {
                var pk = save.GetBoxSlotAtIndex(box, slot);
                if (pk.Species == 0)
                    continue;

                occupied++;
                if (pk.IsShiny) shinyCount++;
                if (pk.IsEgg) eggCount++;

                if (speciesCounts.ContainsKey(pk.Species))
                    speciesCounts[pk.Species]++;
                else
                    speciesCounts[pk.Species] = 1;
            }

            return new BoxStatsData(
                box,
                slotsPerBox,
                occupied,
                slotsPerBox - occupied,
                shinyCount,
                eggCount,
                speciesCounts.Count
            );
        });
    }
}
