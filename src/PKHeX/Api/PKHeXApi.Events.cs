using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;
using static PKHeX.Models.ErrorCodes;

namespace PKHeX.Api;

// Event Flags/Consts Operations
public partial class PKHeXApi
{
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
}
