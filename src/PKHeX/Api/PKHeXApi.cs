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

    // Shared helper methods

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
}
