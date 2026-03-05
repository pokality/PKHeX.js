using PKHeX.Core;
using PkhexWorld;
using PkhexWorld.wit.exports.pokality.pkhex;

namespace PkhexWorld.wit.exports.pokality.pkhex;

public static class ImplHelpers
{
    public static SaveFile GetSave(uint handle)
    {
        var save = SaveFileManager.GetSave((int)handle);
        if (save == null)
            throw new WitException<ITypes.PkhexError>(
                ITypes.PkhexError.Validation($"Invalid save handle: {handle}"), 0);
        return save;
    }

    public static PKM GetPokemon(SaveFile save, int box, int slot)
    {
        var pk = save.GetBoxSlotAtIndex(box, slot);
        if (pk.Species == 0)
            throw new WitException<ITypes.PkhexError>(
                ITypes.PkhexError.Validation($"No Pokemon in box {box} slot {slot}"), 0);
        return pk;
    }

    public static PKM ParsePkm(byte[] data, byte generation)
    {
        var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
        if (pk == null)
            throw new WitException<ITypes.PkhexError>(
                ITypes.PkhexError.Validation("Unable to parse Pokemon data"), 0);
        return pk;
    }

    public static ITypes.PokemonDetail CreatePokemonDetail(PKM pk)
    {
        var moveNames = new List<string>
        {
            GameInfo.Strings.Move[pk.Move1],
            GameInfo.Strings.Move[pk.Move2],
            GameInfo.Strings.Move[pk.Move3],
            GameInfo.Strings.Move[pk.Move4]
        };

        return new ITypes.PokemonDetail(
            species: pk.Species,
            speciesName: GameInfo.Strings.Species[pk.Species],
            nickname: pk.Nickname,
            level: pk.CurrentLevel,
            nature: (byte)pk.Nature,
            natureName: GameInfo.Strings.Natures[(int)pk.Nature],
            ability: (ushort)pk.Ability,
            abilityName: GameInfo.Strings.Ability[pk.Ability],
            heldItem: (ushort)pk.HeldItem,
            heldItemName: GameInfo.Strings.Item[pk.HeldItem],
            moves: new ushort[] { pk.Move1, pk.Move2, pk.Move3, pk.Move4 },
            moveNames: moveNames,
            ivs: new ITypes.StatSpread(pk.IV_HP, pk.IV_ATK, pk.IV_DEF, pk.IV_SPA, pk.IV_SPD, pk.IV_SPE),
            evs: new ITypes.StatSpread(pk.EV_HP, pk.EV_ATK, pk.EV_DEF, pk.EV_SPA, pk.EV_SPD, pk.EV_SPE),
            stats: new ITypes.StatSpread(pk.Stat_HPMax, pk.Stat_ATK, pk.Stat_DEF, pk.Stat_SPA, pk.Stat_SPD, pk.Stat_SPE),
            gender: pk.Gender,
            isShiny: pk.IsShiny,
            isEgg: pk.IsEgg,
            otName: pk.OriginalTrainerName,
            otGender: pk.OriginalTrainerGender,
            pid: pk.PID,
            ball: pk.Ball,
            metLevel: pk.MetLevel,
            metLocation: pk.MetLocation,
            metLocationName: GameInfo.GetLocationName(false, pk.MetLocation, pk.Format, pk.Generation, pk.Version)
        );
    }

    public static void ApplyModifications(PKM pk, ITypes.PokemonModifications mods)
    {
        if (mods.species.HasValue)
            pk.Species = mods.species.Value;
        if (mods.nickname != null)
            pk.Nickname = mods.nickname;
        if (mods.level.HasValue)
            pk.CurrentLevel = mods.level.Value;
        if (mods.nature.HasValue)
            pk.Nature = (Nature)mods.nature.Value;
        if (mods.ability.HasValue)
            pk.Ability = mods.ability.Value;
        if (mods.heldItem.HasValue)
            pk.HeldItem = mods.heldItem.Value;
        if (mods.moves != null)
        {
            if (mods.moves.Length > 0) pk.Move1 = mods.moves[0];
            if (mods.moves.Length > 1) pk.Move2 = mods.moves[1];
            if (mods.moves.Length > 2) pk.Move3 = mods.moves[2];
            if (mods.moves.Length > 3) pk.Move4 = mods.moves[3];
        }
        if (mods.ivs != null)
        {
            pk.IV_HP = mods.ivs.hp;
            pk.IV_ATK = mods.ivs.atk;
            pk.IV_DEF = mods.ivs.def;
            pk.IV_SPA = mods.ivs.spa;
            pk.IV_SPD = mods.ivs.spd;
            pk.IV_SPE = mods.ivs.spe;
        }
        if (mods.evs != null)
        {
            pk.EV_HP = mods.evs.hp;
            pk.EV_ATK = mods.evs.atk;
            pk.EV_DEF = mods.evs.def;
            pk.EV_SPA = mods.evs.spa;
            pk.EV_SPD = mods.evs.spd;
            pk.EV_SPE = mods.evs.spe;
        }
        if (mods.gender.HasValue)
            pk.Gender = mods.gender.Value;
        if (mods.isShiny.HasValue && mods.isShiny.Value && !pk.IsShiny)
            CommonEdits.SetShiny(pk, Shiny.AlwaysStar);
        if (mods.otName != null)
            pk.OriginalTrainerName = mods.otName;
        if (mods.ball.HasValue)
            pk.Ball = mods.ball.Value;

        pk.RefreshChecksum();
    }

    public static WitException<ITypes.PkhexError> Unsupported(string message)
    {
        return new WitException<ITypes.PkhexError>(ITypes.PkhexError.Unsupported(message), 0);
    }

    public static WitException<ITypes.PkhexError> Validation(string message)
    {
        return new WitException<ITypes.PkhexError>(ITypes.PkhexError.Validation(message), 0);
    }

    public static WitException<ITypes.PkhexError> InternalError(string message)
    {
        return new WitException<ITypes.PkhexError>(ITypes.PkhexError.InternalError(message), 0);
    }
}
