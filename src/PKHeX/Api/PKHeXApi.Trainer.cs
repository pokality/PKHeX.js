using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;
using static PKHeX.Models.ErrorCodes;

namespace PKHeX.Api;

// Trainer Operations
public partial class PKHeXApi
{
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

            return new
            {
                ot = save.OT,
                tid = save.DisplayTID,
                sid = save.DisplaySID,
                money = save.Money,
                startDate,
                fame
            };
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

            return new
            {
                skin,
                hair,
                top,
                bottom,
                shoes,
                accessory,
                bag,
                hat
            };
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
}
