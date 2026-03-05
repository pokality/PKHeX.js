using PKHeX.Core;
using PkhexWorld;
using PkhexWorld.wit.exports.pokality.pkhex;

namespace PkhexWorld.wit.exports.pokality.pkhex;

public class GameDataOpsImpl : IGameDataOps
{
    public static string GetSpeciesName(ushort speciesId)
    {
        var species = GameInfo.Strings.Species;
        if (speciesId >= species.Count)
            throw ImplHelpers.Validation($"Species ID {speciesId} is out of range (0-{species.Count - 1})");

        return species[speciesId];
    }

    public static List<ITypes.NamedEntry> GetAllSpecies()
    {
        var species = GameInfo.Strings.Species;
        var result = new List<ITypes.NamedEntry>(species.Count);
        for (int i = 0; i < species.Count; i++)
            result.Add(new ITypes.NamedEntry(i, species[i]));
        return result;
    }

    public static string GetMoveName(ushort moveId)
    {
        var moves = GameInfo.Strings.Move;
        if (moveId >= moves.Count)
            throw ImplHelpers.Validation($"Move ID {moveId} is out of range (0-{moves.Count - 1})");

        return moves[moveId];
    }

    public static List<ITypes.NamedEntry> GetAllMoves()
    {
        var moves = GameInfo.Strings.Move;
        var result = new List<ITypes.NamedEntry>(moves.Count);
        for (int i = 0; i < moves.Count; i++)
            result.Add(new ITypes.NamedEntry(i, moves[i]));
        return result;
    }

    public static string GetAbilityName(ushort abilityId)
    {
        var abilities = GameInfo.Strings.Ability;
        if (abilityId >= abilities.Count)
            throw ImplHelpers.Validation($"Ability ID {abilityId} is out of range (0-{abilities.Count - 1})");

        return abilities[abilityId];
    }

    public static List<ITypes.NamedEntry> GetAllAbilities()
    {
        var abilities = GameInfo.Strings.Ability;
        var result = new List<ITypes.NamedEntry>(abilities.Count);
        for (int i = 0; i < abilities.Count; i++)
            result.Add(new ITypes.NamedEntry(i, abilities[i]));
        return result;
    }

    public static string GetItemName(ushort itemId)
    {
        var items = GameInfo.Strings.Item;
        if (itemId >= items.Count)
            throw ImplHelpers.Validation($"Item ID {itemId} is out of range (0-{items.Count - 1})");

        return items[itemId];
    }

    public static List<ITypes.NamedEntry> GetAllItems()
    {
        var items = GameInfo.Strings.Item;
        var result = new List<ITypes.NamedEntry>(items.Count);
        for (int i = 0; i < items.Count; i++)
            result.Add(new ITypes.NamedEntry(i, items[i]));
        return result;
    }

    public static string GetNatureName(byte natureId)
    {
        var natures = GameInfo.Strings.Natures;
        if (natureId >= natures.Count)
            throw ImplHelpers.Validation($"Nature ID {natureId} is out of range (0-{natures.Count - 1})");

        return natures[natureId];
    }

    public static List<ITypes.NamedEntry> GetAllNatures()
    {
        var natures = GameInfo.Strings.Natures;
        var result = new List<ITypes.NamedEntry>(natures.Count);
        for (int i = 0; i < natures.Count; i++)
            result.Add(new ITypes.NamedEntry(i, natures[i]));
        return result;
    }

    public static string GetTypeName(byte typeId)
    {
        var types = GameInfo.Strings.Types;
        if (typeId >= types.Count)
            throw ImplHelpers.Validation($"Type ID {typeId} is out of range (0-{types.Count - 1})");

        return types[typeId];
    }

    public static List<ITypes.NamedEntry> GetAllTypes()
    {
        var types = GameInfo.Strings.Types;
        var result = new List<ITypes.NamedEntry>(types.Count);
        for (int i = 0; i < types.Count; i++)
            result.Add(new ITypes.NamedEntry(i, types[i]));
        return result;
    }

    public static IGameDataOps.SpeciesCategory GetSpeciesCategory(ushort species)
    {
        var speciesStrings = GameInfo.Strings.Species;
        if (species >= speciesStrings.Count)
            throw ImplHelpers.Validation($"Species ID {species} is out of range (0-{speciesStrings.Count - 1})");

        return new IGameDataOps.SpeciesCategory(
            species,
            speciesStrings[species],
            PKHeX.Core.SpeciesCategory.IsLegendary(species),
            PKHeX.Core.SpeciesCategory.IsSubLegendary(species),
            PKHeX.Core.SpeciesCategory.IsMythical(species),
            PKHeX.Core.SpeciesCategory.IsUltraBeast(species),
            PKHeX.Core.SpeciesCategory.IsParadox(species),
            PKHeX.Core.SpeciesCategory.IsSpecialPokemon(species)
        );
    }

    public static bool IsPrimalForm(ushort species, byte form)
    {
        return PKHeX.Core.FormInfo.IsPrimalForm(species, form);
    }

    public static IGameDataOps.EvolutionData GetSpeciesEvolutions(ushort species, byte generation)
    {
        var context = (EntityContext)generation;
        var evolutions = EvolutionTree.GetEvolutionTree(context);

        var chain = evolutions.GetEvolutionsAndPreEvolutions(species, 0);
        var chainList = new List<IGameDataOps.EvolutionEntry>();
        foreach (var (evoSpecies, evoForm) in chain)
        {
            chainList.Add(new IGameDataOps.EvolutionEntry(
                evoSpecies,
                GameInfo.Strings.Species[evoSpecies],
                evoForm
            ));
        }

        var forward = evolutions.Forward.GetEvolutions(species, 0);
        var forwardList = new List<IGameDataOps.EvolutionEntry>();
        foreach (var (evoSpecies, evoForm) in forward)
        {
            forwardList.Add(new IGameDataOps.EvolutionEntry(
                evoSpecies,
                GameInfo.Strings.Species[evoSpecies],
                evoForm
            ));
        }

        var reverse = evolutions.Reverse.GetPreEvolutions(species, 0);
        var reverseList = new List<IGameDataOps.EvolutionEntry>();
        foreach (var (evoSpecies, evoForm) in reverse)
        {
            reverseList.Add(new IGameDataOps.EvolutionEntry(
                evoSpecies,
                GameInfo.Strings.Species[evoSpecies],
                evoForm
            ));
        }

        var baseSpeciesForm = evolutions.GetBaseSpeciesForm(species, 0);

        return new IGameDataOps.EvolutionData(
            species,
            GameInfo.Strings.Species[species],
            chainList,
            forwardList,
            reverseList,
            baseSpeciesForm.Species,
            GameInfo.Strings.Species[baseSpeciesForm.Species],
            baseSpeciesForm.Form
        );
    }

    public static IGameDataOps.SpeciesForms GetSpeciesForms(ushort species, byte generation)
    {
        var context = (EntityContext)generation;
        var pt = context switch
        {
            EntityContext.Gen1 => (IPersonalTable)PersonalTable.RB,
            EntityContext.Gen2 => PersonalTable.C,
            EntityContext.Gen3 => PersonalTable.E,
            EntityContext.Gen4 => PersonalTable.HGSS,
            EntityContext.Gen5 => PersonalTable.B2W2,
            EntityContext.Gen6 => PersonalTable.AO,
            EntityContext.Gen7 => PersonalTable.USUM,
            EntityContext.Gen8 => PersonalTable.SWSH,
            EntityContext.Gen9 => PersonalTable.SV,
            _ => throw ImplHelpers.Validation($"Invalid generation {generation}")
        };

        var pi = pt.GetFormEntry(species, 0);
        var formCount = pi.FormCount;
        var forms = new List<IGameDataOps.FormEntry>(formCount);

        for (byte i = 0; i < formCount; i++)
        {
            var formEntry = pt.GetFormEntry(species, i);
            forms.Add(new IGameDataOps.FormEntry(
                i,
                $"Form {i}",
                formEntry.Type1,
                GameInfo.Strings.Types[formEntry.Type1],
                formEntry.Type2,
                GameInfo.Strings.Types[formEntry.Type2],
                new ITypes.StatSpread(
                    formEntry.HP, formEntry.ATK, formEntry.DEF,
                    formEntry.SPA, formEntry.SPD, formEntry.SPE
                ),
                formEntry.Gender,
                formEntry.IsDualGender,
                formEntry.Genderless
            ));
        }

        return new IGameDataOps.SpeciesForms(
            species,
            GameInfo.Strings.Species[species],
            generation,
            forms,
            (byte)formCount
        );
    }

    public static List<IGameDataOps.AvailableForm> GetAvailableForms(ushort species, byte generation)
    {
        var context = (EntityContext)generation;
        var formNames = FormConverter.GetFormList(
            species, GameInfo.Strings.Types, GameInfo.Strings.forms,
            Array.Empty<string>(), context
        );

        var result = new List<IGameDataOps.AvailableForm>(formNames.Length);
        for (int i = 0; i < formNames.Length; i++)
        {
            var name = string.IsNullOrEmpty(formNames[i])
                ? (i == 0 ? "Normal" : $"Form {i}")
                : formNames[i];
            result.Add(new IGameDataOps.AvailableForm((byte)i, name));
        }
        return result;
    }

    public static List<IGameDataOps.TeraTypeInfo> GetAllTeraTypes()
    {
        var typeNames = GameInfo.Strings.Types;
        var result = new List<IGameDataOps.TeraTypeInfo>(19);

        for (int i = 0; i < 18 && i < typeNames.Count; i++)
            result.Add(new IGameDataOps.TeraTypeInfo((byte)i, typeNames[i], false));

        result.Add(new IGameDataOps.TeraTypeInfo((byte)TeraTypeUtil.Stellar, "Stellar", true));

        return result;
    }

    public static IGameDataOps.MemoryStrings GetMemoryStrings()
    {
        var memStrings = new MemoryStrings(GameInfo.Strings);
        var memories = GameInfo.Strings.memories;
        var feelings = memStrings.GetMemoryFeelings(8);
        var intensities = memStrings.GetMemoryQualities();

        var memoryList = new List<ITypes.NamedEntry>();
        for (int i = 0; i < memories.Length; i++)
        {
            if (!string.IsNullOrEmpty(memories[i]))
                memoryList.Add(new ITypes.NamedEntry(i, memories[i]));
        }

        var feelingList = new List<ITypes.NamedEntry>();
        for (int i = 0; i < feelings.Length; i++)
        {
            if (!string.IsNullOrEmpty(feelings[i]))
                feelingList.Add(new ITypes.NamedEntry(i, feelings[i]));
        }

        var intensityList = new List<ITypes.NamedEntry>();
        for (int i = 0; i < intensities.Length; i++)
        {
            if (!string.IsNullOrEmpty(intensities[i]))
                intensityList.Add(new ITypes.NamedEntry(i, intensities[i]));
        }

        return new IGameDataOps.MemoryStrings(memoryList, feelingList, intensityList);
    }
}
