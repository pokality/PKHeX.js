using PKHeX.Core;
using PkhexWorld;
using PkhexWorld.wit.exports.pokality.pkhex;

namespace PkhexWorld.wit.exports.pokality.pkhex;

public class PokemonOpsImpl : IPokemonOps
{
    public static List<ITypes.PokemonSummary> GetAllPokemon(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        var pokemonList = new List<ITypes.PokemonSummary>();
        var boxData = save.BoxData;

        for (int i = 0; i < boxData.Count; i++)
        {
            var pk = boxData[i];
            if (pk.Species == 0)
                continue;

            save.GetBoxSlotFromIndex(i, out int box, out int slot);
            pokemonList.Add(new ITypes.PokemonSummary(
                box: box,
                slot: slot,
                species: pk.Species,
                speciesName: GameInfo.Strings.Species[pk.Species],
                level: pk.CurrentLevel,
                isEgg: pk.IsEgg,
                isShiny: pk.IsShiny
            ));
        }

        return pokemonList;
    }

    public static ITypes.PokemonDetail GetPokemon(uint handle, int box, int slot)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);
        return ImplHelpers.CreatePokemonDetail(pk);
    }

    public static void ModifyPokemon(uint handle, int box, int slot, ITypes.PokemonModifications mods)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);
        ImplHelpers.ApplyModifications(pk, mods);
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static void SetPokemon(uint handle, int box, int slot, byte[] data)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = save.GetDecryptedPKM(data);
        if (pk.Species == 0)
            throw ImplHelpers.Validation("Invalid Pokemon data: species is 0");

        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static void DeletePokemon(uint handle, int box, int slot)
    {
        var save = ImplHelpers.GetSave(handle);
        save.SetBoxSlotAtIndex(save.BlankPKM, box, slot);
    }

    public static void MovePokemon(uint handle, int fromBox, int fromSlot, int toBox, int toSlot)
    {
        var save = ImplHelpers.GetSave(handle);

        var sourcePk = save.GetBoxSlotAtIndex(fromBox, fromSlot);
        if (sourcePk.Species == 0)
            throw ImplHelpers.Validation($"No Pokemon in source box {fromBox} slot {fromSlot}");

        var destPk = save.GetBoxSlotAtIndex(toBox, toSlot);

        save.SetBoxSlotAtIndex(sourcePk, toBox, toSlot);
        save.SetBoxSlotAtIndex(destPk.Species == 0 ? save.BlankPKM : destPk, fromBox, fromSlot);
    }

    public static string ExportShowdown(uint handle, int box, int slot)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);
        return ShowdownParsing.GetShowdownText(pk);
    }

    public static void ImportShowdown(uint handle, int box, int slot, string text)
    {
        var save = ImplHelpers.GetSave(handle);

        if (string.IsNullOrWhiteSpace(text))
            throw ImplHelpers.Validation("Showdown text cannot be empty");

        if (!ShowdownParsing.TryParseAnyLanguage(text.AsSpan(), out var set))
            throw ImplHelpers.Validation("Failed to parse Showdown text");

        if (set.Species == 0)
            throw ImplHelpers.Validation("Invalid species in Showdown text");

        var pk = save.BlankPKM;

        pk.Species = set.Species;
        pk.Form = set.Form;
        pk.HeldItem = set.HeldItem;
        pk.Ability = set.Ability;
        pk.CurrentLevel = set.Level;
        pk.Nature = set.Nature;
        pk.Gender = set.Gender ?? (byte)pk.GetSaneGender();

        if (!string.IsNullOrWhiteSpace(set.Nickname))
            pk.Nickname = set.Nickname;
        else
            pk.ClearNickname();

        if (set.Shiny)
            CommonEdits.SetShiny(pk, Shiny.AlwaysStar);

        pk.CurrentFriendship = set.Friendship;

        for (int i = 0; i < 6; i++)
        {
            if (i < set.EVs.Length)
            {
                switch (i)
                {
                    case 0: pk.EV_HP = set.EVs[i]; break;
                    case 1: pk.EV_ATK = set.EVs[i]; break;
                    case 2: pk.EV_DEF = set.EVs[i]; break;
                    case 3: pk.EV_SPE = set.EVs[i]; break;
                    case 4: pk.EV_SPA = set.EVs[i]; break;
                    case 5: pk.EV_SPD = set.EVs[i]; break;
                }
            }

            if (i < set.IVs.Length)
            {
                switch (i)
                {
                    case 0: pk.IV_HP = set.IVs[i]; break;
                    case 1: pk.IV_ATK = set.IVs[i]; break;
                    case 2: pk.IV_DEF = set.IVs[i]; break;
                    case 3: pk.IV_SPE = set.IVs[i]; break;
                    case 4: pk.IV_SPA = set.IVs[i]; break;
                    case 5: pk.IV_SPD = set.IVs[i]; break;
                }
            }
        }

        for (int i = 0; i < 4 && i < set.Moves.Length; i++)
        {
            switch (i)
            {
                case 0: pk.Move1 = set.Moves[i]; break;
                case 1: pk.Move2 = set.Moves[i]; break;
                case 2: pk.Move3 = set.Moves[i]; break;
                case 3: pk.Move4 = set.Moves[i]; break;
            }
        }

        pk.HealPP();
        pk.Heal();
        pk.RefreshChecksum();

        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static ITypes.LegalityResult CheckLegality(uint handle, int box, int slot)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        var analysis = new LegalityAnalysis(pk);
        var errorList = new List<string>();
        var localizer = LegalityLocalizationContext.Create(analysis);

        foreach (var r in analysis.Results)
        {
            if (!r.Valid)
                errorList.Add(localizer.Humanize(r));
        }

        return new ITypes.LegalityResult(
            valid: analysis.Valid,
            errors: errorList,
            report: analysis.Report()
        );
    }

    public static void LegalizePokemon(uint handle, int box, int slot)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        pk.SetMoveset();
        pk.Heal();
        pk.RefreshChecksum();

        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static ITypes.PidInfo GetPidInfo(uint handle, int box, int slot)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        var shinyType = pk.ShinyXor == 0 ? "Square" : pk.IsShiny ? "Star" : "None";
        var gender = pk.Gender;
        var genderName = gender == 0 ? "Male" : gender == 1 ? "Female" : "Genderless";

        return new ITypes.PidInfo(
            pid: pk.PID,
            isShiny: pk.IsShiny,
            shinyType: shinyType,
            nature: (byte)pk.Nature,
            natureName: pk.Nature.ToString(),
            gender: gender,
            genderName: genderName
        );
    }

    public static void GeneratePid(uint handle, int box, int slot, byte nature, bool shiny)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        if (nature > 24)
            throw ImplHelpers.Validation($"Nature {nature} is out of range (0-24)");

        var rnd = new Random();
        var targetNature = (Nature)nature;
        var newPID = EntityPID.GetRandomPID(rnd, pk.Species, pk.Gender, save.Version, targetNature, pk.Form, pk.PID);

        if (shiny)
            newPID = ShinyUtil.GetShinyPID(save.TID16, save.SID16, newPID, 0);

        pk.PID = newPID;
        pk.Nature = targetNature;
        pk.RefreshAbility(pk.AbilityNumber >> 1);
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static void SetPid(uint handle, int box, int slot, uint pid)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        pk.PID = pid;
        pk.RefreshAbility(pk.AbilityNumber >> 1);
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static void SetShiny(uint handle, int box, int slot, byte shinyType)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        var type = (Shiny)shinyType;

        if (type == Shiny.Never)
            pk.SetUnshiny();
        else
            CommonEdits.SetShiny(pk, type);

        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static ITypes.FormData GetForm(uint handle, int box, int slot)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

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

        return new ITypes.FormData(
            form: pk.Form,
            formName: formName,
            formCount: (byte)formNames.Length,
            formArgument: formArgument,
            formArgumentRemain: formArgumentRemain,
            formArgumentElapsed: formArgumentElapsed,
            formArgumentMaximum: formArgumentMaximum
        );
    }

    public static void SetForm(uint handle, int box, int slot, byte form)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        var pi = pk.PersonalInfo;
        var maxForm = pi.FormCount - 1;

        if (form > maxForm)
            throw ImplHelpers.Validation($"Form {form} is out of range (0-{maxForm}) for {GameInfo.Strings.Species[pk.Species]}");

        pk.Form = form;
        pk.RefreshChecksum();
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static void SetFormArgument(uint handle, int box, int slot, uint formArgument)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        if (pk is not IFormArgument formArg)
            throw ImplHelpers.Unsupported("This Pokemon does not support form arguments");

        formArg.FormArgument = formArgument;
        pk.RefreshChecksum();
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static void ChangeSpeciesAndForm(uint handle, int box, int slot, ushort species, byte form)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        if (species < 1 || species > 1025)
            throw ImplHelpers.Validation($"Species {species} is out of range (1-1025)");

        var pt = save.Personal;
        var newPi = pt.GetFormEntry(species, form);
        if (newPi == null || newPi.HP == 0)
            throw ImplHelpers.Validation($"Form {form} not available for species {species} in this game");

        pk.Species = species;
        pk.Form = form;

        if (pk is IFormArgument formArg)
            formArg.FormArgument = 0;

        if (!pk.IsNicknamed)
            pk.ClearNickname();

        pk.RefreshChecksum();
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static ITypes.ContestStats GetContestStats(uint handle, int box, int slot)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        if (pk is not IContestStatsReadOnly contestStats)
            throw ImplHelpers.Unsupported("This Pokemon does not support contest stats");

        return new ITypes.ContestStats(
            cool: contestStats.ContestCool,
            beauty: contestStats.ContestBeauty,
            cute: contestStats.ContestCute,
            smart: contestStats.ContestSmart,
            tough: contestStats.ContestTough,
            sheen: contestStats.ContestSheen
        );
    }

    public static void SetContestStat(uint handle, int box, int slot, string statName, byte value)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        if (string.IsNullOrWhiteSpace(statName))
            throw ImplHelpers.Validation("Stat name cannot be empty");

        if (pk is not IContestStats contestStats)
            throw ImplHelpers.Unsupported("This Pokemon does not support contest stats");

        switch (statName.ToLowerInvariant())
        {
            case "cool":
                contestStats.ContestCool = value;
                break;
            case "beauty":
                contestStats.ContestBeauty = value;
                break;
            case "cute":
                contestStats.ContestCute = value;
                break;
            case "smart":
                contestStats.ContestSmart = value;
                break;
            case "tough":
                contestStats.ContestTough = value;
                break;
            case "sheen":
                contestStats.ContestSheen = value;
                break;
            default:
                throw ImplHelpers.Validation($"Invalid contest stat name: {statName}");
        }

        pk.RefreshChecksum();
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static ITypes.FriendshipData GetFriendship(uint handle, int box, int slot)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        byte? affection = null;
        byte? fullness = null;
        byte? enjoyment = null;

        if (pk is IAffection affectionPk)
            affection = affectionPk.OriginalTrainerAffection;

        if (pk is IFullnessEnjoyment fullnessPk)
        {
            fullness = fullnessPk.Fullness;
            enjoyment = fullnessPk.Enjoyment;
        }

        return new ITypes.FriendshipData(
            current: pk.CurrentFriendship,
            ot: pk.OriginalTrainerFriendship,
            ht: pk.HandlingTrainerFriendship,
            affection: affection,
            fullness: fullness,
            enjoyment: enjoyment
        );
    }

    public static void SetFriendship(uint handle, int box, int slot, byte value)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        pk.CurrentFriendship = Math.Clamp(value, (byte)0, (byte)255);
        pk.RefreshChecksum();
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static void SetOtFriendship(uint handle, int box, int slot, byte value)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        pk.OriginalTrainerFriendship = Math.Clamp(value, (byte)0, (byte)255);
        pk.RefreshChecksum();
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static void SetHtFriendship(uint handle, int box, int slot, byte value)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        pk.HandlingTrainerFriendship = Math.Clamp(value, (byte)0, (byte)255);
        pk.RefreshChecksum();
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static void SetAffection(uint handle, int box, int slot, byte value)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        if (pk is not IAffection affectionPk)
            throw ImplHelpers.Unsupported("This Pokemon does not support affection (Gen 6-7 only)");

        affectionPk.OriginalTrainerAffection = value;
        pk.RefreshChecksum();
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static void SetFullness(uint handle, int box, int slot, byte value)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        if (pk is not IFullnessEnjoyment fullnessPk)
            throw ImplHelpers.Unsupported("This Pokemon does not support fullness");

        fullnessPk.Fullness = value;
        pk.RefreshChecksum();
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static void SetEnjoyment(uint handle, int box, int slot, byte value)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        if (pk is not IFullnessEnjoyment enjoymentPk)
            throw ImplHelpers.Unsupported("This Pokemon does not support enjoyment");

        enjoymentPk.Enjoyment = value;
        pk.RefreshChecksum();
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static void MaximizeFriendship(uint handle, int box, int slot)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        pk.CurrentFriendship = 255;
        pk.OriginalTrainerFriendship = 255;
        pk.HandlingTrainerFriendship = 255;

        if (pk is IAffection affectionPk)
        {
            affectionPk.OriginalTrainerAffection = 255;
            affectionPk.HandlingTrainerAffection = 255;
        }

        if (pk is IFullnessEnjoyment fullnessPk)
        {
            fullnessPk.Fullness = 255;
            fullnessPk.Enjoyment = 255;
        }

        pk.RefreshChecksum();
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static List<ITypes.RibbonEntry> GetRibbons(uint handle, int box, int slot)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        var ribbonInfo = RibbonInfo.GetRibbonInfo(pk);
        var ribbonList = new List<ITypes.RibbonEntry>();

        foreach (var r in ribbonInfo)
        {
            ribbonList.Add(new ITypes.RibbonEntry(
                name: r.Name,
                hasRibbon: r.HasRibbon,
                ribbonCount: r.RibbonCount,
                ribbonType: r.Type.ToString()
            ));
        }

        return ribbonList;
    }

    public static int GetRibbonCount(uint handle, int box, int slot)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        var ribbonInfo = RibbonInfo.GetRibbonInfo(pk);
        return ribbonInfo.Count(r => r.HasRibbon || r.RibbonCount > 0);
    }

    public static void SetRibbon(uint handle, int box, int slot, string ribbonName, bool value)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        if (string.IsNullOrWhiteSpace(ribbonName))
            throw ImplHelpers.Validation("Ribbon name cannot be empty");

        var property = pk.GetType().GetProperty(ribbonName);
        if (property == null)
            throw ImplHelpers.Validation($"Ribbon '{ribbonName}' not found on this Pokemon");

        if (property.PropertyType == typeof(bool))
            property.SetValue(pk, value);
        else if (property.PropertyType == typeof(byte))
            property.SetValue(pk, value ? (byte)1 : (byte)0);
        else
            throw ImplHelpers.Validation($"Ribbon '{ribbonName}' has unsupported type");

        pk.RefreshChecksum();
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static ITypes.MemoriesData GetMemories(uint handle, int box, int slot)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        if (pk is not IMemoryOT otMem)
            throw ImplHelpers.Unsupported("This Pokemon does not support memories (Gen 6+ only)");

        var otMemory = new ITypes.MemoryInfo(
            memoryId: otMem.OriginalTrainerMemory,
            intensity: otMem.OriginalTrainerMemoryIntensity,
            feeling: otMem.OriginalTrainerMemoryFeeling,
            variable: otMem.OriginalTrainerMemoryVariable,
            text: GetMemoryText(otMem.OriginalTrainerMemory)
        );

        ITypes.MemoryInfo? htMemory = null;
        if (pk is IMemoryHT htMem)
        {
            htMemory = new ITypes.MemoryInfo(
                memoryId: htMem.HandlingTrainerMemory,
                intensity: htMem.HandlingTrainerMemoryIntensity,
                feeling: htMem.HandlingTrainerMemoryFeeling,
                variable: htMem.HandlingTrainerMemoryVariable,
                text: GetMemoryText(htMem.HandlingTrainerMemory)
            );
        }

        return new ITypes.MemoriesData(
            otMemory: otMemory,
            htMemory: htMemory
        );
    }

    public static void SetOtMemory(uint handle, int box, int slot, byte memoryId, byte intensity, byte feeling, ushort variable)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        if (pk is not IMemoryOT otMem)
            throw ImplHelpers.Unsupported("This Pokemon does not support memories (Gen 6+ only)");

        otMem.OriginalTrainerMemory = memoryId;
        otMem.OriginalTrainerMemoryIntensity = Math.Clamp(intensity, (byte)0, (byte)7);
        otMem.OriginalTrainerMemoryFeeling = Math.Clamp(feeling, (byte)0, (byte)24);
        otMem.OriginalTrainerMemoryVariable = variable;

        pk.RefreshChecksum();
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static void SetHtMemory(uint handle, int box, int slot, byte memoryId, byte intensity, byte feeling, ushort variable)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        if (pk is not IMemoryHT htMem)
            throw ImplHelpers.Unsupported("This Pokemon does not support handling trainer memories (Gen 6+ only)");

        htMem.HandlingTrainerMemory = memoryId;
        htMem.HandlingTrainerMemoryIntensity = Math.Clamp(intensity, (byte)0, (byte)7);
        htMem.HandlingTrainerMemoryFeeling = Math.Clamp(feeling, (byte)0, (byte)24);
        htMem.HandlingTrainerMemoryVariable = variable;

        pk.RefreshChecksum();
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static void ClearMemories(uint handle, int box, int slot)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        if (pk is IMemoryOT otMem)
        {
            otMem.OriginalTrainerMemory = 0;
            otMem.OriginalTrainerMemoryIntensity = 0;
            otMem.OriginalTrainerMemoryFeeling = 0;
            otMem.OriginalTrainerMemoryVariable = 0;
        }

        if (pk is IMemoryHT htMem)
        {
            htMem.HandlingTrainerMemory = 0;
            htMem.HandlingTrainerMemoryIntensity = 0;
            htMem.HandlingTrainerMemoryFeeling = 0;
            htMem.HandlingTrainerMemoryVariable = 0;
        }

        pk.RefreshChecksum();
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static ITypes.TeraTypeData GetTeraType(uint handle, int box, int slot)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        if (pk is not ITeraTypeReadOnly teraPk)
            throw ImplHelpers.Unsupported("This Pokemon does not support Tera Type (Gen 9 only)");

        var teraType = teraPk.TeraType;
        var teraTypeEffective = pk is ITeraType teraFull ? teraFull.GetTeraType() : teraType;
        var isOverridden = teraType != teraTypeEffective;

        return new ITypes.TeraTypeData(
            teraType: (byte)teraType,
            teraTypeName: GetTeraTypeName(teraType),
            effectiveTeraType: (byte)teraTypeEffective,
            effectiveTeraTypeName: GetTeraTypeName(teraTypeEffective),
            isOverridden: isOverridden
        );
    }

    public static void SetTeraType(uint handle, int box, int slot, byte teraType)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        if (pk is not ITeraType teraPk)
            throw ImplHelpers.Unsupported("This Pokemon does not support Tera Type (Gen 9 only)");

        if (teraType > 17 && teraType != TeraTypeUtil.Stellar)
            throw ImplHelpers.Validation($"Tera Type {teraType} is out of range (0-17 or 99 for Stellar)");

        teraPk.SetTeraType((MoveType)teraType);
        pk.RefreshChecksum();
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    public static void ResetTeraType(uint handle, int box, int slot)
    {
        var save = ImplHelpers.GetSave(handle);
        var pk = ImplHelpers.GetPokemon(save, box, slot);

        if (pk is not ITeraType teraPk)
            throw ImplHelpers.Unsupported("This Pokemon does not support Tera Type (Gen 9 only)");

        teraPk.TeraTypeOverride = (MoveType)TeraTypeUtil.OverrideNone;
        pk.RefreshChecksum();
        save.SetBoxSlotAtIndex(pk, box, slot);
    }

    private static string GetMemoryText(int memoryId)
    {
        if (memoryId == 0)
            return string.Empty;

        var memories = GameInfo.Strings.memories;
        if (memoryId < memories.Length)
            return memories[memoryId];

        return $"Memory {memoryId}";
    }

    private static string GetTeraTypeName(MoveType type)
    {
        var typeIndex = (int)type;
        if (typeIndex == TeraTypeUtil.Stellar)
            return "Stellar";

        var typeNames = GameInfo.Strings.Types;
        if (typeIndex >= 0 && typeIndex < typeNames.Count)
            return typeNames[typeIndex];

        return $"Type {typeIndex}";
    }
}
