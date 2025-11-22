using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;
using static PKHeX.Models.ErrorCodes;

namespace PKHeX.Api;

// Static Game Data Operations
public partial class PKHeXApi
{
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetSpeciesName(int speciesId)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (speciesId < 0 || speciesId >= GameInfo.Strings.Species.Count)
                throw new ValidationException($"Species ID {speciesId} is out of range (0-{GameInfo.Strings.Species.Count - 1})", "INVALID_SPECIES_ID");

            var name = GameInfo.Strings.Species[speciesId];
            return new NameResponse(true, name);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetAllSpecies()
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            var speciesList = new List<NamedEntity>();
            var species = GameInfo.Strings.Species;

            for (int i = 0; i < species.Count; i++)
            {
                speciesList.Add(new NamedEntity(i, species[i]));
            }

            return new SpeciesListResponse(true, speciesList);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetMoveName(int moveId)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (moveId < 0 || moveId >= GameInfo.Strings.Move.Count)
                throw new ValidationException($"Move ID {moveId} is out of range (0-{GameInfo.Strings.Move.Count - 1})", "INVALID_MOVE_ID");

            var name = GameInfo.Strings.Move[moveId];
            return new NameResponse(true, name);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetAllMoves()
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            var movesList = new List<NamedEntity>();
            var moves = GameInfo.Strings.Move;

            for (int i = 0; i < moves.Count; i++)
            {
                movesList.Add(new NamedEntity(i, moves[i]));
            }

            return new MovesListResponse(true, movesList);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetAbilityName(int abilityId)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (abilityId < 0 || abilityId >= GameInfo.Strings.Ability.Count)
                throw new ValidationException($"Ability ID {abilityId} is out of range (0-{GameInfo.Strings.Ability.Count - 1})", "INVALID_ABILITY_ID");

            var name = GameInfo.Strings.Ability[abilityId];
            return new NameResponse(true, name);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetAllAbilities()
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            var abilitiesList = new List<NamedEntity>();
            var abilities = GameInfo.Strings.Ability;

            for (int i = 0; i < abilities.Count; i++)
            {
                abilitiesList.Add(new NamedEntity(i, abilities[i]));
            }

            return new AbilitiesListResponse(true, abilitiesList);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetItemName(int itemId)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (itemId < 0 || itemId >= GameInfo.Strings.Item.Count)
                throw new ValidationException($"Item ID {itemId} is out of range (0-{GameInfo.Strings.Item.Count - 1})", "INVALID_ITEM_ID");

            var name = GameInfo.Strings.Item[itemId];
            return new NameResponse(true, name);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetAllItems()
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            var itemsList = new List<NamedEntity>();
            var items = GameInfo.Strings.Item;

            for (int i = 0; i < items.Count; i++)
            {
                itemsList.Add(new NamedEntity(i, items[i]));
            }

            return new ItemsListResponse(true, itemsList);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetNatureName(int natureId)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (natureId < 0 || natureId >= GameInfo.Strings.Natures.Count)
                throw new ValidationException($"Nature ID {natureId} is out of range (0-{GameInfo.Strings.Natures.Count - 1})", "INVALID_NATURE_ID");

            var name = GameInfo.Strings.Natures[natureId];
            return new NameResponse(true, name);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetAllNatures()
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            var naturesList = new List<NamedEntity>();
            var natures = GameInfo.Strings.Natures;

            for (int i = 0; i < natures.Count; i++)
            {
                naturesList.Add(new NamedEntity(i, natures[i]));
            }

            return new NaturesListResponse(true, naturesList);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetTypeName(int typeId)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (typeId < 0 || typeId >= GameInfo.Strings.Types.Count)
                throw new ValidationException($"Type ID {typeId} is out of range (0-{GameInfo.Strings.Types.Count - 1})", "INVALID_TYPE_ID");

            var name = GameInfo.Strings.Types[typeId];
            return new NameResponse(true, name);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetAllTypes()
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            var typesList = new List<NamedEntity>();
            var types = GameInfo.Strings.Types;

            for (int i = 0; i < types.Count; i++)
            {
                typesList.Add(new NamedEntity(i, types[i]));
            }

            return new TypesListResponse(true, typesList);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.Any>]
    public static object GetSpeciesEvolutions(int species, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            ApiHelpers.ValidateNonNegative(species, nameof(species), INVALID_SPECIES);
            ApiHelpers.ValidateNonNegative(generation, nameof(generation), "INVALID_GENERATION");

            if (species < 1 || species > 1025)
                throw new ValidationException($"Species {species} is out of range (1-1025)", INVALID_SPECIES);

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var context = (EntityContext)generation;
            var evolutions = EvolutionTree.GetEvolutionTree(context);

            var chain = evolutions.GetEvolutionsAndPreEvolutions((ushort)species, 0);
            var evolutionChain = new List<object>();

            foreach (var (evoSpecies, evoForm) in chain)
            {
                evolutionChain.Add(new
                {
                    species = evoSpecies,
                    speciesName = GameInfo.Strings.Species[evoSpecies],
                    form = evoForm
                });
            }

            var forwardEvolutions = new List<object>();
            var forward = evolutions.Forward.GetEvolutions((ushort)species, 0);
            foreach (var (evoSpecies, evoForm) in forward)
            {
                forwardEvolutions.Add(new
                {
                    species = evoSpecies,
                    speciesName = GameInfo.Strings.Species[evoSpecies],
                    form = evoForm
                });
            }

            var preEvolutions = new List<object>();
            var reverse = evolutions.Reverse.GetPreEvolutions((ushort)species, 0);
            foreach (var (evoSpecies, evoForm) in reverse)
            {
                preEvolutions.Add(new
                {
                    species = evoSpecies,
                    speciesName = GameInfo.Strings.Species[evoSpecies],
                    form = evoForm
                });
            }

            var baseSpeciesForm = evolutions.GetBaseSpeciesForm((ushort)species, 0);

            return new
            {
                species,
                speciesName = GameInfo.Strings.Species[species],
                generation,
                evolutionChain,
                chainLength = evolutionChain.Count,
                forwardEvolutions,
                preEvolutions,
                baseSpecies = baseSpeciesForm.Species,
                baseSpeciesName = GameInfo.Strings.Species[baseSpeciesForm.Species],
                baseForm = baseSpeciesForm.Form
            };
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.Any>]
    public static object GetSpeciesForms(int species, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            ApiHelpers.ValidateNonNegative(species, nameof(species), INVALID_SPECIES);
            ApiHelpers.ValidateNonNegative(generation, nameof(generation), "INVALID_GENERATION");

            if (species < 1 || species > 1025)
                throw new ValidationException($"Species {species} is out of range (1-1025)", INVALID_SPECIES);

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

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
                _ => throw new ValidationException($"Invalid generation {generation}", "INVALID_GENERATION")
            };

            var forms = new List<object>();

            var pi = pt.GetFormEntry((ushort)species, 0);
            var formCount = pi.FormCount;

            for (byte i = 0; i < formCount; i++)
            {
                var formEntry = pt.GetFormEntry((ushort)species, i);

                forms.Add(new
                {
                    formIndex = i,
                    formName = $"Form {i}",
                    type1 = formEntry.Type1,
                    type1Name = GameInfo.Strings.Types[formEntry.Type1],
                    type2 = formEntry.Type2,
                    type2Name = GameInfo.Strings.Types[formEntry.Type2],
                    baseStats = new
                    {
                        hp = formEntry.HP,
                        attack = formEntry.ATK,
                        defense = formEntry.DEF,
                        spAttack = formEntry.SPA,
                        spDefense = formEntry.SPD,
                        speed = formEntry.SPE
                    },
                    genderRatio = formEntry.Gender,
                    isDualGender = formEntry.IsDualGender,
                    isGenderless = formEntry.Genderless
                });
            }

            return new
            {
                species,
                speciesName = GameInfo.Strings.Species[species],
                generation,
                forms,
                formCount
            };
        });
    }
}
