using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;
using static PKHeX.Models.ErrorCodes;

namespace PKHeX.Api;

// Time Operations
public partial class PKHeXApi
{
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
}
