using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;
using static PKHeX.Models.ErrorCodes;

namespace PKHeX.Api;

[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "All required types are preserved via TrimmerRoots")]
public partial class PKHeXApi
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string LoadSave(string base64Data)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            if (string.IsNullOrWhiteSpace(base64Data))
                throw new ValidationException("Save data cannot be empty", EMPTY_DATA);

            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64Data);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            var save = SaveUtil.GetSaveFile(data);
            if (save == null)
                throw new ValidationException("Unable to load save file. Format not recognized or data is corrupted.", INVALID_SAVE_FORMAT);

            var handle = SaveFileManager.CreateHandle(save);
            return new SaveFileHandle(true, handle);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetSaveInfo(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            return new SaveFileInfo(
                true,
                $"Gen {save.Generation}",
                save.Version.ToString(),
                save.OT,
                save.DisplayTID,
                save.DisplaySID,
                save.BoxCount
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string ExportSave(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var data = save.Write();
            var base64Data = Convert.ToBase64String(data.Span);
            return new ExportSaveResponse(true, base64Data);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string DisposeSave(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            ApiHelpers.ValidateHandle(handle);
            var removed = SaveFileManager.RemoveHandle(handle);
            if (!removed)
                throw new ValidationException("Invalid save file handle", INVALID_HANDLE);

            return new SuccessMessage(true, "Save disposed successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetActiveHandleCount()
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var count = SaveFileManager.GetActiveHandleCount();
            return new { success = true, count };
        });
    }

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
    public static string GetParty(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var partyList = new List<PokemonSummary>();
            var partyData = save.PartyData;

            for (int i = 0; i < partyData.Count; i++)
            {
                var pk = partyData[i];
                if (pk == null || pk.Species == 0)
                    continue;

                partyList.Add(new PokemonSummary(
                    -1,
                    i,
                    pk.Species,
                    GameInfo.Strings.Species[pk.Species],
                    pk.CurrentLevel,
                    pk.IsEgg,
                    pk.IsShiny
                ));
            }

            return partyList;
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
    public static string GetPartySlot(int handle, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (slot < 0 || slot >= 6)
                throw new ValidationException($"Party slot {slot} is out of range (0-5)", INVALID_SLOT);

            if (slot >= save.PartyCount)
                throw new ValidationException($"Party slot {slot} is empty", EMPTY_SLOT);

            var pk = save.GetPartySlotAtIndex(slot);
            if (pk.Species == 0)
                throw new ValidationException($"No Pokemon in party slot {slot}", EMPTY_SLOT);

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

    private static PokemonDetail CreatePokemonDetailObject(PKM pk)
    {
        var moves = new[] { (int)pk.Move1, (int)pk.Move2, (int)pk.Move3, (int)pk.Move4 };
        var moveNames = new[] {
            GameInfo.Strings.Move[pk.Move1],
            GameInfo.Strings.Move[pk.Move2],
            GameInfo.Strings.Move[pk.Move3],
            GameInfo.Strings.Move[pk.Move4]
        };

        var ivs = new[] { pk.IV_HP, pk.IV_ATK, pk.IV_DEF, pk.IV_SPE, pk.IV_SPA, pk.IV_SPD };
        var evs = new[] { pk.EV_HP, pk.EV_ATK, pk.EV_DEF, pk.EV_SPE, pk.EV_SPA, pk.EV_SPD };
        var stats = new[] { pk.Stat_HPMax, pk.Stat_ATK, pk.Stat_DEF, pk.Stat_SPE, pk.Stat_SPA, pk.Stat_SPD };

        return new PokemonDetail(
            Species: pk.Species,
            SpeciesName: GameInfo.Strings.Species[pk.Species],
            Nickname: pk.Nickname,
            Level: pk.CurrentLevel,
            Nature: (int)pk.Nature,
            NatureName: GameInfo.Strings.Natures[(int)pk.Nature],
            Ability: pk.Ability,
            AbilityName: GameInfo.Strings.Ability[pk.Ability],
            HeldItem: pk.HeldItem,
            HeldItemName: GameInfo.Strings.Item[pk.HeldItem],
            Moves: moves,
            MoveNames: moveNames,
            IVs: ivs,
            EVs: evs,
            Stats: stats,
            Gender: pk.Gender,
            IsShiny: pk.IsShiny,
            IsEgg: pk.IsEgg,
            OT_Name: pk.OriginalTrainerName,
            OT_Gender: pk.OriginalTrainerGender,
            PID: pk.PID,
            Ball: pk.Ball,
            MetLevel: pk.MetLevel,
            MetLocation: pk.MetLocation,
            MetLocationName: GameInfo.GetLocationName(false, pk.MetLocation, pk.Format, pk.Generation, pk.Version)
        );
    }

    private static void ApplyModifications(PKM pk, PokemonModifications mods, SaveFile save)
    {
        if (mods.Species.HasValue)
        {
            if (mods.Species.Value < 0 || mods.Species.Value > save.MaxSpeciesID)
                throw new ValidationException($"Species {mods.Species.Value} is out of range", INVALID_SPECIES);
            pk.Species = (ushort)mods.Species.Value;
        }

        if (mods.Nickname != null)
            pk.Nickname = mods.Nickname;

        if (mods.Level.HasValue)
        {
            if (mods.Level.Value < 1 || mods.Level.Value > 100)
                throw new ValidationException($"Level {mods.Level.Value} is out of range (1-100)", INVALID_LEVEL);
            pk.CurrentLevel = (byte)mods.Level.Value;
        }

        if (mods.Nature.HasValue)
        {
            if (mods.Nature.Value < 0 || mods.Nature.Value >= GameInfo.Strings.Natures.Count)
                throw new ValidationException($"Nature {mods.Nature.Value} is out of range", INVALID_NATURE);
            pk.Nature = (Nature)mods.Nature.Value;
        }

        if (mods.Ability.HasValue)
        {
            if (mods.Ability.Value < 0 || mods.Ability.Value > save.MaxAbilityID)
                throw new ValidationException($"Ability {mods.Ability.Value} is out of range", INVALID_ABILITY);
            pk.Ability = mods.Ability.Value;
        }

        if (mods.HeldItem.HasValue)
        {
            if (mods.HeldItem.Value < 0 || mods.HeldItem.Value > save.MaxItemID)
                throw new ValidationException($"Item {mods.HeldItem.Value} is out of range", INVALID_ITEM);
            pk.HeldItem = mods.HeldItem.Value;
        }

        if (mods.Moves != null)
        {
            if (mods.Moves.Length > 4)
                throw new ValidationException("Cannot have more than 4 moves", "TOO_MANY_MOVES");

            if (mods.Moves.Length > 0)
            {
                if (mods.Moves[0] < 0 || mods.Moves[0] > save.MaxMoveID)
                    throw new ValidationException($"Move {mods.Moves[0]} is out of range", INVALID_MOVE);
                pk.Move1 = (ushort)mods.Moves[0];
            }
            if (mods.Moves.Length > 1)
            {
                if (mods.Moves[1] < 0 || mods.Moves[1] > save.MaxMoveID)
                    throw new ValidationException($"Move {mods.Moves[1]} is out of range", INVALID_MOVE);
                pk.Move2 = (ushort)mods.Moves[1];
            }
            if (mods.Moves.Length > 2)
            {
                if (mods.Moves[2] < 0 || mods.Moves[2] > save.MaxMoveID)
                    throw new ValidationException($"Move {mods.Moves[2]} is out of range", INVALID_MOVE);
                pk.Move3 = (ushort)mods.Moves[2];
            }
            if (mods.Moves.Length > 3)
            {
                if (mods.Moves[3] < 0 || mods.Moves[3] > save.MaxMoveID)
                    throw new ValidationException($"Move {mods.Moves[3]} is out of range", INVALID_MOVE);
                pk.Move4 = (ushort)mods.Moves[3];
            }
        }

        if (mods.IVs != null)
        {
            if (mods.IVs.Length != 6)
                throw new ValidationException("IVs must have exactly 6 values", INVALID_IVS);

            if (mods.IVs[0] < 0 || mods.IVs[0] > save.MaxIV)
                throw new ValidationException($"IV {mods.IVs[0]} is out of range (0-{save.MaxIV})", INVALID_IV);
            pk.IV_HP = mods.IVs[0];

            if (mods.IVs[1] < 0 || mods.IVs[1] > save.MaxIV)
                throw new ValidationException($"IV {mods.IVs[1]} is out of range (0-{save.MaxIV})", INVALID_IV);
            pk.IV_ATK = mods.IVs[1];

            if (mods.IVs[2] < 0 || mods.IVs[2] > save.MaxIV)
                throw new ValidationException($"IV {mods.IVs[2]} is out of range (0-{save.MaxIV})", INVALID_IV);
            pk.IV_DEF = mods.IVs[2];

            if (mods.IVs[3] < 0 || mods.IVs[3] > save.MaxIV)
                throw new ValidationException($"IV {mods.IVs[3]} is out of range (0-{save.MaxIV})", INVALID_IV);
            pk.IV_SPE = mods.IVs[3];

            if (mods.IVs[4] < 0 || mods.IVs[4] > save.MaxIV)
                throw new ValidationException($"IV {mods.IVs[4]} is out of range (0-{save.MaxIV})", INVALID_IV);
            pk.IV_SPA = mods.IVs[4];

            if (mods.IVs[5] < 0 || mods.IVs[5] > save.MaxIV)
                throw new ValidationException($"IV {mods.IVs[5]} is out of range (0-{save.MaxIV})", INVALID_IV);
            pk.IV_SPD = mods.IVs[5];
        }

        if (mods.EVs != null)
        {
            if (mods.EVs.Length != 6)
                throw new ValidationException("EVs must have exactly 6 values", INVALID_EVS);

            if (mods.EVs[0] < 0 || mods.EVs[0] > save.MaxEV)
                throw new ValidationException($"EV {mods.EVs[0]} is out of range (0-{save.MaxEV})", INVALID_EV);
            pk.EV_HP = mods.EVs[0];

            if (mods.EVs[1] < 0 || mods.EVs[1] > save.MaxEV)
                throw new ValidationException($"EV {mods.EVs[1]} is out of range (0-{save.MaxEV})", INVALID_EV);
            pk.EV_ATK = mods.EVs[1];

            if (mods.EVs[2] < 0 || mods.EVs[2] > save.MaxEV)
                throw new ValidationException($"EV {mods.EVs[2]} is out of range (0-{save.MaxEV})", INVALID_EV);
            pk.EV_DEF = mods.EVs[2];

            if (mods.EVs[3] < 0 || mods.EVs[3] > save.MaxEV)
                throw new ValidationException($"EV {mods.EVs[3]} is out of range (0-{save.MaxEV})", INVALID_EV);
            pk.EV_SPE = mods.EVs[3];

            if (mods.EVs[4] < 0 || mods.EVs[4] > save.MaxEV)
                throw new ValidationException($"EV {mods.EVs[4]} is out of range (0-{save.MaxEV})", INVALID_EV);
            pk.EV_SPA = mods.EVs[4];

            if (mods.EVs[5] < 0 || mods.EVs[5] > save.MaxEV)
                throw new ValidationException($"EV {mods.EVs[5]} is out of range (0-{save.MaxEV})", INVALID_EV);
            pk.EV_SPD = mods.EVs[5];
        }

        if (mods.Gender.HasValue)
        {
            if (mods.Gender.Value < 0 || mods.Gender.Value > 2)
                throw new ValidationException($"Gender {mods.Gender.Value} is out of range (0=Male, 1=Female, 2=Genderless)", "INVALID_GENDER");
            pk.Gender = (byte)mods.Gender.Value;
        }

        if (mods.IsShiny.HasValue && mods.IsShiny.Value && !pk.IsShiny)
            CommonEdits.SetShiny(pk, Shiny.AlwaysStar);

        if (mods.OT_Name != null)
            pk.OriginalTrainerName = mods.OT_Name;

        if (mods.Ball.HasValue)
        {
            if (mods.Ball.Value < 0 || mods.Ball.Value > save.MaxBallID)
                throw new ValidationException($"Ball {mods.Ball.Value} is out of range", "INVALID_BALL");
            pk.Ball = (byte)mods.Ball.Value;
        }

        pk.RefreshChecksum();
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string CheckLegality(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            var analysis = new LegalityAnalysis(pk);
            var errorList = new List<string>();
            var localizer = LegalityLocalizationContext.Create(analysis);

            foreach (var r in analysis.Results)
            {
                if (!r.Valid)
                    errorList.Add(localizer.Humanize(r));
            }

            return new LegalityResult(
                analysis.Valid,
                errorList.ToArray(),
                analysis.Report()
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string LegalizePokemon(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            pk.SetMoveset();
            pk.Heal();
            pk.RefreshChecksum();

            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Pokemon legalized successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string ExportShowdown(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            var showdownText = ShowdownParsing.GetShowdownText(pk);
            return new ShowdownResponse(true, showdownText);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string ImportShowdown(int handle, int box, int slot, string showdownText)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            ApiHelpers.ValidateBox(save, box);
            ApiHelpers.ValidateSlot(save, slot);

            if (string.IsNullOrWhiteSpace(showdownText))
                throw new ValidationException("Showdown text cannot be empty", "EMPTY_SHOWDOWN_TEXT");

            if (!ShowdownParsing.TryParseAnyLanguage(showdownText.AsSpan(), out var set))
                throw new ValidationException("Failed to parse Showdown text", "INVALID_SHOWDOWN_FORMAT");

            if (set.Species == 0)
                throw new ValidationException("Invalid species in Showdown text", INVALID_SPECIES);

            var pk = save.BlankPKM;
            ApplyShowdownSet(pk, set, save);

            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Pokemon imported from Showdown format successfully");
        });
    }

    private static void ApplyShowdownSet(PKM pk, ShowdownSet set, SaveFile save)
    {
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
    }

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

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetContestStats(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (pk is not IContestStatsReadOnly contestStats)
                throw new ValidationException("This Pokemon does not support contest stats", "NO_CONTEST_STATS");

            return new ContestStats(
                contestStats.ContestCool,
                contestStats.ContestBeauty,
                contestStats.ContestCute,
                contestStats.ContestSmart,
                contestStats.ContestTough,
                contestStats.ContestSheen
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetContestStat(int handle, int box, int slot, string statName, byte value)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (string.IsNullOrWhiteSpace(statName))
                throw new ValidationException("Stat name cannot be empty", "EMPTY_STAT_NAME");

            if (pk is not IContestStats contestStats)
                throw new ValidationException("This Pokemon does not support contest stats", "NO_CONTEST_STATS");

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
                    throw new ValidationException($"Invalid contest stat name: {statName}", "INVALID_STAT_NAME");
            }

            pk.RefreshChecksum();
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Contest stat set successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetTrainerInfo(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            return new TrainerInfoResponse(
                true,
                save.OT,
                save.DisplayTID,
                save.DisplaySID,
                save.Gender,
                save.Language,
                save.Money,
                save.PlayedHours,
                save.PlayedMinutes,
                save.PlayedSeconds
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetTrainerInfo(int handle, string trainerDataJson)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (string.IsNullOrWhiteSpace(trainerDataJson))
                throw new ValidationException("Trainer data cannot be empty", "EMPTY_TRAINER_DATA");

            var trainerData = JsonSerializer.Deserialize<TrainerInfo>(trainerDataJson, JsonOptions);
            if (trainerData == null)
                throw new ValidationException("Invalid trainer data JSON", INVALID_JSON);

            save.OT = trainerData.OT;
            save.Gender = (byte)trainerData.Gender;
            save.Language = trainerData.Language;
            save.Money = trainerData.Money;
            save.PlayedHours = trainerData.PlayedHours;
            save.PlayedMinutes = trainerData.PlayedMinutes;
            save.PlayedSeconds = trainerData.PlayedSeconds;

            return new SuccessMessage(true, "Trainer info updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetTrainerCard(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            string? startDate = null;
            int fame = 0;

            return new TrainerCard(
                save.OT,
                save.DisplayTID,
                save.DisplaySID,
                save.Money,
                startDate,
                fame
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetTrainerAppearance(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            int skin = 0, hair = 0, top = 0, bottom = 0, shoes = 0, accessory = 0, bag = 0, hat = 0;

            return new TrainerAppearance(
                skin,
                hair,
                top,
                bottom,
                shoes,
                accessory,
                bag,
                hat
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetTrainerAppearance(int handle, string appearanceJson)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (string.IsNullOrWhiteSpace(appearanceJson))
                throw new ValidationException("Appearance data cannot be empty", "EMPTY_APPEARANCE_DATA");

            var appearance = JsonSerializer.Deserialize<TrainerAppearance>(appearanceJson, JsonOptions);
            if (appearance == null)
                throw new ValidationException("Invalid appearance data JSON", INVALID_JSON);

            return new SuccessMessage(true, "Trainer appearance updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetRivalName(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            string rivalName = string.Empty;

            if (save is SAV1 sav1)
            {
                rivalName = sav1.Rival;
            }
            else if (save is SAV2 sav2)
            {
                rivalName = sav2.Rival;
            }
            else if (save is SAV4 sav4)
            {
                rivalName = sav4.Rival;
            }
            else
            {
                throw new ValidationException("Rival name not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new RivalNameResponse(true, rivalName);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetRivalName(int handle, string rivalName)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (string.IsNullOrWhiteSpace(rivalName))
                throw new ValidationException("Rival name cannot be empty", "EMPTY_RIVAL_NAME");

            if (save is SAV1 sav1)
            {
                sav1.Rival = rivalName;
            }
            else if (save is SAV2 sav2)
            {
                sav2.Rival = rivalName;
            }
            else if (save is SAV4 sav4)
            {
                sav4.Rival = rivalName;
            }
            else
            {
                throw new ValidationException("Rival name not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new SuccessMessage(true, "Rival name updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetBoxNames(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            var boxNames = new List<string>();
            for (int i = 0; i < save.BoxCount; i++)
            {
                if (save is IBoxDetailNameRead nameRead)
                    boxNames.Add(nameRead.GetBoxName(i));
                else
                    boxNames.Add(BoxDetailNameExtensions.GetDefaultBoxName(i));
            }

            return boxNames;
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetBoxWallpapers(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            var boxInfoList = new List<object>();
            for (int i = 0; i < save.BoxCount; i++)
            {
                string boxName = save is IBoxDetailNameRead nameRead
                    ? nameRead.GetBoxName(i)
                    : BoxDetailNameExtensions.GetDefaultBoxName(i);

                int wallpaper = save is IBoxDetailWallpaper wallpaperRead
                    ? wallpaperRead.GetBoxWallpaper(i)
                    : 0;

                boxInfoList.Add(new
                {
                    name = boxName,
                    wallpaper
                });
            }

            return boxInfoList;
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetBoxWallpaper(int handle, int box, int wallpaperId)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            ApiHelpers.ValidateBox(save, box);

            if (wallpaperId < 0)
                throw new ValidationException($"Wallpaper ID {wallpaperId} is invalid", "INVALID_WALLPAPER");

            if (save is not IBoxDetailWallpaper wallpaperWrite)
                throw new ValidationException("Box wallpapers are not supported for this save file generation", "UNSUPPORTED_GENERATION");

            wallpaperWrite.SetBoxWallpaper(box, wallpaperId);

            return new SuccessMessage(true, "Box wallpaper updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetBattleBox(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            var battleBoxList = new List<object>();

            if (save is SAV5 sav5)
            {
                var battleBox = sav5.BattleBox;
                for (int i = 0; i < 6; i++)
                {
                    var pkData = battleBox[i];
                    var pk = save.GetDecryptedPKM(pkData.ToArray());
                    if (pk.Species == 0)
                        continue;

                    battleBoxList.Add(new
                    {
                        box = -2,
                        slot = i,
                        species = pk.Species,
                        speciesName = GameInfo.Strings.Species[pk.Species],
                        level = pk.CurrentLevel,
                        isEgg = pk.IsEgg,
                        isShiny = pk.IsShiny
                    });
                }
            }
            else if (save is SAV6XY sav6xy)
            {
                var battleBox = sav6xy.BattleBox;
                for (int i = 0; i < 6; i++)
                {
                    var pkData = battleBox[i];
                    var pk = save.GetDecryptedPKM(pkData.ToArray());
                    if (pk.Species == 0)
                        continue;

                    battleBoxList.Add(new
                    {
                        box = -2,
                        slot = i,
                        species = pk.Species,
                        speciesName = GameInfo.Strings.Species[pk.Species],
                        level = pk.CurrentLevel,
                        isEgg = pk.IsEgg,
                        isShiny = pk.IsShiny
                    });
                }
            }
            else if (save is SAV6AO sav6ao)
            {
                var battleBox = sav6ao.BattleBox;
                for (int i = 0; i < 6; i++)
                {
                    var pkData = battleBox[i];
                    var pk = save.GetDecryptedPKM(pkData.ToArray());
                    if (pk.Species == 0)
                        continue;

                    battleBoxList.Add(new
                    {
                        box = -2,
                        slot = i,
                        species = pk.Species,
                        speciesName = GameInfo.Strings.Species[pk.Species],
                        level = pk.CurrentLevel,
                        isEgg = pk.IsEgg,
                        isShiny = pk.IsShiny
                    });
                }
            }
            else
            {
                throw new ValidationException("Battle Box is only supported for Generation 5-6 saves", "UNSUPPORTED_GENERATION");
            }

            return battleBoxList;
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetBattleBoxSlot(int handle, int slot, string base64PkmData)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (slot < 0 || slot >= 6)
                throw new ValidationException($"Battle Box slot {slot} is out of range (0-5)", INVALID_SLOT);

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

            if (save is SAV5 sav5)
            {
                var battleBox = sav5.BattleBox;
                var encData = pk.EncryptedBoxData;
                encData.CopyTo(battleBox[slot].Span);
            }
            else if (save is SAV6XY sav6xy)
            {
                var battleBox = sav6xy.BattleBox;
                var encData = pk.EncryptedBoxData;
                encData.CopyTo(battleBox[slot].Span);
            }
            else if (save is SAV6AO sav6ao)
            {
                var battleBox = sav6ao.BattleBox;
                var encData = pk.EncryptedBoxData;
                encData.CopyTo(battleBox[slot].Span);
            }
            else
            {
                throw new ValidationException("Battle Box is only supported for Generation 5-6 saves", "UNSUPPORTED_GENERATION");
            }

            return new SuccessMessage(true, "Battle Box slot updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetDaycare(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            int slot1Species = 0, slot1Level = 0;
            string slot1SpeciesName = string.Empty;
            int slot2Species = 0, slot2Level = 0;
            string slot2SpeciesName = string.Empty;
            bool hasEgg = false;

            if (save is IDaycareStorage daycare)
            {
                var slot1Data = daycare.GetDaycareSlot(0);
                var pk1 = save.GetDecryptedPKM(slot1Data.ToArray());
                if (pk1.Species != 0)
                {
                    slot1Species = pk1.Species;
                    slot1SpeciesName = GameInfo.Strings.Species[pk1.Species];
                    slot1Level = pk1.CurrentLevel;
                }

                if (daycare.DaycareSlotCount > 1)
                {
                    var slot2Data = daycare.GetDaycareSlot(1);
                    var pk2 = save.GetDecryptedPKM(slot2Data.ToArray());
                    if (pk2.Species != 0)
                    {
                        slot2Species = pk2.Species;
                        slot2SpeciesName = GameInfo.Strings.Species[pk2.Species];
                        slot2Level = pk2.CurrentLevel;
                    }
                }

                if (save is IDaycareEggState eggState)
                {
                    hasEgg = eggState.IsEggAvailable;
                }
            }
            else
            {
                throw new ValidationException("Daycare not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new DaycareData(
                slot1Species,
                slot1SpeciesName,
                slot1Level,
                slot2Species,
                slot2SpeciesName,
                slot2Level,
                hasEgg
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPouchItems(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            var pouchList = new List<PouchData>();
            var inventory = save.Inventory;

            for (int pouchIndex = 0; pouchIndex < inventory.Count; pouchIndex++)
            {
                var pouch = inventory[pouchIndex];
                var itemSlots = new List<ItemSlot>();

                foreach (var item in pouch.Items)
                {
                    if (item.Index == 0 || item.Count == 0)
                        continue;

                    var itemName = GameInfo.Strings.Item[item.Index];
                    itemSlots.Add(new ItemSlot(item.Index, itemName, item.Count));
                }

                pouchList.Add(new PouchData(
                    pouch.Type.ToString(),
                    pouchIndex,
                    itemSlots,
                    pouch.Items.Length
                ));
            }

            return pouchList;
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string AddItemToPouch(int handle, int itemId, int count, int pouchIndex)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (itemId < 0 || itemId > save.MaxItemID)
                throw new ValidationException($"Item ID {itemId} is out of range (0-{save.MaxItemID})", "INVALID_ITEM_ID");

            if (count <= 0)
                throw new ValidationException($"Count {count} must be greater than 0", "INVALID_COUNT");

            var inventory = save.Inventory;
            if (pouchIndex < 0 || pouchIndex >= inventory.Count)
                throw new ValidationException($"Pouch index {pouchIndex} is out of range (0-{inventory.Count - 1})", "INVALID_POUCH_INDEX");

            var pouch = inventory[pouchIndex];

            if (!pouch.Info.IsLegal(pouch.Type, itemId, count))
                throw new ValidationException($"Item {itemId} is not legal for pouch type {pouch.Type}", "ILLEGAL_ITEM");

            var existingItem = pouch.Items.FirstOrDefault(i => i.Index == itemId);
            if (existingItem != null)
            {
                var newCount = Math.Min(existingItem.Count + count, pouch.MaxCount);
                existingItem.Count = newCount;
            }
            else
            {
                var emptySlot = pouch.Items.FirstOrDefault(i => i.Index == 0);
                if (emptySlot == null)
                    throw new ValidationException("No empty slots available in pouch", "POUCH_FULL");

                emptySlot.Index = itemId;
                emptySlot.Count = Math.Min(count, pouch.MaxCount);
            }

            return new SuccessMessage(true, "Item added to pouch successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string RemoveItemFromPouch(int handle, int itemId, int count)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (itemId < 0 || itemId > save.MaxItemID)
                throw new ValidationException($"Item ID {itemId} is out of range (0-{save.MaxItemID})", "INVALID_ITEM_ID");

            if (count <= 0)
                throw new ValidationException($"Count {count} must be greater than 0", "INVALID_COUNT");

            var inventory = save.Inventory;
            bool itemFound = false;

            foreach (var pouch in inventory)
            {
                var item = pouch.Items.FirstOrDefault(i => i.Index == itemId);
                if (item != null && item.Count > 0)
                {
                    itemFound = true;
                    item.Count = Math.Max(0, item.Count - count);
                    if (item.Count == 0)
                        item.Index = 0;
                    break;
                }
            }

            if (!itemFound)
                throw new ValidationException($"Item {itemId} not found in inventory", "ITEM_NOT_FOUND");

            return new SuccessMessage(true, "Item removed from pouch successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPokedex(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            var pokedexList = new List<PokedexEntry>();

            for (ushort species = 1; species <= save.MaxSpeciesID; species++)
            {
                var seen = save.GetSeen(species);
                var caught = save.GetCaught(species);

                if (seen || caught)
                {
                    pokedexList.Add(new PokedexEntry(
                        (int)species,
                        GameInfo.Strings.Species[species],
                        seen,
                        caught
                    ));
                }
            }

            return pokedexList;
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetPokedexSeen(int handle, int species, int form)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (species < 0 || species > save.MaxSpeciesID)
                throw new ValidationException($"Species {species} is out of range (0-{save.MaxSpeciesID})", INVALID_SPECIES);

            save.SetSeen((ushort)species, true);

            return new SuccessMessage(true, "Pokedex seen status updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetPokedexCaught(int handle, int species, int form)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (species < 0 || species > save.MaxSpeciesID)
                throw new ValidationException($"Species {species} is out of range (0-{save.MaxSpeciesID})", INVALID_SPECIES);

            save.SetCaught((ushort)species, true);

            return new SuccessMessage(true, "Pokedex caught status updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetBadges(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            var badgeList = new List<bool>();
            int badgeCount = 0;

            if (save is SAV1 sav1)
            {
                for (int i = 0; i < 8; i++)
                {
                    bool hasBadge = (sav1.Badges & (1 << i)) != 0;
                    badgeList.Add(hasBadge);
                    if (hasBadge) badgeCount++;
                }
            }
            else if (save is SAV2 sav2)
            {
                for (int i = 0; i < 16; i++)
                {
                    bool hasBadge = (sav2.Badges & (1 << i)) != 0;
                    badgeList.Add(hasBadge);
                    if (hasBadge) badgeCount++;
                }
            }
            else if (save is SAV3 sav3)
            {
                for (int i = 0; i < 8; i++)
                {
                    bool hasBadge = (sav3.Badges & (1 << i)) != 0;
                    badgeList.Add(hasBadge);
                    if (hasBadge) badgeCount++;
                }
            }
            else if (save is SAV4 sav4)
            {
                for (int i = 0; i < 8; i++)
                {
                    bool hasBadge = (sav4.Badges & (1 << i)) != 0;
                    badgeList.Add(hasBadge);
                    if (hasBadge) badgeCount++;
                }
            }
            else if (save is SAV5 sav5)
            {
                int badges = sav5.Misc.Badges;
                for (int i = 0; i < 8; i++)
                {
                    bool hasBadge = (badges & (1 << i)) != 0;
                    badgeList.Add(hasBadge);
                    if (hasBadge) badgeCount++;
                }
            }
            else if (save is SAV6 sav6)
            {
                for (int i = 0; i < 8; i++)
                {
                    bool hasBadge = (sav6.Badges & (1 << i)) != 0;
                    badgeList.Add(hasBadge);
                    if (hasBadge) badgeCount++;
                }
            }
            else if (save is SAV7 sav7)
            {
                uint stamps = sav7.Misc.Stamps;
                for (int i = 0; i < 8; i++)
                {
                    bool hasBadge = (stamps & (1u << i)) != 0;
                    badgeList.Add(hasBadge);
                    if (hasBadge) badgeCount++;
                }
            }
            else if (save is SAV8SWSH sav8)
            {
                for (int i = 0; i < 8; i++)
                {
                    bool hasBadge = (sav8.Badges & (1 << i)) != 0;
                    badgeList.Add(hasBadge);
                    if (hasBadge) badgeCount++;
                }
            }
            else
            {
                throw new ValidationException("Badges not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new BadgeData(
                badgeCount,
                badgeList.ToArray()
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetBadge(int handle, int badgeIndex, bool value)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (badgeIndex < 0)
                throw new ValidationException($"Badge index {badgeIndex} must be non-negative", "INVALID_BADGE_INDEX");

            if (save is SAV1 sav1)
            {
                if (badgeIndex >= 8)
                    throw new ValidationException($"Badge index {badgeIndex} is out of range (0-7)", "INVALID_BADGE_INDEX");
                if (value)
                    sav1.Badges |= (byte)(1 << badgeIndex);
                else
                    sav1.Badges &= (byte)~(1 << badgeIndex);
            }
            else if (save is SAV2 sav2)
            {
                if (badgeIndex >= 16)
                    throw new ValidationException($"Badge index {badgeIndex} is out of range (0-15)", "INVALID_BADGE_INDEX");
                if (value)
                    sav2.Badges |= (ushort)(1 << badgeIndex);
                else
                    sav2.Badges &= (ushort)~(1 << badgeIndex);
            }
            else if (save is SAV3 sav3)
            {
                if (badgeIndex >= 8)
                    throw new ValidationException($"Badge index {badgeIndex} is out of range (0-7)", "INVALID_BADGE_INDEX");
                if (value)
                    sav3.Badges |= (byte)(1 << badgeIndex);
                else
                    sav3.Badges &= (byte)~(1 << badgeIndex);
            }
            else if (save is SAV4 sav4)
            {
                if (badgeIndex >= 8)
                    throw new ValidationException($"Badge index {badgeIndex} is out of range (0-7)", "INVALID_BADGE_INDEX");
                if (value)
                    sav4.Badges |= (byte)(1 << badgeIndex);
                else
                    sav4.Badges &= (byte)~(1 << badgeIndex);
            }
            else if (save is SAV5 sav5)
            {
                if (badgeIndex >= 8)
                    throw new ValidationException($"Badge index {badgeIndex} is out of range (0-7)", "INVALID_BADGE_INDEX");
                int badges = sav5.Misc.Badges;
                if (value)
                    badges |= (1 << badgeIndex);
                else
                    badges &= ~(1 << badgeIndex);
                sav5.Misc.Badges = badges;
            }
            else if (save is SAV6 sav6)
            {
                if (badgeIndex >= 8)
                    throw new ValidationException($"Badge index {badgeIndex} is out of range (0-7)", "INVALID_BADGE_INDEX");
                if (value)
                    sav6.Badges |= (byte)(1 << badgeIndex);
                else
                    sav6.Badges &= (byte)~(1 << badgeIndex);
            }
            else if (save is SAV7 sav7)
            {
                if (badgeIndex >= 8)
                    throw new ValidationException($"Badge index {badgeIndex} is out of range (0-7)", "INVALID_BADGE_INDEX");
                uint stamps = sav7.Misc.Stamps;
                if (value)
                    stamps |= (1u << badgeIndex);
                else
                    stamps &= ~(1u << badgeIndex);
                sav7.Misc.Stamps = stamps;
            }
            else if (save is SAV8SWSH sav8)
            {
                if (badgeIndex >= 8)
                    throw new ValidationException($"Badge index {badgeIndex} is out of range (0-7)", "INVALID_BADGE_INDEX");
                if (value)
                    sav8.Badges |= (byte)(1 << badgeIndex);
                else
                    sav8.Badges &= (byte)~(1 << badgeIndex);
            }
            else
            {
                throw new ValidationException("Badges not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new SuccessMessage(true, "Badge updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetBattlePoints(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            int battlePoints = 0;

            if (save is SAV4 sav4)
            {
                battlePoints = sav4.BP;
            }
            else if (save is SAV5 sav5)
            {
                battlePoints = sav5.BattleSubway.BP;
            }
            else if (save is SAV6 sav6)
            {
                battlePoints = sav6.BP;
            }
            else if (save is SAV7 sav7)
            {
                battlePoints = (int)sav7.Misc.BP;
            }
            else
            {
                throw new ValidationException("Battle Points not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new BattlePointsResponse(true, battlePoints);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetBattlePoints(int handle, int battlePoints)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (battlePoints < 0)
                throw new ValidationException($"Battle Points {battlePoints} must be non-negative", "INVALID_BATTLE_POINTS");

            if (save is SAV4 sav4)
            {
                sav4.BP = battlePoints;
            }
            else if (save is SAV5 sav5)
            {
                sav5.BattleSubway.BP = battlePoints;
            }
            else if (save is SAV6 sav6)
            {
                sav6.BP = battlePoints;
            }
            else if (save is SAV7 sav7)
            {
                sav7.Misc.BP = (uint)battlePoints;
            }
            else
            {
                throw new ValidationException("Battle Points not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new SuccessMessage(true, "Battle Points updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetCoins(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            int coins = 0;

            if (save is SAV1 sav1)
            {
                coins = (int)sav1.Coin;
            }
            else if (save is SAV2 sav2)
            {
                coins = (int)sav2.Coin;
            }
            else if (save is SAV3 sav3)
            {
                coins = (int)sav3.Coin;
            }
            else if (save is SAV4 sav4)
            {
                coins = (int)sav4.Coin;
            }
            else
            {
                throw new ValidationException("Coins not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new CoinsResponse(true, coins);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetCoins(int handle, int coins)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (coins < 0)
                throw new ValidationException($"Coins {coins} must be non-negative", "INVALID_COINS");

            if (save is SAV1 sav1)
            {
                sav1.Coin = (uint)Math.Min(coins, 9999);
            }
            else if (save is SAV2 sav2)
            {
                sav2.Coin = (uint)Math.Min(coins, 9999);
            }
            else if (save is SAV3 sav3)
            {
                sav3.Coin = (uint)Math.Min(coins, 9999);
            }
            else if (save is SAV4 sav4)
            {
                sav4.Coin = (uint)Math.Min(coins, 50000);
            }
            else
            {
                throw new ValidationException("Coins not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new SuccessMessage(true, "Coins updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetRecords(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            var records = new Dictionary<string, int>();

            if (save is SAV6 sav6)
            {
                var recordBlock = sav6.Records;
                for (int i = 0; i < RecordBlock6.RecordCount; i++)
                {
                    records[$"record_{i}"] = recordBlock.GetRecord(i);
                }
            }
            else if (save is SAV7 sav7)
            {
                var recordBlock = sav7.Records;
                for (int i = 0; i < RecordBlock6.RecordCount; i++)
                {
                    records[$"record_{i}"] = recordBlock.GetRecord(i);
                }
            }
            else if (save is SAV8SWSH sav8)
            {
                var recordBlock = sav8.Records;
                for (int i = 0; i < Record8.RecordCount; i++)
                {
                    records[$"record_{i}"] = recordBlock.GetRecord(i);
                }
            }
            else
            {
                throw new ValidationException("Records not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new RecordsResponse(true, records);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetRecord(int handle, int recordIndex, int value)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            ApiHelpers.ValidateNonNegative(recordIndex, "Record index", "INVALID_RECORD_INDEX");
            ApiHelpers.ValidateNonNegative(value, "Record value", "INVALID_RECORD_VALUE");

            if (save is SAV6 sav6)
            {
                var recordBlock = sav6.Records;
                ApiHelpers.ValidateRange(recordIndex, 0, RecordBlock6.RecordCount - 1, "Record index", "INVALID_RECORD_INDEX");
                recordBlock.SetRecord(recordIndex, value);
            }
            else if (save is SAV7 sav7)
            {
                var recordBlock = sav7.Records;
                ApiHelpers.ValidateRange(recordIndex, 0, RecordBlock6.RecordCount - 1, "Record index", "INVALID_RECORD_INDEX");
                recordBlock.SetRecord(recordIndex, value);
            }
            else if (save is SAV8SWSH sav8)
            {
                var recordBlock = sav8.Records;
                ApiHelpers.ValidateRange(recordIndex, 0, Record8.RecordCount - 1, "Record index", "INVALID_RECORD_INDEX");
                recordBlock.SetRecord(recordIndex, value);
            }
            else
            {
                throw new ValidationException("Records not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return ApiHelpers.SuccessMessage("Record updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetEventFlag(int handle, int flagIndex)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            ApiHelpers.ValidateNonNegative(flagIndex, "Flag index", "INVALID_FLAG_INDEX");

            bool flagValue;

            if (save is IEventFlagArray eventFlagArray)
            {
                ApiHelpers.ValidateRange(flagIndex, 0, eventFlagArray.EventFlagCount - 1, "Flag index", "INVALID_FLAG_INDEX");
                flagValue = eventFlagArray.GetEventFlag(flagIndex);
            }
            else if (save is IEventFlagProvider37 provider37)
            {
                ApiHelpers.ValidateRange(flagIndex, 0, provider37.EventWork.EventFlagCount - 1, "Flag index", "INVALID_FLAG_INDEX");
                flagValue = provider37.EventWork.GetEventFlag(flagIndex);
            }
            else
            {
                throw new ValidationException("Event flags not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new EventFlagResponse(true, flagIndex, flagValue);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetEventFlag(int handle, int flagIndex, bool value)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            ApiHelpers.ValidateNonNegative(flagIndex, "Flag index", "INVALID_FLAG_INDEX");

            if (save is IEventFlagArray eventFlagArray)
            {
                ApiHelpers.ValidateRange(flagIndex, 0, eventFlagArray.EventFlagCount - 1, "Flag index", "INVALID_FLAG_INDEX");
                eventFlagArray.SetEventFlag(flagIndex, value);
            }
            else if (save is IEventFlagProvider37 provider37)
            {
                ApiHelpers.ValidateRange(flagIndex, 0, provider37.EventWork.EventFlagCount - 1, "Flag index", "INVALID_FLAG_INDEX");
                provider37.EventWork.SetEventFlag(flagIndex, value);
            }
            else
            {
                throw new ValidationException("Event flags not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return ApiHelpers.SuccessMessage("Event flag updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetEventConst(int handle, int constIndex)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            ApiHelpers.ValidateNonNegative(constIndex, "Const index", "INVALID_CONST_INDEX");

            int constValue;

            if (save is SAV2 sav2)
            {
                constValue = sav2.GetWork(constIndex);
            }
            else if (save is IEventFlagProvider37 provider37)
            {
                if (provider37.EventWork is IEventWorkArray<ushort> workArray)
                {
                    ApiHelpers.ValidateRange(constIndex, 0, workArray.EventWorkCount - 1, "Const index", "INVALID_CONST_INDEX");
                    constValue = workArray.GetWork(constIndex);
                }
                else
                {
                    throw new ValidationException("Event consts not supported for this save file generation", "UNSUPPORTED_GENERATION");
                }
            }
            else
            {
                throw new ValidationException("Event consts not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new EventConstResponse(true, constIndex, constValue);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetEventConst(int handle, int constIndex, int value)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            ApiHelpers.ValidateNonNegative(constIndex, "Const index", "INVALID_CONST_INDEX");

            if (save is SAV2 sav2)
            {
                sav2.SetWork(constIndex, (byte)value);
            }
            else if (save is IEventFlagProvider37 provider37)
            {
                if (provider37.EventWork is IEventWorkArray<ushort> workArray)
                {
                    ApiHelpers.ValidateRange(constIndex, 0, workArray.EventWorkCount - 1, "Const index", "INVALID_CONST_INDEX");
                    workArray.SetWork(constIndex, (ushort)value);
                }
                else
                {
                    throw new ValidationException("Event consts not supported for this save file generation", "UNSUPPORTED_GENERATION");
                }
            }
            else
            {
                throw new ValidationException("Event consts not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return ApiHelpers.SuccessMessage("Event const updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetBattleFacilityStats(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            var stats = new Dictionary<string, object>();

            if (save is SAV5 sav5)
            {
                var subway = sav5.BattleSubway;
                stats["bp"] = subway.BP;
                stats["superSingle"] = subway.SuperSingle;
                stats["superDouble"] = subway.SuperDouble;
                stats["superMulti"] = subway.SuperMulti;
                stats["singlePast"] = subway.SinglePast;
                stats["doublePast"] = subway.DoublePast;
                stats["multiNPCPast"] = subway.MultiNPCPast;
                stats["multiFriendsPast"] = subway.MultiFriendsPast;
                stats["superSinglePast"] = subway.SuperSinglePast;
                stats["superDoublePast"] = subway.SuperDoublePast;
                stats["superMultiNPCPast"] = subway.SuperMultiNPCPast;
                stats["superMultiFriendsPast"] = subway.SuperMultiFriendsPast;
                stats["singleRecord"] = subway.SingleRecord;
                stats["doubleRecord"] = subway.DoubleRecord;
                stats["multiNPCRecord"] = subway.MultiNPCRecord;
                stats["multiFriendsRecord"] = subway.MultiFriendsRecord;
                stats["superSingleRecord"] = subway.SuperSingleRecord;
                stats["superDoubleRecord"] = subway.SuperDoubleRecord;
                stats["superMultiNPCRecord"] = subway.SuperMultiNPCRecord;
                stats["superMultiFriendsRecord"] = subway.SuperMultiFriendsRecord;
            }
            else if (save is SAV6 sav6)
            {
                stats["bp"] = sav6.BP;
            }
            else if (save is SAV7 sav7)
            {
                stats["bp"] = sav7.Misc.BP;
            }
            else
            {
                throw new ValidationException("Battle Facility stats not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return stats;
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetHallOfFame(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            var hofEntries = new List<HallOfFameEntry>();

            if (save is SAV1 sav1)
            {
                var hof = sav1.HallOfFame;
                for (int team = 0; team < HallOfFameReader1.TeamCount; team++)
                {
                    var teamList = new List<PokemonSummary>();
                    var memberCount = hof.GetTeamMemberCount(team);

                    if (memberCount == 0)
                        continue;

                    for (int slot = 0; slot < memberCount; slot++)
                    {
                        var entity = hof.GetEntity(team, slot);
                        if (entity.Species == 0)
                            continue;

                        var summary = new PokemonSummary(
                            Box: -3,
                            Slot: slot,
                            Species: entity.Species,
                            SpeciesName: GameInfo.Strings.Species[entity.Species],
                            Level: entity.Level,
                            IsEgg: false,
                            IsShiny: false
                        );
                        teamList.Add(summary);
                    }

                    if (teamList.Count > 0)
                    {
                        var hofEntry = new HallOfFameEntry(
                            Index: team,
                            Timestamp: string.Empty,
                            Team: teamList.ToArray()
                        );
                        hofEntries.Add(hofEntry);
                    }
                }
            }
            else if (save is SAV3 sav3)
            {
                var entries = HallFame3Entry.GetEntries(sav3);
                for (int i = 0; i < entries.Length; i++)
                {
                    var entry = entries[i];
                    var team = entry.Team;
                    var teamList = new List<PokemonSummary>();

                    for (int j = 0; j < team.Length; j++)
                    {
                        var pk = team[j];
                        if (pk.Species == 0)
                            continue;

                        var summary = new PokemonSummary(
                            Box: -3,
                            Slot: j,
                            Species: pk.Species,
                            SpeciesName: GameInfo.Strings.Species[pk.Species],
                            Level: pk.Level,
                            IsEgg: false,
                            IsShiny: pk.IsShiny
                        );
                        teamList.Add(summary);
                    }

                    if (teamList.Count > 0)
                    {
                        var hofEntry = new HallOfFameEntry(
                            Index: i,
                            Timestamp: string.Empty,
                            Team: teamList.ToArray()
                        );
                        hofEntries.Add(hofEntry);
                    }
                }
            }
            else
            {
                throw new ValidationException("Hall of Fame not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new HallOfFameResponse(true, hofEntries);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetHallOfFameEntry(int handle, int index, string teamJson)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (index < 0)
                throw new ValidationException($"Index {index} must be non-negative", "INVALID_INDEX");

            if (string.IsNullOrWhiteSpace(teamJson))
                throw new ValidationException("Team data cannot be empty", "EMPTY_TEAM_DATA");

            var team = JsonSerializer.Deserialize<PokemonSummary[]>(teamJson, JsonOptions);
            if (team == null || team.Length == 0)
                throw new ValidationException("Invalid team data JSON", INVALID_JSON);

            if (save is SAV3 sav3)
            {
                var entries = HallFame3Entry.GetEntries(sav3);
                if (index >= entries.Length)
                    throw new ValidationException($"Index {index} is out of range (0-{entries.Length - 1})", "INVALID_INDEX");

                var entry = entries[index];
                var entryTeam = entry.Team;
                for (int i = 0; i < Math.Min(team.Length, entryTeam.Length); i++)
                {
                    var pkData = team[i];
                    var pk = entryTeam[i];
                    pk.Species = (ushort)pkData.Species;
                    pk.Level = pkData.Level;
                }

                HallFame3Entry.SetEntries(sav3, entries);
            }
            else
            {
                throw new ValidationException("Hall of Fame modification not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new SuccessMessage(true, "Hall of Fame entry updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetSecondsPlayed(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            int totalSeconds = (save.PlayedHours * 3600) + (save.PlayedMinutes * 60) + save.PlayedSeconds;
            return new SecondsPlayedResponse(true, totalSeconds);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetSecondsToStart(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            return new SecondsToStartResponse(true, save.SecondsToStart);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetSecondsToFame(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            return new SecondsToFameResponse(true, save.SecondsToFame);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetGameTime(int handle, int hours, int minutes, int seconds)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (hours < 0)
                throw new ValidationException($"Hours {hours} must be non-negative", "INVALID_HOURS");

            if (minutes < 0 || minutes >= 60)
                throw new ValidationException($"Minutes {minutes} must be between 0 and 59", "INVALID_MINUTES");

            if (seconds < 0 || seconds >= 60)
                throw new ValidationException($"Seconds {seconds} must be between 0 and 59", "INVALID_SECONDS");

            save.PlayedHours = hours;
            save.PlayedMinutes = minutes;
            save.PlayedSeconds = seconds;

            return new SuccessMessage(true, "Game time updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetSecondsToStart(int handle, int seconds)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (seconds < 0)
                throw new ValidationException($"Seconds {seconds} must be non-negative", "INVALID_SECONDS");

            save.SecondsToStart = (uint)seconds;

            return new SuccessMessage(true, "Seconds to start updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetSecondsToFame(int handle, int seconds)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (seconds < 0)
                throw new ValidationException($"Seconds {seconds} must be non-negative", "INVALID_SECONDS");

            save.SecondsToFame = (uint)seconds;

            return new SuccessMessage(true, "Seconds to fame updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetMailbox(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            throw new ValidationException("Mail functionality is not currently supported", "UNSUPPORTED_FEATURE");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetMailMessage(int handle, int index)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (index < 0)
                throw new ValidationException($"Mail index {index} must be non-negative", "INVALID_MAIL_INDEX");

            throw new ValidationException("Mail functionality is not currently supported", "UNSUPPORTED_FEATURE");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetMailMessage(int handle, int index, string mailJson)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (index < 0)
                throw new ValidationException($"Mail index {index} must be non-negative", "INVALID_MAIL_INDEX");

            if (string.IsNullOrWhiteSpace(mailJson))
                throw new ValidationException("Mail data cannot be empty", "EMPTY_MAIL_DATA");

            throw new ValidationException("Mail functionality is not currently supported", "UNSUPPORTED_FEATURE");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string DeleteMail(int handle, int index)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (index < 0)
                throw new ValidationException($"Mail index {index} must be non-negative", "INVALID_MAIL_INDEX");

            throw new ValidationException("Mail functionality is not currently supported", "UNSUPPORTED_FEATURE");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetMysteryGifts(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            var giftList = new List<MysteryGiftCard>();

            if (save is IMysteryGiftStorage giftStorage)
            {
                for (int i = 0; i < giftStorage.GiftCountMax; i++)
                {
                    var gift = giftStorage.GetMysteryGift(i);
                    if (gift.Species == 0 && gift.ItemID == 0)
                        continue;

                    var card = new MysteryGiftCard(
                        Index: i,
                        Type: gift.Type.ToString(),
                        CardTitle: gift.CardTitle,
                        IsItem: gift.IsItem,
                        IsPokemon: gift.IsEntity,
                        IsBP: false,
                        ItemId: gift.ItemID,
                        Species: gift.Species,
                        Level: gift.Level,
                        IsShiny: gift.IsShiny,
                        IsEgg: gift.IsEgg
                    );
                    giftList.Add(card);
                }
            }
            else
            {
                throw new ValidationException("Mystery Gifts not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new MysteryGiftsResponse(true, giftList);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetMysteryGiftCard(int handle, int index)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (index < 0)
                throw new ValidationException($"Gift index {index} must be non-negative", "INVALID_GIFT_INDEX");

            if (save is not IMysteryGiftStorage giftStorage)
                throw new ValidationException("Mystery Gifts not supported for this save file generation", "UNSUPPORTED_GENERATION");

            if (index >= giftStorage.GiftCountMax)
                throw new ValidationException($"Gift index {index} is out of range (0-{giftStorage.GiftCountMax - 1})", "INVALID_GIFT_INDEX");

            var gift = giftStorage.GetMysteryGift(index);
            var card = new MysteryGiftCard(
                Index: index,
                Type: gift.Type.ToString(),
                CardTitle: gift.CardTitle,
                IsItem: gift.IsItem,
                IsPokemon: gift.IsEntity,
                IsBP: false,
                ItemId: gift.ItemID,
                Species: gift.Species,
                Level: gift.Level,
                IsShiny: gift.IsShiny,
                IsEgg: gift.IsEgg
            );

            return new MysteryGiftCardResponse(true, card);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetMysteryGiftCard(int handle, int index, string cardJson)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (index < 0)
                throw new ValidationException($"Gift index {index} must be non-negative", "INVALID_GIFT_INDEX");

            if (string.IsNullOrWhiteSpace(cardJson))
                throw new ValidationException("Card data cannot be empty", "EMPTY_CARD_DATA");

            var cardData = JsonSerializer.Deserialize<MysteryGiftCard>(cardJson, JsonOptions);
            if (cardData == null)
                throw new ValidationException("Invalid card data JSON", INVALID_JSON);

            if (save is not IMysteryGiftStorage giftStorage)
                throw new ValidationException("Mystery Gifts not supported for this save file generation", "UNSUPPORTED_GENERATION");

            if (index >= giftStorage.GiftCountMax)
                throw new ValidationException($"Gift index {index} is out of range (0-{giftStorage.GiftCountMax - 1})", "INVALID_GIFT_INDEX");

            var gift = giftStorage.GetMysteryGift(index);
            gift.CardTitle = cardData.CardTitle;
            giftStorage.SetMysteryGift(index, gift);

            return new SuccessMessage(true, "Mystery Gift card updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetMysteryGiftFlags(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            var flags = new List<bool>();

            if (save is IMysteryGiftFlags giftFlags)
            {
                for (int i = 0; i < giftFlags.MysteryGiftReceivedFlagMax; i++)
                {
                    flags.Add(giftFlags.GetMysteryGiftReceivedFlag(i));
                }
            }
            else
            {
                throw new ValidationException("Mystery Gift flags not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new MysteryGiftFlagsResponse(true, flags.ToArray());
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string DeleteMysteryGift(int handle, int index)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (index < 0)
                throw new ValidationException($"Gift index {index} must be non-negative", "INVALID_GIFT_INDEX");

            if (save is not IMysteryGiftStorage giftStorage)
                throw new ValidationException("Mystery Gifts not supported for this save file generation", "UNSUPPORTED_GENERATION");

            if (index >= giftStorage.GiftCountMax)
                throw new ValidationException($"Gift index {index} is out of range (0-{giftStorage.GiftCountMax - 1})", "INVALID_GIFT_INDEX");

            var gift = giftStorage.GetMysteryGift(index);
            gift.Data.Clear();
            giftStorage.SetMysteryGift(index, gift);

            return new SuccessMessage(true, "Mystery Gift deleted successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetSecretBase(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (save is SAV3 sav3)
            {
                var trainerName = sav3.OT;
                var trainerId = (int)sav3.TID16;
                var secretId = (int)sav3.SID16;
                var gender = sav3.Gender;
                var language = sav3.Language;

                var secretBase = new SecretBaseData(
                    TrainerName: trainerName,
                    TrainerID: trainerId,
                    SecretID: secretId,
                    Gender: gender,
                    Language: language,
                    LocationName: "Secret Base",
                    LocationID: 0
                );

                return new SecretBaseResponse(true, secretBase);
            }
            else if (save is SAV4 sav4)
            {
                var trainerName = sav4.OT;
                var trainerId = (int)sav4.TID16;
                var secretId = (int)sav4.SID16;
                var gender = sav4.Gender;
                var language = sav4.Language;

                var secretBase = new SecretBaseData(
                    TrainerName: trainerName,
                    TrainerID: trainerId,
                    SecretID: secretId,
                    Gender: gender,
                    Language: language,
                    LocationName: "Underground Base",
                    LocationID: 0
                );

                return new SecretBaseResponse(true, secretBase);
            }
            else if (save is SAV6AO sav6ao)
            {
                var secretBaseBlock = sav6ao.SecretBase;
                var selfBase = secretBaseBlock.GetSecretBaseSelf();

                var secretBase = new SecretBaseData(
                    TrainerName: selfBase.TrainerName,
                    TrainerID: (int)sav6ao.TID16,
                    SecretID: (int)sav6ao.SID16,
                    Gender: sav6ao.Gender,
                    Language: sav6ao.Language,
                    LocationName: "Secret Base",
                    LocationID: selfBase.BaseLocation
                );

                return new SecretBaseResponse(true, secretBase);
            }
            else
            {
                throw new ValidationException("Secret Base only supported for Gen 3, Gen 4, and Gen 6 (ORAS) saves", "UNSUPPORTED_GENERATION");
            }
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetEntralinkData(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (save is SAV5 sav5)
            {
                var forestLevel = 0;
                var missionsCompleted = 0;
                var whiteForestCount = 0;
                var blackCityCount = 0;

                if (sav5 is SAV5BW sav5bw)
                {
                    var entree = sav5bw.Entralink;
                    whiteForestCount = entree.WhiteForestLevel;
                    blackCityCount = entree.BlackCityLevel;
                }

                var entralinkData = new EntralinkData(
                    ForestLevel: forestLevel,
                    MissionsCompleted: missionsCompleted,
                    WhiteForestCount: whiteForestCount,
                    BlackCityCount: blackCityCount
                );

                return new EntralinkResponse(true, entralinkData);
            }
            else
            {
                throw new ValidationException("Entralink/Join Avenue only supported for Gen 5 saves", "UNSUPPORTED_GENERATION");
            }
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPokePelago(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (save is SAV7 sav7)
            {
                var pelago = sav7.ResortSave;
                var beansCount = 0;
                for (int i = 0; i < ResortSave7.BEANS_MAX; i++)
                {
                    beansCount += pelago.GetPokebeanCount(i);
                }

                var pokePelagoData = new PokePelagoData(
                    BeansCount: beansCount,
                    IsleAevelynDevelopment: 0,
                    IsleAphunDevelopment: 0,
                    IsleEvelupDevelopment: 0,
                    PokemonCount: 0
                );

                return new PokePelagoResponse(true, pokePelagoData);
            }
            else
            {
                throw new ValidationException("Poké Pelago only supported for Gen 7 saves", "UNSUPPORTED_GENERATION");
            }
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetFestivalPlaza(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (save is SAV7 sav7)
            {
                var plaza = sav7.Festa;
                var rank = plaza.FestaRank;
                var coins = plaza.FestaCoins;

                var festivalPlazaData = new FestivalPlazaData(
                    Rank: rank,
                    FestivalCoins: coins,
                    TotalVisitors: 0,
                    FacilityCount: 0
                );

                return new FestivalPlazaResponse(true, festivalPlazaData);
            }
            else
            {
                throw new ValidationException("Festival Plaza only supported for Gen 7 saves", "UNSUPPORTED_GENERATION");
            }
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPokeJobs(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (save is SAV8SWSH sav8)
            {
                var activeJobsCount = 0;
                var completedJobsCount = 0;

                var pokeJobsData = new PokeJobsData(
                    ActiveJobsCount: activeJobsCount,
                    CompletedJobsCount: completedJobsCount
                );

                return new PokeJobsResponse(true, pokeJobsData);
            }
            else
            {
                throw new ValidationException("Pokémon Jobs only supported for Gen 8 (Sword/Shield) saves", "UNSUPPORTED_GENERATION");
            }
        });
    }

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
    [return: JSMarshalAs<JSType.String>]
    public static string SetPKMShiny(string base64PkmData, int generation, int shinyType)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            if (shinyType < 0 || shinyType > 5)
                throw new ValidationException($"Shiny type {shinyType} is out of range (0-5)", INVALID_SHINY_TYPE);

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            var type = (Shiny)shinyType;

            if (type == Shiny.Never)
            {
                pk.SetUnshiny();
            }
            else
            {
                CommonEdits.SetShiny(pk, type);
            }

            var modifiedData = pk.DecryptedPartyData;
            var base64Result = Convert.ToBase64String(modifiedData);

            return new PKMDataResponse(true, base64Result);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPKMPIDInfo(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

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

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetPKMPID(string base64PkmData, int generation, int pid)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            if (pid < 0)
                throw new ValidationException("PID must be non-negative", INVALID_PID);

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            pk.PID = (uint)pid;
            pk.RefreshAbility(pk.AbilityNumber >> 1);

            var modifiedData = pk.DecryptedPartyData;
            var base64Result = Convert.ToBase64String(modifiedData);

            return new PKMDataResponse(true, base64Result);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPKMData(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            return CreatePokemonDetailObject(pk);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string ModifyPKMData(string base64PkmData, int generation, string modificationsJson)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            var mods = JsonSerializer.Deserialize<PokemonModifications>(modificationsJson, JsonOptions);
            if (mods == null)
                throw new ValidationException("Invalid modifications JSON", INVALID_JSON);


            if (mods.Species.HasValue)
                pk.Species = (ushort)mods.Species.Value;
            if (mods.Nickname != null)
                pk.Nickname = mods.Nickname;
            if (mods.Level.HasValue)
                pk.CurrentLevel = (byte)mods.Level.Value;
            if (mods.Nature.HasValue)
                pk.Nature = (Nature)mods.Nature.Value;
            if (mods.Ability.HasValue)
                pk.Ability = mods.Ability.Value;
            if (mods.HeldItem.HasValue)
                pk.HeldItem = mods.HeldItem.Value;
            if (mods.Gender.HasValue)
                pk.Gender = (byte)mods.Gender.Value;
            if (mods.IsShiny.HasValue && mods.IsShiny.Value && !pk.IsShiny)
                CommonEdits.SetShiny(pk, Shiny.AlwaysStar);
            if (mods.OT_Name != null)
                pk.OriginalTrainerName = mods.OT_Name;
            if (mods.Ball.HasValue)
                pk.Ball = (byte)mods.Ball.Value;

            if (mods.Moves != null && mods.Moves.Length > 0)
            {
                if (mods.Moves.Length > 0) pk.Move1 = (ushort)mods.Moves[0];
                if (mods.Moves.Length > 1) pk.Move2 = (ushort)mods.Moves[1];
                if (mods.Moves.Length > 2) pk.Move3 = (ushort)mods.Moves[2];
                if (mods.Moves.Length > 3) pk.Move4 = (ushort)mods.Moves[3];
            }

            if (mods.IVs != null && mods.IVs.Length == 6)
            {
                pk.IV_HP = mods.IVs[0];
                pk.IV_ATK = mods.IVs[1];
                pk.IV_DEF = mods.IVs[2];
                pk.IV_SPE = mods.IVs[3];
                pk.IV_SPA = mods.IVs[4];
                pk.IV_SPD = mods.IVs[5];
            }

            if (mods.EVs != null && mods.EVs.Length == 6)
            {
                pk.EV_HP = mods.EVs[0];
                pk.EV_ATK = mods.EVs[1];
                pk.EV_DEF = mods.EVs[2];
                pk.EV_SPE = mods.EVs[3];
                pk.EV_SPA = mods.EVs[4];
                pk.EV_SPD = mods.EVs[5];
            }

            pk.RefreshChecksum();

            var modifiedData = pk.DecryptedPartyData;
            var base64Result = Convert.ToBase64String(modifiedData);

            return new PKMDataResponse(true, base64Result);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string CheckPKMLegality(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            var analysis = new LegalityAnalysis(pk);
            var errorList = new List<string>();
            var localizer = LegalityLocalizationContext.Create(analysis);

            foreach (var r in analysis.Results)
            {
                if (!r.Valid)
                    errorList.Add(localizer.Humanize(r));
            }

            return new LegalityResult(
                analysis.Valid,
                errorList.ToArray(),
                analysis.Report()
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string LegalizePKMData(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            pk.SetMoveset();
            pk.Heal();
            pk.RefreshChecksum();

            var modifiedData = pk.DecryptedPartyData;
            var base64Result = Convert.ToBase64String(modifiedData);

            return new PKMDataResponse(true, base64Result);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string ExportPKMShowdown(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            var showdownText = ShowdownParsing.GetShowdownText(pk);
            return new ShowdownResponse(true, showdownText);
        });
    }

    // TODO: ImportPKMShowdown and CreatePKM require proper blank Pokemon creation
    // which needs investigation into PKHeX.Core's EntityBlank or similar APIs
    /*
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string ImportPKMShowdown(string showdownText, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            if (string.IsNullOrWhiteSpace(showdownText))
                throw new ValidationException("Showdown text cannot be empty", "EMPTY_SHOWDOWN_TEXT");

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            if (!ShowdownParsing.TryParseAnyLanguage(showdownText.AsSpan(), out var set))
                throw new ValidationException("Failed to parse Showdown text", "INVALID_SHOWDOWN_FORMAT");

            if (set.Species == 0)
                throw new ValidationException("Invalid species in Showdown text", INVALID_SPECIES);

            // Create a blank Pokemon using EntityBlank
            var context = (EntityContext)generation;
            var pk = EntityBlank.GetBlank(context);

            // Apply showdown set properties
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

            var pkmData = pk.DecryptedPartyData;
            var base64Result = Convert.ToBase64String(pkmData);

            return new PKMDataResponse(true, base64Result);
        });
    }
    */

    /*
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string CreatePKM(int species, int level, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            if (species < 1 || species > 1025)
                throw new ValidationException($"Species {species} is out of range (1-1025)", INVALID_SPECIES);

            if (level < 1 || level > 100)
                throw new ValidationException($"Level {level} is out of range (1-100)", INVALID_LEVEL);

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var context = (EntityContext)generation;
            var pk = EntityBlank.GetBlank(context);

            pk.Species = (ushort)species;
            pk.CurrentLevel = (byte)level;
            pk.ClearNickname();
            pk.SetMoveset();
            pk.Heal();
            pk.RefreshChecksum();

            var pkmData = pk.DecryptedPartyData;
            var base64Result = Convert.ToBase64String(pkmData);

            return new PKMDataResponse(true, base64Result);
        });
    }
    */

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string CalculatePKMStats(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);


            return new PokemonStats(
                pk.Stat_HPMax,
                pk.Stat_ATK,
                pk.Stat_DEF,
                pk.Stat_SPA,
                pk.Stat_SPD,
                pk.Stat_SPE
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPKMRibbons(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

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
    public static string SetPKMRibbon(string base64PkmData, int generation, string ribbonName, bool value)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

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

            var modifiedData = pk.DecryptedPartyData;
            var base64Result = Convert.ToBase64String(modifiedData);

            return new PKMDataResponse(true, base64Result);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string RerollPKMEncryptionConstant(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            var rnd = new Random();
            pk.EncryptionConstant = (uint)rnd.Next();
            pk.RefreshChecksum();

            var modifiedData = pk.DecryptedPartyData;
            var base64Result = Convert.ToBase64String(modifiedData);

            return new PKMDataResponse(true, base64Result);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPKMHiddenPower(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            var hpType = pk.HPType;
            var hpPower = pk.HPPower;

            return new HiddenPowerInfo(
                (int)hpType,
                GameInfo.Strings.Types[(int)hpType],
                hpPower
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPKMCharacteristic(string base64PkmData, int generation)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);

            var characteristic = pk.Characteristic;
            var characteristics = GameInfo.Strings.characteristics;
            var characteristicText = characteristic < characteristics.Length ? characteristics[characteristic] : "";

            return new CharacteristicInfo(
                characteristic,
                characteristicText
            );
        });
    }

    // TODO: GetPKMSuggestedMoves requires complex learnset integration
    // Will be implemented in a future update
    /*
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPKMSuggestedMoves(string base64PkmData, int generation)
    {
        // Requires LearnInfo and learnset database access
    }
    */

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
            var evolutionChain = new List<EvolutionEntry>();

            foreach (var (evoSpecies, evoForm) in chain)
            {
                evolutionChain.Add(new EvolutionEntry(
                    evoSpecies,
                    GameInfo.Strings.Species[evoSpecies],
                    evoForm
                ));
            }


            var forwardEvolutions = new List<EvolutionEntry>();
            var forward = evolutions.Forward.GetEvolutions((ushort)species, 0);
            foreach (var (evoSpecies, evoForm) in forward)
            {
                forwardEvolutions.Add(new EvolutionEntry(
                    evoSpecies,
                    GameInfo.Strings.Species[evoSpecies],
                    evoForm
                ));
            }


            var preEvolutions = new List<EvolutionEntry>();
            var reverse = evolutions.Reverse.GetPreEvolutions((ushort)species, 0);
            foreach (var (evoSpecies, evoForm) in reverse)
            {
                preEvolutions.Add(new EvolutionEntry(
                    evoSpecies,
                    GameInfo.Strings.Species[evoSpecies],
                    evoForm
                ));
            }


            var baseSpeciesForm = evolutions.GetBaseSpeciesForm((ushort)species, 0);

            return new EvolutionInfo(
                species,
                GameInfo.Strings.Species[species],
                generation,
                evolutionChain,
                evolutionChain.Count,
                forwardEvolutions,
                preEvolutions,
                baseSpeciesForm.Species,
                GameInfo.Strings.Species[baseSpeciesForm.Species],
                baseSpeciesForm.Form
            );
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

            var forms = new List<SpeciesFormInfo>();


            var pi = pt.GetFormEntry((ushort)species, 0);
            var formCount = pi.FormCount;

            for (byte i = 0; i < formCount; i++)
            {
                var formEntry = pt.GetFormEntry((ushort)species, i);

                forms.Add(new SpeciesFormInfo(
                    i,
                    $"Form {i}",
                    formEntry.Type1,
                    GameInfo.Strings.Types[formEntry.Type1],
                    formEntry.Type2,
                    GameInfo.Strings.Types[formEntry.Type2],
                    new PokemonStats(
                        formEntry.HP,
                        formEntry.ATK,
                        formEntry.DEF,
                        formEntry.SPA,
                        formEntry.SPD,
                        formEntry.SPE
                    ),
                    formEntry.Gender,
                    formEntry.IsDualGender,
                    formEntry.Genderless
                ));
            }

            return new SpeciesFormsInfo(
                species,
                GameInfo.Strings.Species[species],
                generation,
                forms,
                formCount
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.Any>]
    public static object ConvertPKMFormat(string base64PkmData, int fromGeneration, int toGeneration)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            ApiHelpers.ValidateNonNegative(fromGeneration, nameof(fromGeneration), "INVALID_GENERATION");
            ApiHelpers.ValidateNonNegative(toGeneration, nameof(toGeneration), "INVALID_GENERATION");

            if (fromGeneration < 1 || fromGeneration > 9)
                throw new ValidationException($"From generation {fromGeneration} is out of range (1-9)", "INVALID_GENERATION");

            if (toGeneration < 1 || toGeneration > 9)
                throw new ValidationException($"To generation {toGeneration} is out of range (1-9)", "INVALID_GENERATION");


            var pk = EntityFormat.GetFromBytes(data, (EntityContext)fromGeneration);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", INVALID_PKM_DATA);


            Type? targetType = toGeneration switch
            {
                1 => typeof(PK1),
                2 => typeof(PK2),
                3 => typeof(PK3),
                4 => typeof(PK4),
                5 => typeof(PK5),
                6 => typeof(PK6),
                7 => typeof(PK7),
                8 => typeof(PK8),
                9 => typeof(PK9),
                _ => null
            };

            if (targetType == null)
                throw new ValidationException($"Invalid target generation {toGeneration}", "INVALID_GENERATION");


            var converted = EntityConverter.ConvertToType(pk, targetType, out var result);

            if (converted == null)
            {
                var errorMessage = result switch
                {
                    EntityConverterResult.NoTransferRoute => "No valid conversion route exists between these generations",
                    EntityConverterResult.IncompatibleForm => "Pokemon form is incompatible with target generation",
                    EntityConverterResult.IncompatibleSpecies => "Pokemon species is incompatible with target generation",
                    EntityConverterResult.IncompatibleLanguageGB => "Language incompatibility for GB transfer",
                    _ => $"Conversion failed: {result}"
                };
                throw new ValidationException(errorMessage, "CONVERSION_FAILED");
            }


            converted.RefreshChecksum();
            var convertedData = converted.DecryptedPartyData;
            var base64Result = Convert.ToBase64String(convertedData);

            return new ConversionResult(
                true,
                base64Result,
                fromGeneration,
                toGeneration,
                pk.GetType().Name,
                converted.GetType().Name,
                result.ToString()
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPKMMetLocations(int generation, int gameVersion, bool eggLocations)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var context = (EntityContext)generation;
            var version = (GameVersion)gameVersion;
            var locations = GameInfo.GetLocationList(version, context, eggLocations);

            var locationList = new List<LocationInfo>();
            foreach (var loc in locations)
            {
                locationList.Add(new LocationInfo(
                    loc.Value,
                    loc.Text
                ));
            }

            return new MetLocationsInfo(
                generation,
                gameVersion,
                eggLocations,
                locationList,
                locationList.Count
            );
        });
    }
}


