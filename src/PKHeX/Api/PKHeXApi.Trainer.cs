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

            var trainerData = JsonSerializer.Deserialize<TrainerInfo>(trainerDataJson, JsonContext.Default.Options);
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

            return new TrainerCard(
                save.OT,
                save.DisplayTID,
                save.DisplaySID,
                save.Money,
                startDate,
                fame
            );
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

            return new TrainerAppearance(
                skin,
                hair,
                top,
                bottom,
                shoes,
                accessory,
                bag,
                hat
            );
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

            var appearance = JsonSerializer.Deserialize<TrainerAppearance>(appearanceJson, JsonContext.Default.Options);
            if (appearance == null)
                throw new ValidationException("Invalid appearance data JSON", INVALID_JSON);

            return new SuccessMessage(true, "Trainer appearance updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPlayerAppearance9a(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (save is not SAV9ZA sav9za)
                throw new ValidationException("Player appearance details are only available in Legends Z-A saves", "UNSUPPORTED_GENERATION");

            var appearance = sav9za.PlayerAppearance;

            return new PlayerAppearance9aResponse(
                true,
                appearance.SkinColor,
                appearance.LipColor,
                appearance.DarkCircles,
                appearance.EyeColor,
                appearance.EyebrowColor,
                appearance.EyebrowShape,
                appearance.EyelashColor,
                appearance.EyelashShape,
                appearance.BeautySpotFirst,
                appearance.BeautySpotSecond,
                appearance.Freckles,
                appearance.HairColor,
                appearance.ColorBlocking,
                appearance.BalayageFadeFirst,
                appearance.BalayageFadeSecond,
                appearance.FaceShape,
                appearance.Bangs,
                appearance.HairColorMode
            );
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetPlayerAppearance9a(int handle, string appearanceJson)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (save is not SAV9ZA sav9za)
                throw new ValidationException("Player appearance details are only available in Legends Z-A saves", "UNSUPPORTED_GENERATION");

            if (string.IsNullOrWhiteSpace(appearanceJson))
                throw new ValidationException("Appearance data cannot be empty", "EMPTY_APPEARANCE_DATA");

            var data = JsonSerializer.Deserialize<PlayerAppearance9aInput>(appearanceJson, JsonContext.Default.Options);
            if (data == null)
                throw new ValidationException("Invalid appearance data JSON", INVALID_JSON);

            var appearance = sav9za.PlayerAppearance;

            if (data.SkinColor.HasValue) appearance.SkinColor = data.SkinColor.Value;
            if (data.LipColor.HasValue) appearance.LipColor = data.LipColor.Value;
            if (data.DarkCircles.HasValue) appearance.DarkCircles = data.DarkCircles.Value;
            if (data.EyeColor.HasValue) appearance.EyeColor = data.EyeColor.Value;
            if (data.EyebrowColor.HasValue) appearance.EyebrowColor = data.EyebrowColor.Value;
            if (data.EyebrowShape.HasValue) appearance.EyebrowShape = data.EyebrowShape.Value;
            if (data.EyelashColor.HasValue) appearance.EyelashColor = data.EyelashColor.Value;
            if (data.EyelashShape.HasValue) appearance.EyelashShape = data.EyelashShape.Value;
            if (data.BeautySpotFirst.HasValue) appearance.BeautySpotFirst = data.BeautySpotFirst.Value;
            if (data.BeautySpotSecond.HasValue) appearance.BeautySpotSecond = data.BeautySpotSecond.Value;
            if (data.Freckles.HasValue) appearance.Freckles = data.Freckles.Value;
            if (data.HairColor.HasValue) appearance.HairColor = data.HairColor.Value;
            if (data.ColorBlocking.HasValue) appearance.ColorBlocking = data.ColorBlocking.Value;
            if (data.BalayageFadeFirst.HasValue) appearance.BalayageFadeFirst = data.BalayageFadeFirst.Value;
            if (data.BalayageFadeSecond.HasValue) appearance.BalayageFadeSecond = data.BalayageFadeSecond.Value;
            if (data.FaceShape.HasValue) appearance.FaceShape = data.FaceShape.Value;
            if (data.Bangs.HasValue) appearance.Bangs = data.Bangs.Value;
            if (data.HairColorMode.HasValue) appearance.HairColorMode = data.HairColorMode.Value;

            return new SuccessMessage(true, "Player appearance updated successfully");
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
