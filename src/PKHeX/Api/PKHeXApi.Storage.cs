using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;
using static PKHeX.Models.ErrorCodes;

namespace PKHeX.Api;

// Storage Operations
public partial class PKHeXApi
{
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetBoxNames(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            var boxNames = new List<string>();
            for (int i = 0; i < save.BoxCount; i++)
            {
                if (save is IBoxDetailNameRead nameRead)
                    boxNames.Add(nameRead.GetBoxName(i));
                else
                    boxNames.Add(BoxDetailNameExtensions.GetDefaultBoxName(i));
            }

            return boxNames;
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetBoxWallpapers(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            var boxInfoList = new List<object>();
            for (int i = 0; i < save.BoxCount; i++)
            {
                string boxName = save is IBoxDetailNameRead nameRead
                    ? nameRead.GetBoxName(i)
                    : BoxDetailNameExtensions.GetDefaultBoxName(i);

                int wallpaper = save is IBoxDetailWallpaper wallpaperRead
                    ? wallpaperRead.GetBoxWallpaper(i)
                    : 0;

                boxInfoList.Add(new
                {
                    name = boxName,
                    wallpaper
                });
            }

            return boxInfoList;
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetBoxWallpaper(int handle, int box, int wallpaperId)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            ApiHelpers.ValidateBox(save, box);

            if (wallpaperId < 0)
                throw new ValidationException($"Wallpaper ID {wallpaperId} is invalid", "INVALID_WALLPAPER");

            if (save is not IBoxDetailWallpaper wallpaperWrite)
                throw new ValidationException("Box wallpapers are not supported for this save file generation", "UNSUPPORTED_GENERATION");

            wallpaperWrite.SetBoxWallpaper(box, wallpaperId);

            return new SuccessMessage(true, "Box wallpaper updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetBattleBox(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            var battleBoxList = new List<object>();

            if (save is SAV5 sav5)
            {
                var battleBox = sav5.BattleBox;
                for (int i = 0; i < 6; i++)
                {
                    var pkData = battleBox[i];
                    var pk = save.GetDecryptedPKM(pkData.ToArray());
                    if (pk.Species == 0)
                        continue;

                    battleBoxList.Add(new
                    {
                        box = -2,
                        slot = i,
                        species = pk.Species,
                        speciesName = GameInfo.Strings.Species[pk.Species],
                        level = pk.CurrentLevel,
                        isEgg = pk.IsEgg,
                        isShiny = pk.IsShiny
                    });
                }
            }
            else if (save is SAV6XY sav6xy)
            {
                var battleBox = sav6xy.BattleBox;
                for (int i = 0; i < 6; i++)
                {
                    var pkData = battleBox[i];
                    var pk = save.GetDecryptedPKM(pkData.ToArray());
                    if (pk.Species == 0)
                        continue;

                    battleBoxList.Add(new
                    {
                        box = -2,
                        slot = i,
                        species = pk.Species,
                        speciesName = GameInfo.Strings.Species[pk.Species],
                        level = pk.CurrentLevel,
                        isEgg = pk.IsEgg,
                        isShiny = pk.IsShiny
                    });
                }
            }
            else if (save is SAV6AO sav6ao)
            {
                var battleBox = sav6ao.BattleBox;
                for (int i = 0; i < 6; i++)
                {
                    var pkData = battleBox[i];
                    var pk = save.GetDecryptedPKM(pkData.ToArray());
                    if (pk.Species == 0)
                        continue;

                    battleBoxList.Add(new
                    {
                        box = -2,
                        slot = i,
                        species = pk.Species,
                        speciesName = GameInfo.Strings.Species[pk.Species],
                        level = pk.CurrentLevel,
                        isEgg = pk.IsEgg,
                        isShiny = pk.IsShiny
                    });
                }
            }
            else
            {
                throw new ValidationException("Battle Box is only supported for Generation 5-6 saves", "UNSUPPORTED_GENERATION");
            }

            return battleBoxList;
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetBattleBoxSlot(int handle, int slot, string base64PkmData)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (slot < 0 || slot >= 6)
                throw new ValidationException($"Battle Box slot {slot} is out of range (0-5)", INVALID_SLOT);

            byte[] data;
            try
            {
                data = Convert.FromBase64String(base64PkmData);
            }
            catch (FormatException)
            {
                throw new ValidationException("Invalid base64 encoding", INVALID_BASE64);
            }

            var pk = save.GetDecryptedPKM(data);
            if (pk.Species == 0)
                throw new ValidationException("Invalid Pokemon data", INVALID_PKM_DATA);

            if (save is SAV5 sav5)
            {
                var battleBox = sav5.BattleBox;
                var encData = pk.EncryptedBoxData;
                encData.CopyTo(battleBox[slot].Span);
            }
            else if (save is SAV6XY sav6xy)
            {
                var battleBox = sav6xy.BattleBox;
                var encData = pk.EncryptedBoxData;
                encData.CopyTo(battleBox[slot].Span);
            }
            else if (save is SAV6AO sav6ao)
            {
                var battleBox = sav6ao.BattleBox;
                var encData = pk.EncryptedBoxData;
                encData.CopyTo(battleBox[slot].Span);
            }
            else
            {
                throw new ValidationException("Battle Box is only supported for Generation 5-6 saves", "UNSUPPORTED_GENERATION");
            }

            return new SuccessMessage(true, "Battle Box slot updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetDaycare(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            int slot1Species = 0, slot1Level = 0;
            string slot1SpeciesName = string.Empty;
            int slot2Species = 0, slot2Level = 0;
            string slot2SpeciesName = string.Empty;
            bool hasEgg = false;

            if (save is IDaycareStorage daycare)
            {
                var slot1Data = daycare.GetDaycareSlot(0);
                var pk1 = save.GetDecryptedPKM(slot1Data.ToArray());
                if (pk1.Species != 0)
                {
                    slot1Species = pk1.Species;
                    slot1SpeciesName = GameInfo.Strings.Species[pk1.Species];
                    slot1Level = pk1.CurrentLevel;
                }

                if (daycare.DaycareSlotCount > 1)
                {
                    var slot2Data = daycare.GetDaycareSlot(1);
                    var pk2 = save.GetDecryptedPKM(slot2Data.ToArray());
                    if (pk2.Species != 0)
                    {
                        slot2Species = pk2.Species;
                        slot2SpeciesName = GameInfo.Strings.Species[pk2.Species];
                        slot2Level = pk2.CurrentLevel;
                    }
                }

                if (save is IDaycareEggState eggState)
                {
                    hasEgg = eggState.IsEggAvailable;
                }
            }
            else
            {
                throw new ValidationException("Daycare not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new
            {
                slot1Species,
                slot1SpeciesName,
                slot1Level,
                slot2Species,
                slot2SpeciesName,
                slot2Level,
                hasEgg
            };
        });
    }
}
