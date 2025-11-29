using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;

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

            bool flagValue;

            if (save is IEventFlagArray eventFlagArray)
            {
                flagValue = eventFlagArray.GetEventFlag(flagIndex);
            }
            else if (save is IEventFlagProvider37 provider37)
            {
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

            if (save is IEventFlagArray eventFlagArray)
            {
                eventFlagArray.SetEventFlag(flagIndex, value);
            }
            else if (save is IEventFlagProvider37 provider37)
            {
                provider37.EventWork.SetEventFlag(flagIndex, value);
            }
            else
            {
                throw new ValidationException("Event flags not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new SuccessMessage(true, "Event flag updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetEventConst(int handle, int constIndex)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            int constValue;

            if (save is SAV2 sav2)
            {
                constValue = sav2.GetWork(constIndex);
            }
            else if (save is IEventFlagProvider37 provider37)
            {
                if (provider37.EventWork is IEventWorkArray<ushort> workArray)
                {
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

            if (save is SAV2 sav2)
            {
                sav2.SetWork(constIndex, (byte)value);
            }
            else if (save is IEventFlagProvider37 provider37)
            {
                if (provider37.EventWork is IEventWorkArray<ushort> workArray)
                {
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

            return new SuccessMessage(true, "Event const updated successfully");
        });
    }
}
