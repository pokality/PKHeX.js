using PKHeX.Core;
using PkhexWorld;
using PkhexWorld.wit.exports.pokality.pkhex;

namespace PkhexWorld.wit.exports.pokality.pkhex;

public class TrainerOpsImpl : ITrainerOps
{
    public static ITrainerOps.TrainerInfo GetTrainerInfo(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        return new ITrainerOps.TrainerInfo(
            save.OT,
            save.DisplayTID,
            save.DisplaySID,
            (byte)save.Gender,
            save.Language,
            save.Money,
            save.PlayedHours,
            save.PlayedMinutes,
            save.PlayedSeconds
        );
    }

    public static void SetTrainerInfo(uint handle, ITrainerOps.TrainerModifications mods)
    {
        var save = ImplHelpers.GetSave(handle);

        if (mods.ot != null)
            save.OT = mods.ot;
        if (mods.gender.HasValue)
            save.Gender = mods.gender.Value;
        if (mods.language.HasValue)
            save.Language = mods.language.Value;
        if (mods.money.HasValue)
            save.Money = mods.money.Value;
    }

    public static ITrainerOps.TrainerCard GetTrainerCard(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        return new ITrainerOps.TrainerCard(
            save.OT,
            save.DisplayTID,
            save.DisplaySID,
            save.Money
        );
    }

    public static string GetRivalName(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);

        if (save is SAV1 sav1)
            return sav1.Rival;
        if (save is SAV2 sav2)
            return sav2.Rival;
        if (save is SAV4 sav4)
            return sav4.Rival;

        throw ImplHelpers.Unsupported("Rival name is not supported for this save file generation");
    }

    public static void SetRivalName(uint handle, string name)
    {
        var save = ImplHelpers.GetSave(handle);

        if (string.IsNullOrWhiteSpace(name))
            throw ImplHelpers.Validation("Rival name cannot be empty");

        if (save is SAV1 sav1)
            sav1.Rival = name;
        else if (save is SAV2 sav2)
            sav2.Rival = name;
        else if (save is SAV4 sav4)
            sav4.Rival = name;
        else
            throw ImplHelpers.Unsupported("Rival name is not supported for this save file generation");
    }

    public static ITrainerOps.PlayerAppearanceZa GetPlayerAppearanceZa(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);

        if (save is not SAV9ZA sav9za)
            throw ImplHelpers.Unsupported("Player appearance is only available in Legends Z-A saves");

        var appearance = sav9za.PlayerAppearance;
        return new ITrainerOps.PlayerAppearanceZa(
            (byte)appearance.SkinColor,
            (byte)appearance.LipColor,
            (byte)appearance.DarkCircles,
            (byte)appearance.EyeColor,
            (byte)appearance.EyebrowColor,
            (byte)appearance.EyebrowShape,
            (byte)appearance.EyelashColor,
            (byte)appearance.EyelashShape,
            (byte)appearance.BeautySpotFirst,
            (byte)appearance.BeautySpotSecond,
            (byte)appearance.Freckles,
            (byte)appearance.HairColor,
            (byte)appearance.ColorBlocking,
            (byte)appearance.BalayageFadeFirst,
            (byte)appearance.BalayageFadeSecond,
            (byte)appearance.FaceShape,
            (byte)appearance.Bangs,
            (byte)appearance.HairColorMode
        );
    }

    public static void SetPlayerAppearanceZa(uint handle, ITrainerOps.PlayerAppearanceZa appearance)
    {
        var save = ImplHelpers.GetSave(handle);

        if (save is not SAV9ZA sav9za)
            throw ImplHelpers.Unsupported("Player appearance is only available in Legends Z-A saves");

        var pa = sav9za.PlayerAppearance;
        pa.SkinColor = appearance.skinColor;
        pa.LipColor = appearance.lipColor;
        pa.DarkCircles = appearance.darkCircles;
        pa.EyeColor = appearance.eyeColor;
        pa.EyebrowColor = appearance.eyebrowColor;
        pa.EyebrowShape = appearance.eyebrowShape;
        pa.EyelashColor = appearance.eyelashColor;
        pa.EyelashShape = appearance.eyelashShape;
        pa.BeautySpotFirst = appearance.beautySpotFirst;
        pa.BeautySpotSecond = appearance.beautySpotSecond;
        pa.Freckles = appearance.freckles;
        pa.HairColor = appearance.hairColor;
        pa.ColorBlocking = appearance.colorBlocking;
        pa.BalayageFadeFirst = appearance.balayageFadeFirst;
        pa.BalayageFadeSecond = appearance.balayageFadeSecond;
        pa.FaceShape = appearance.faceShape;
        pa.Bangs = appearance.bangs;
        pa.HairColorMode = appearance.hairColorMode;
    }
}
