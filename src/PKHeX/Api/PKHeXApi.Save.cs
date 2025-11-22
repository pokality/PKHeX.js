using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;
using static PKHeX.Models.ErrorCodes;

namespace PKHeX.Api;

// Save File Operations
public partial class PKHeXApi
{
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
}
