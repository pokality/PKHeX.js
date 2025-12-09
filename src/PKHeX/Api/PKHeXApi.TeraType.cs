using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;

namespace PKHeX.Api;

// Tera Type Operations (Gen 9)
public partial class PKHeXApi
{
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetTeraType(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (pk is not ITeraTypeReadOnly teraPk)
                throw new ValidationException("This Pokemon does not support Tera Type (Gen 9 only)", "UNSUPPORTED_FEATURE");

            var teraType = teraPk.TeraType;
            var teraTypeEffective = pk is ITeraType teraFull ? teraFull.GetTeraType() : teraType;
            var isOverridden = teraType != teraTypeEffective;

            return new TeraTypeData(
                (int)teraType,
                GetTeraTypeName(teraType),
                (int)teraTypeEffective,
                GetTeraTypeName(teraTypeEffective),
                isOverridden
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetTeraType(int handle, int box, int slot, int teraType)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (pk is not ITeraType teraPk)
                throw new ValidationException("This Pokemon does not support Tera Type (Gen 9 only)", "UNSUPPORTED_FEATURE");

            // Tera types: 0-17 are standard types, 99 is Stellar (SV DLC)
            if (teraType < 0 || (teraType > 17 && teraType != TeraTypeUtil.Stellar))
                throw new ValidationException($"Tera Type {teraType} is out of range (0-17 or 99 for Stellar)", "INVALID_TERA_TYPE");

            teraPk.SetTeraType((MoveType)teraType);
            pk.RefreshChecksum();
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, $"Tera Type set to {GetTeraTypeName((MoveType)teraType)}");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetTeraTypeOverride(int handle, int box, int slot, int teraType)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (pk is not ITeraType teraPk)
                throw new ValidationException("This Pokemon does not support Tera Type override", "UNSUPPORTED_FEATURE");

            // Tera types: 0-17 are standard types, 99 is Stellar (SV DLC), 19 to reset
            if (teraType != TeraTypeUtil.OverrideNone && teraType != TeraTypeUtil.Stellar && (teraType < 0 || teraType > 17))
                throw new ValidationException($"Tera Type {teraType} is out of range (0-17, 19 to reset, or 99 for Stellar)", "INVALID_TERA_TYPE");

            teraPk.SetTeraType((MoveType)teraType);
            pk.RefreshChecksum();
            save.SetBoxSlotAtIndex(pk, box, slot);

            var typeName = teraType == TeraTypeUtil.OverrideNone ? "original" : GetTeraTypeName((MoveType)teraType);
            return new SuccessMessage(true, $"Tera Type override set to {typeName}");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string ResetTeraType(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (pk is not ITeraType teraPk)
                throw new ValidationException("This Pokemon does not support Tera Type reset", "UNSUPPORTED_FEATURE");

            // OverrideNone (19) resets to original
            teraPk.TeraTypeOverride = (MoveType)TeraTypeUtil.OverrideNone;
            pk.RefreshChecksum();
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Tera Type reset to original");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetAllTeraTypes()
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var types = new List<TeraTypeInfo>();

            // Standard types (0-17)
            var typeNames = GameInfo.Strings.Types;
            for (int i = 0; i < 18 && i < typeNames.Count; i++)
            {
                types.Add(new TeraTypeInfo(i, typeNames[i], false));
            }

            // Stellar type (99) - added in Indigo Disk DLC
            types.Add(new TeraTypeInfo(TeraTypeUtil.Stellar, "Stellar", true));

            return new TeraTypesListResponse(types);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPKMTeraType(string base64PkmData, int generation)
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
                throw new ValidationException("Invalid base64 encoding", "INVALID_BASE64");
            }

            if (generation < 1 || generation > 9)
                throw new ValidationException($"Generation {generation} is out of range (1-9)", "INVALID_GENERATION");

            var pk = EntityFormat.GetFromBytes(data, (EntityContext)generation);
            if (pk == null)
                throw new ValidationException("Unable to parse Pokemon data", "INVALID_PKM_DATA");

            if (pk is not ITeraTypeReadOnly teraPk)
                throw new ValidationException("This Pokemon does not support Tera Type (Gen 9 only)", "UNSUPPORTED_FEATURE");

            var teraType = teraPk.TeraType;
            var teraTypeEffective = pk is ITeraType teraFull ? teraFull.GetTeraType() : teraType;
            var isOverridden = teraType != teraTypeEffective;

            return new TeraTypeData(
                (int)teraType,
                GetTeraTypeName(teraType),
                (int)teraTypeEffective,
                GetTeraTypeName(teraTypeEffective),
                isOverridden
            );
        });
    }

    private static string GetTeraTypeName(MoveType type)
    {
        var typeIndex = (int)type;
        if (typeIndex == TeraTypeUtil.Stellar)
            return "Stellar";

        var typeNames = GameInfo.Strings.Types;
        return typeIndex >= 0 && typeIndex < typeNames.Count ? typeNames[typeIndex] : $"Type {typeIndex}";
    }
}
