using System.Reflection;
using PKHeX.Core;
using PkhexWorld;
using PkhexWorld.wit.exports.pokality.pkhex;

namespace PkhexWorld.wit.exports.pokality.pkhex;

public class PkmOpsImpl : IPkmOps
{
    public static ITypes.PokemonDetail GetPkmData(byte[] data, byte generation)
    {
        var pk = ImplHelpers.ParsePkm(data, generation);
        return ImplHelpers.CreatePokemonDetail(pk);
    }

    public static byte[] ModifyPkmData(byte[] data, byte generation, ITypes.PokemonModifications mods)
    {
        var pk = ImplHelpers.ParsePkm(data, generation);
        ImplHelpers.ApplyModifications(pk, mods);
        return pk.DecryptedPartyData;
    }

    public static ITypes.StatSpread CalculatePkmStats(byte[] data, byte generation)
    {
        var pk = ImplHelpers.ParsePkm(data, generation);
        return new ITypes.StatSpread(
            pk.Stat_HPMax, pk.Stat_ATK, pk.Stat_DEF,
            pk.Stat_SPA, pk.Stat_SPD, pk.Stat_SPE
        );
    }

    public static ITypes.PidInfo GetPkmPidInfo(byte[] data, byte generation)
    {
        var pk = ImplHelpers.ParsePkm(data, generation);
        var shinyType = pk.ShinyXor == 0 ? "Square" : pk.IsShiny ? "Star" : "None";
        var gender = pk.Gender;
        var genderName = gender == 0 ? "Male" : gender == 1 ? "Female" : "Genderless";

        return new ITypes.PidInfo(
            pk.PID, pk.IsShiny, shinyType,
            (byte)pk.Nature, pk.Nature.ToString(),
            gender, genderName
        );
    }

    public static byte[] SetPkmPid(byte[] data, byte generation, uint pid)
    {
        var pk = ImplHelpers.ParsePkm(data, generation);
        pk.PID = pid;
        pk.RefreshAbility(pk.AbilityNumber >> 1);
        pk.RefreshChecksum();
        return pk.DecryptedPartyData;
    }

    public static byte[] SetPkmShiny(byte[] data, byte generation, byte shinyType)
    {
        var pk = ImplHelpers.ParsePkm(data, generation);
        var type = (Shiny)shinyType;

        if (type == Shiny.Never)
            pk.SetUnshiny();
        else
            CommonEdits.SetShiny(pk, type);

        pk.RefreshChecksum();
        return pk.DecryptedPartyData;
    }

    public static ITypes.LegalityResult CheckPkmLegality(byte[] data, byte generation)
    {
        var pk = ImplHelpers.ParsePkm(data, generation);
        var analysis = new LegalityAnalysis(pk);
        var valid = analysis.Valid;

        var localizer = LegalityLocalizationContext.Create(analysis);
        var errors = new List<string>();
        foreach (var result in analysis.Results)
        {
            if (!result.Valid)
                errors.Add(localizer.Humanize(result));
        }

        var report = analysis.Report();
        return new ITypes.LegalityResult(valid, errors, report);
    }

    public static byte[] LegalizePkmData(byte[] data, byte generation)
    {
        var pk = ImplHelpers.ParsePkm(data, generation);
        pk.SetMoveset();
        pk.Heal();
        pk.RefreshChecksum();
        return pk.DecryptedPartyData;
    }

    public static string ExportPkmShowdown(byte[] data, byte generation)
    {
        var pk = ImplHelpers.ParsePkm(data, generation);
        return ShowdownParsing.GetShowdownText(pk);
    }

    public static List<ITypes.RibbonEntry> GetPkmRibbons(byte[] data, byte generation)
    {
        var pk = ImplHelpers.ParsePkm(data, generation);
        var ribbons = new List<ITypes.RibbonEntry>();

        foreach (var ribbon in RibbonInfo.GetRibbonInfo(pk))
        {
            ribbons.Add(new ITypes.RibbonEntry(
                ribbon.Name,
                ribbon.HasRibbon,
                ribbon.RibbonCount,
                ribbon.Type.ToString()
            ));
        }

        return ribbons;
    }

    public static byte[] SetPkmRibbon(byte[] data, byte generation, string ribbonName, bool value)
    {
        var pk = ImplHelpers.ParsePkm(data, generation);
        var prop = pk.GetType().GetProperty(ribbonName,
            BindingFlags.Public | BindingFlags.Instance);

        if (prop == null)
            throw ImplHelpers.Validation($"Ribbon property '{ribbonName}' not found");

        if (prop.PropertyType == typeof(bool))
            prop.SetValue(pk, value);
        else if (prop.PropertyType == typeof(byte))
            prop.SetValue(pk, value ? (byte)1 : (byte)0);
        else
            throw ImplHelpers.Validation($"Ribbon property '{ribbonName}' has unsupported type {prop.PropertyType}");

        pk.RefreshChecksum();
        return pk.DecryptedPartyData;
    }

    public static byte[] RerollPkmEncryptionConstant(byte[] data, byte generation)
    {
        var pk = ImplHelpers.ParsePkm(data, generation);
        pk.EncryptionConstant = (uint)new Random().Next();
        pk.RefreshChecksum();
        return pk.DecryptedPartyData;
    }

    public static ITypes.HiddenPowerInfo GetPkmHiddenPower(byte[] data, byte generation)
    {
        var pk = ImplHelpers.ParsePkm(data, generation);
        return new ITypes.HiddenPowerInfo(
            (int)pk.HPType,
            GameInfo.Strings.Types[(int)pk.HPType],
            pk.HPPower
        );
    }

    public static ITypes.CharacteristicInfo GetPkmCharacteristic(byte[] data, byte generation)
    {
        var pk = ImplHelpers.ParsePkm(data, generation);
        var index = pk.Characteristic;
        var characteristics = GameInfo.Strings.characteristics;
        var description = index >= 0 && index < characteristics.Length
            ? characteristics[index]
            : "";

        return new ITypes.CharacteristicInfo(index, description);
    }

    public static ITypes.TeraTypeData GetPkmTeraType(byte[] data, byte generation)
    {
        var pk = ImplHelpers.ParsePkm(data, generation);

        if (pk is not ITeraTypeReadOnly tera)
            throw ImplHelpers.Unsupported("Tera type is only available for Generation 9 Pokemon");

        var teraType = tera.TeraType;
        var effectiveType = pk is ITeraType teraFull ? teraFull.GetTeraType() : teraType;
        var isOverridden = teraType != effectiveType;
        var types = GameInfo.Strings.Types;

        var teraTypeName = (int)teraType == TeraTypeUtil.Stellar
            ? "Stellar"
            : (int)teraType < types.Count ? types[(int)teraType] : teraType.ToString();

        var effectiveTypeName = (int)effectiveType == TeraTypeUtil.Stellar
            ? "Stellar"
            : (int)effectiveType < types.Count ? types[(int)effectiveType] : effectiveType.ToString();

        return new ITypes.TeraTypeData(
            (byte)teraType, teraTypeName,
            (byte)effectiveType, effectiveTypeName,
            isOverridden
        );
    }

    public static byte[] ConvertPkmFormat(byte[] data, byte fromGeneration, byte toGeneration)
    {
        var pk = ImplHelpers.ParsePkm(data, fromGeneration);

        Type targetType = toGeneration switch
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
            _ => throw ImplHelpers.Validation($"Unsupported target generation: {toGeneration}")
        };

        var result = EntityConverter.ConvertToType(pk, targetType, out var comment);
        if (result == null)
            throw ImplHelpers.Validation($"Conversion failed: {comment}");

        return result.DecryptedPartyData;
    }

    public static List<ITypes.NamedEntry> GetMetLocations(byte generation, int gameVersion, bool eggLocations)
    {
        var locations = GameInfo.GetLocationList(
            (GameVersion)gameVersion,
            (EntityContext)generation,
            eggLocations
        );

        var entries = new List<ITypes.NamedEntry>();
        foreach (var loc in locations)
            entries.Add(new ITypes.NamedEntry(loc.Value, loc.Text));

        return entries;
    }
}
