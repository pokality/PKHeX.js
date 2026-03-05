using PKHeX.Core;
using PkhexWorld;
using PkhexWorld.wit.exports.pokality.pkhex;

namespace PkhexWorld.wit.exports.pokality.pkhex;

public class StorageOpsImpl : IStorageOps
{
    public static List<string> GetBoxNames(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        var names = new List<string>(save.BoxCount);

        for (int i = 0; i < save.BoxCount; i++)
        {
            if (save is IBoxDetailNameRead nameRead)
                names.Add(nameRead.GetBoxName(i));
            else
                names.Add(BoxDetailNameExtensions.GetDefaultBoxName(i));
        }

        return names;
    }

    public static List<IStorageOps.BoxInfo> GetBoxInfo(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        var result = new List<IStorageOps.BoxInfo>(save.BoxCount);

        for (int i = 0; i < save.BoxCount; i++)
        {
            string name;
            if (save is IBoxDetailNameRead nameRead)
                name = nameRead.GetBoxName(i);
            else
                name = BoxDetailNameExtensions.GetDefaultBoxName(i);

            int wallpaper = 0;
            if (save is IBoxDetailWallpaper wp)
                wallpaper = wp.GetBoxWallpaper(i);

            result.Add(new IStorageOps.BoxInfo(name, wallpaper));
        }

        return result;
    }

    public static void SetBoxWallpaper(uint handle, int box, int wallpaper)
    {
        var save = ImplHelpers.GetSave(handle);
        if (save is not IBoxDetailWallpaper wp)
            throw ImplHelpers.Unsupported("This save file does not support box wallpapers");

        if (box < 0 || box >= save.BoxCount)
            throw ImplHelpers.Validation($"Box index {box} out of range (0-{save.BoxCount - 1})");

        wp.SetBoxWallpaper(box, wallpaper);
    }

    public static IStorageOps.BoxStats GetBoxStats(uint handle, int box)
    {
        var save = ImplHelpers.GetSave(handle);

        if (box < 0 || box >= save.BoxCount)
            throw ImplHelpers.Validation($"Box index {box} out of range (0-{save.BoxCount - 1})");

        int occupied = 0;
        int shinyCount = 0;
        int eggCount = 0;
        var speciesSeen = new HashSet<ushort>();

        for (int slot = 0; slot < save.BoxSlotCount; slot++)
        {
            var pk = save.GetBoxSlotAtIndex(box, slot);
            if (pk.Species == 0)
                continue;

            occupied++;
            if (pk.IsShiny) shinyCount++;
            if (pk.IsEgg) eggCount++;
            speciesSeen.Add(pk.Species);
        }

        int empty = save.BoxSlotCount - occupied;

        return new IStorageOps.BoxStats(
            box: box,
            totalSlots: save.BoxSlotCount,
            occupied: occupied,
            empty: empty,
            shinyCount: shinyCount,
            eggCount: eggCount,
            uniqueSpecies: speciesSeen.Count
        );
    }

    public static IStorageOps.BatchLegalityResult BatchCheckLegality(uint handle, List<(int, int)> locations)
    {
        var save = ImplHelpers.GetSave(handle);
        var results = new List<IStorageOps.BatchLegalityEntry>();
        int validCount = 0;
        int invalidCount = 0;
        int emptyCount = 0;

        foreach (var (box, slot) in locations)
        {
            var pk = save.GetBoxSlotAtIndex(box, slot);

            if (pk.Species == 0)
            {
                emptyCount++;
                results.Add(new IStorageOps.BatchLegalityEntry(
                    box: box,
                    slot: slot,
                    valid: false,
                    empty: true,
                    errors: null,
                    species: null
                ));
                continue;
            }

            var analysis = new LegalityAnalysis(pk);
            var localizer = LegalityLocalizationContext.Create(analysis);
            var errors = new List<string>();

            foreach (var result in analysis.Results)
            {
                if (!result.Valid)
                    errors.Add(localizer.Humanize(result));
            }

            if (analysis.Valid)
                validCount++;
            else
                invalidCount++;

            results.Add(new IStorageOps.BatchLegalityEntry(
                box: box,
                slot: slot,
                valid: analysis.Valid,
                empty: false,
                errors: errors,
                species: pk.Species
            ));
        }

        return new IStorageOps.BatchLegalityResult(results, validCount, invalidCount, emptyCount);
    }

    public static List<IStorageOps.BatchOperationResult> BatchModifyPokemon(uint handle, List<IStorageOps.BatchModifyEntry> entries)
    {
        var save = ImplHelpers.GetSave(handle);
        var results = new List<IStorageOps.BatchOperationResult>();

        foreach (var entry in entries)
        {
            try
            {
                var pk = save.GetBoxSlotAtIndex(entry.box, entry.slot);
                if (pk.Species == 0)
                {
                    results.Add(new IStorageOps.BatchOperationResult(
                        entry.box, entry.slot, false, "Slot is empty"));
                    continue;
                }

                ImplHelpers.ApplyModifications(pk, entry.modifications);
                save.SetBoxSlotAtIndex(pk, entry.box, entry.slot);

                results.Add(new IStorageOps.BatchOperationResult(
                    entry.box, entry.slot, true, null));
            }
            catch (Exception ex)
            {
                results.Add(new IStorageOps.BatchOperationResult(
                    entry.box, entry.slot, false, ex.Message));
            }
        }

        return results;
    }

    public static int ClearBox(uint handle, int box)
    {
        var save = ImplHelpers.GetSave(handle);

        if (box < 0 || box >= save.BoxCount)
            throw ImplHelpers.Validation($"Box index {box} out of range (0-{save.BoxCount - 1})");

        int cleared = 0;
        for (int slot = 0; slot < save.BoxSlotCount; slot++)
        {
            var pk = save.GetBoxSlotAtIndex(box, slot);
            if (pk.Species == 0)
                continue;

            save.SetBoxSlotAtIndex(save.BlankPKM, box, slot);
            cleared++;
        }

        return cleared;
    }

    public static int ClearAllBoxes(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        int cleared = 0;

        for (int box = 0; box < save.BoxCount; box++)
        {
            for (int slot = 0; slot < save.BoxSlotCount; slot++)
            {
                var pk = save.GetBoxSlotAtIndex(box, slot);
                if (pk.Species == 0)
                    continue;

                save.SetBoxSlotAtIndex(save.BlankPKM, box, slot);
                cleared++;
            }
        }

        return cleared;
    }

    public static void SortBox(uint handle, int box, string sortBy)
    {
        var save = ImplHelpers.GetSave(handle);

        if (box < 0 || box >= save.BoxCount)
            throw ImplHelpers.Validation($"Box index {box} out of range (0-{save.BoxCount - 1})");

        var pokemon = new List<PKM>();
        for (int slot = 0; slot < save.BoxSlotCount; slot++)
        {
            var pk = save.GetBoxSlotAtIndex(box, slot);
            if (pk.Species != 0)
                pokemon.Add(pk);
        }

        var sorted = sortBy.ToLowerInvariant() switch
        {
            "species" => pokemon.OrderBy(p => p.Species).ToList(),
            "level" => pokemon.OrderBy(p => p.CurrentLevel).ToList(),
            "name" => pokemon.OrderBy(p => p.Nickname, StringComparer.OrdinalIgnoreCase).ToList(),
            "pokedex" => pokemon.OrderBy(p => p.Species).ToList(),
            "shiny" => pokemon.OrderByDescending(p => p.IsShiny).ThenBy(p => p.Species).ToList(),
            "type" => pokemon.OrderBy(p => p.PersonalInfo.Type1).ThenBy(p => p.PersonalInfo.Type2).ToList(),
            _ => throw ImplHelpers.Validation($"Unknown sort criteria: {sortBy}. Valid options: species, level, name, pokedex, shiny, type")
        };

        for (int slot = 0; slot < save.BoxSlotCount; slot++)
            save.SetBoxSlotAtIndex(save.BlankPKM, box, slot);

        for (int i = 0; i < sorted.Count; i++)
            save.SetBoxSlotAtIndex(sorted[i], box, i);
    }

    public static void CompactBox(uint handle, int box)
    {
        var save = ImplHelpers.GetSave(handle);

        if (box < 0 || box >= save.BoxCount)
            throw ImplHelpers.Validation($"Box index {box} out of range (0-{save.BoxCount - 1})");

        var pokemon = new List<PKM>();
        for (int slot = 0; slot < save.BoxSlotCount; slot++)
        {
            var pk = save.GetBoxSlotAtIndex(box, slot);
            if (pk.Species != 0)
                pokemon.Add(pk);
        }

        for (int slot = 0; slot < save.BoxSlotCount; slot++)
            save.SetBoxSlotAtIndex(save.BlankPKM, box, slot);

        for (int i = 0; i < pokemon.Count; i++)
            save.SetBoxSlotAtIndex(pokemon[i], box, i);
    }

    public static List<ITypes.PokemonSummary> GetBattleBox(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        var summaries = new List<ITypes.PokemonSummary>();

        if (save is SAV5 sav5)
        {
            for (int i = 0; i < BattleBox5.Count; i++)
            {
                var slotData = sav5.BattleBox.GetSlot(i);
                var pk = save.GetDecryptedPKM(slotData.ToArray());
                if (pk.Species == 0)
                    continue;

                summaries.Add(new ITypes.PokemonSummary(
                    box: -1, slot: i,
                    species: pk.Species,
                    speciesName: GameInfo.Strings.Species[pk.Species],
                    level: pk.CurrentLevel,
                    isEgg: pk.IsEgg,
                    isShiny: pk.IsShiny
                ));
            }
            return summaries;
        }

        if (save is SAV6XY sav6xy)
        {
            for (int i = 0; i < BattleBox6.Count; i++)
            {
                var slotData = sav6xy.BattleBox.GetSlot(i);
                var pk = save.GetDecryptedPKM(slotData.ToArray());
                if (pk.Species == 0)
                    continue;

                summaries.Add(new ITypes.PokemonSummary(
                    box: -1, slot: i,
                    species: pk.Species,
                    speciesName: GameInfo.Strings.Species[pk.Species],
                    level: pk.CurrentLevel,
                    isEgg: pk.IsEgg,
                    isShiny: pk.IsShiny
                ));
            }
            return summaries;
        }

        if (save is SAV6AO sav6ao)
        {
            for (int i = 0; i < BattleBox6.Count; i++)
            {
                var slotData = sav6ao.BattleBox.GetSlot(i);
                var pk = save.GetDecryptedPKM(slotData.ToArray());
                if (pk.Species == 0)
                    continue;

                summaries.Add(new ITypes.PokemonSummary(
                    box: -1, slot: i,
                    species: pk.Species,
                    speciesName: GameInfo.Strings.Species[pk.Species],
                    level: pk.CurrentLevel,
                    isEgg: pk.IsEgg,
                    isShiny: pk.IsShiny
                ));
            }
            return summaries;
        }

        throw ImplHelpers.Unsupported("Battle Box is only available in Gen 5 and Gen 6 saves");
    }

    public static void SetBattleBoxSlot(uint handle, int slot, byte[] data)
    {
        var save = ImplHelpers.GetSave(handle);

        if (slot < 0 || slot > 5)
            throw ImplHelpers.Validation($"Battle Box slot must be between 0 and 5, got {slot}");

        if (save is SAV5 sav5)
        {
            var dest = sav5.BattleBox.GetSlot(slot);
            data.CopyTo(dest);
            return;
        }

        if (save is SAV6XY sav6xy)
        {
            var dest = sav6xy.BattleBox.GetSlot(slot);
            data.CopyTo(dest);
            return;
        }

        if (save is SAV6AO sav6ao)
        {
            var dest = sav6ao.BattleBox.GetSlot(slot);
            data.CopyTo(dest);
            return;
        }

        throw ImplHelpers.Unsupported("Battle Box is only available in Gen 5 and Gen 6 saves");
    }

    public static IStorageOps.DaycareInfo GetDaycare(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);

        if (save is not IDaycareStorage daycare)
            throw ImplHelpers.Unsupported("This save file does not support daycare");

        ushort slot1Species = 0;
        string slot1Name = "(empty)";
        byte slot1Level = 0;
        ushort slot2Species = 0;
        string slot2Name = "(empty)";
        byte slot2Level = 0;

        if (daycare.DaycareSlotCount > 0 && daycare.IsDaycareOccupied(0))
        {
            var slotData = daycare.GetDaycareSlot(0);
            var pk = save.GetDecryptedPKM(slotData.ToArray());
            if (pk.Species != 0)
            {
                slot1Species = pk.Species;
                slot1Name = GameInfo.Strings.Species[pk.Species];
                slot1Level = pk.CurrentLevel;
            }
        }

        if (daycare.DaycareSlotCount > 1 && daycare.IsDaycareOccupied(1))
        {
            var slotData = daycare.GetDaycareSlot(1);
            var pk = save.GetDecryptedPKM(slotData.ToArray());
            if (pk.Species != 0)
            {
                slot2Species = pk.Species;
                slot2Name = GameInfo.Strings.Species[pk.Species];
                slot2Level = pk.CurrentLevel;
            }
        }

        bool hasEgg = false;
        if (save is IDaycareEggState eggState)
            hasEgg = eggState.IsEggAvailable;

        return new IStorageOps.DaycareInfo(
            slot1Species: slot1Species,
            slot1SpeciesName: slot1Name,
            slot1Level: slot1Level,
            slot2Species: slot2Species,
            slot2SpeciesName: slot2Name,
            slot2Level: slot2Level,
            hasEgg: hasEgg
        );
    }
}
