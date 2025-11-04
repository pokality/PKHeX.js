using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;
using static PKHeX.Models.ErrorCodes;

namespace PKHeX.Api;

// Progress/Badges/Points Operations
public partial class PKHeXApi
{
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetBadges(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            var badgeList = new List<bool>();
            int badgeCount = 0;

            if (save is SAV1 sav1)
            {
                for (int i = 0; i < 8; i++)
                {
                    bool hasBadge = (sav1.Badges & (1 << i)) != 0;
                    badgeList.Add(hasBadge);
                    if (hasBadge) badgeCount++;
                }
            }
            else if (save is SAV2 sav2)
            {
                for (int i = 0; i < 16; i++)
                {
                    bool hasBadge = (sav2.Badges & (1 << i)) != 0;
                    badgeList.Add(hasBadge);
                    if (hasBadge) badgeCount++;
                }
            }
            else if (save is SAV3 sav3)
            {
                for (int i = 0; i < 8; i++)
                {
                    bool hasBadge = (sav3.Badges & (1 << i)) != 0;
                    badgeList.Add(hasBadge);
                    if (hasBadge) badgeCount++;
                }
            }
            else if (save is SAV4 sav4)
            {
                for (int i = 0; i < 8; i++)
                {
                    bool hasBadge = (sav4.Badges & (1 << i)) != 0;
                    badgeList.Add(hasBadge);
                    if (hasBadge) badgeCount++;
                }
            }
            else if (save is SAV5 sav5)
            {
                int badges = sav5.Misc.Badges;
                for (int i = 0; i < 8; i++)
                {
                    bool hasBadge = (badges & (1 << i)) != 0;
                    badgeList.Add(hasBadge);
                    if (hasBadge) badgeCount++;
                }
            }
            else if (save is SAV6 sav6)
            {
                for (int i = 0; i < 8; i++)
                {
                    bool hasBadge = (sav6.Badges & (1 << i)) != 0;
                    badgeList.Add(hasBadge);
                    if (hasBadge) badgeCount++;
                }
            }
            else if (save is SAV7 sav7)
            {
                uint stamps = sav7.Misc.Stamps;
                for (int i = 0; i < 8; i++)
                {
                    bool hasBadge = (stamps & (1u << i)) != 0;
                    badgeList.Add(hasBadge);
                    if (hasBadge) badgeCount++;
                }
            }
            else if (save is SAV8SWSH sav8)
            {
                for (int i = 0; i < 8; i++)
                {
                    bool hasBadge = (sav8.Badges & (1 << i)) != 0;
                    badgeList.Add(hasBadge);
                    if (hasBadge) badgeCount++;
                }
            }
            else
            {
                throw new ValidationException("Badges not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new
            {
                badgeCount,
                badges = badgeList.ToArray()
            };
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetBadge(int handle, int badgeIndex, bool value)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (badgeIndex < 0)
                throw new ValidationException($"Badge index {badgeIndex} must be non-negative", "INVALID_BADGE_INDEX");

            if (save is SAV1 sav1)
            {
                if (badgeIndex >= 8)
                    throw new ValidationException($"Badge index {badgeIndex} is out of range (0-7)", "INVALID_BADGE_INDEX");
                if (value)
                    sav1.Badges |= (byte)(1 << badgeIndex);
                else
                    sav1.Badges &= (byte)~(1 << badgeIndex);
            }
            else if (save is SAV2 sav2)
            {
                if (badgeIndex >= 16)
                    throw new ValidationException($"Badge index {badgeIndex} is out of range (0-15)", "INVALID_BADGE_INDEX");
                if (value)
                    sav2.Badges |= (ushort)(1 << badgeIndex);
                else
                    sav2.Badges &= (ushort)~(1 << badgeIndex);
            }
            else if (save is SAV3 sav3)
            {
                if (badgeIndex >= 8)
                    throw new ValidationException($"Badge index {badgeIndex} is out of range (0-7)", "INVALID_BADGE_INDEX");
                if (value)
                    sav3.Badges |= (byte)(1 << badgeIndex);
                else
                    sav3.Badges &= (byte)~(1 << badgeIndex);
            }
            else if (save is SAV4 sav4)
            {
                if (badgeIndex >= 8)
                    throw new ValidationException($"Badge index {badgeIndex} is out of range (0-7)", "INVALID_BADGE_INDEX");
                if (value)
                    sav4.Badges |= (byte)(1 << badgeIndex);
                else
                    sav4.Badges &= (byte)~(1 << badgeIndex);
            }
            else if (save is SAV5 sav5)
            {
                if (badgeIndex >= 8)
                    throw new ValidationException($"Badge index {badgeIndex} is out of range (0-7)", "INVALID_BADGE_INDEX");
                int badges = sav5.Misc.Badges;
                if (value)
                    badges |= (1 << badgeIndex);
                else
                    badges &= ~(1 << badgeIndex);
                sav5.Misc.Badges = badges;
            }
            else if (save is SAV6 sav6)
            {
                if (badgeIndex >= 8)
                    throw new ValidationException($"Badge index {badgeIndex} is out of range (0-7)", "INVALID_BADGE_INDEX");
                if (value)
                    sav6.Badges |= (byte)(1 << badgeIndex);
                else
                    sav6.Badges &= (byte)~(1 << badgeIndex);
            }
            else if (save is SAV7 sav7)
            {
                if (badgeIndex >= 8)
                    throw new ValidationException($"Badge index {badgeIndex} is out of range (0-7)", "INVALID_BADGE_INDEX");
                uint stamps = sav7.Misc.Stamps;
                if (value)
                    stamps |= (1u << badgeIndex);
                else
                    stamps &= ~(1u << badgeIndex);
                sav7.Misc.Stamps = stamps;
            }
            else if (save is SAV8SWSH sav8)
            {
                if (badgeIndex >= 8)
                    throw new ValidationException($"Badge index {badgeIndex} is out of range (0-7)", "INVALID_BADGE_INDEX");
                if (value)
                    sav8.Badges |= (byte)(1 << badgeIndex);
                else
                    sav8.Badges &= (byte)~(1 << badgeIndex);
            }
            else
            {
                throw new ValidationException("Badges not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new SuccessMessage(true, "Badge updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetBattlePoints(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            int battlePoints = 0;

            if (save is SAV4 sav4)
            {
                battlePoints = sav4.BP;
            }
            else if (save is SAV5 sav5)
            {
                battlePoints = sav5.BattleSubway.BP;
            }
            else if (save is SAV6 sav6)
            {
                battlePoints = sav6.BP;
            }
            else if (save is SAV7 sav7)
            {
                battlePoints = (int)sav7.Misc.BP;
            }
            else
            {
                throw new ValidationException("Battle Points not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new BattlePointsResponse(true, battlePoints);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetBattlePoints(int handle, int battlePoints)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (battlePoints < 0)
                throw new ValidationException($"Battle Points {battlePoints} must be non-negative", "INVALID_BATTLE_POINTS");

            if (save is SAV4 sav4)
            {
                sav4.BP = battlePoints;
            }
            else if (save is SAV5 sav5)
            {
                sav5.BattleSubway.BP = battlePoints;
            }
            else if (save is SAV6 sav6)
            {
                sav6.BP = battlePoints;
            }
            else if (save is SAV7 sav7)
            {
                sav7.Misc.BP = (uint)battlePoints;
            }
            else
            {
                throw new ValidationException("Battle Points not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new SuccessMessage(true, "Battle Points updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetCoins(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            int coins = 0;

            if (save is SAV1 sav1)
            {
                coins = (int)sav1.Coin;
            }
            else if (save is SAV2 sav2)
            {
                coins = (int)sav2.Coin;
            }
            else if (save is SAV3 sav3)
            {
                coins = (int)sav3.Coin;
            }
            else if (save is SAV4 sav4)
            {
                coins = (int)sav4.Coin;
            }
            else
            {
                throw new ValidationException("Coins not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new CoinsResponse(true, coins);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetCoins(int handle, int coins)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (coins < 0)
                throw new ValidationException($"Coins {coins} must be non-negative", "INVALID_COINS");

            if (save is SAV1 sav1)
            {
                sav1.Coin = (uint)Math.Min(coins, 9999);
            }
            else if (save is SAV2 sav2)
            {
                sav2.Coin = (uint)Math.Min(coins, 9999);
            }
            else if (save is SAV3 sav3)
            {
                sav3.Coin = (uint)Math.Min(coins, 9999);
            }
            else if (save is SAV4 sav4)
            {
                sav4.Coin = (uint)Math.Min(coins, 50000);
            }
            else
            {
                throw new ValidationException("Coins not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new SuccessMessage(true, "Coins updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetRecords(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            var records = new Dictionary<string, int>();

            if (save is SAV6 sav6)
            {
                var recordBlock = sav6.Records;
                for (int i = 0; i < RecordBlock6.RecordCount; i++)
                {
                    records[$"record_{i}"] = recordBlock.GetRecord(i);
                }
            }
            else if (save is SAV7 sav7)
            {
                var recordBlock = sav7.Records;
                for (int i = 0; i < RecordBlock6.RecordCount; i++)
                {
                    records[$"record_{i}"] = recordBlock.GetRecord(i);
                }
            }
            else if (save is SAV8SWSH sav8)
            {
                var recordBlock = sav8.Records;
                for (int i = 0; i < Record8.RecordCount; i++)
                {
                    records[$"record_{i}"] = recordBlock.GetRecord(i);
                }
            }
            else
            {
                throw new ValidationException("Records not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new RecordsResponse(true, records);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetRecord(int handle, int recordIndex, int value)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            ApiHelpers.ValidateNonNegative(recordIndex, "Record index", "INVALID_RECORD_INDEX");
            ApiHelpers.ValidateNonNegative(value, "Record value", "INVALID_RECORD_VALUE");

            if (save is SAV6 sav6)
            {
                var recordBlock = sav6.Records;
                ApiHelpers.ValidateRange(recordIndex, 0, RecordBlock6.RecordCount - 1, "Record index", "INVALID_RECORD_INDEX");
                recordBlock.SetRecord(recordIndex, value);
            }
            else if (save is SAV7 sav7)
            {
                var recordBlock = sav7.Records;
                ApiHelpers.ValidateRange(recordIndex, 0, RecordBlock6.RecordCount - 1, "Record index", "INVALID_RECORD_INDEX");
                recordBlock.SetRecord(recordIndex, value);
            }
            else if (save is SAV8SWSH sav8)
            {
                var recordBlock = sav8.Records;
                ApiHelpers.ValidateRange(recordIndex, 0, Record8.RecordCount - 1, "Record index", "INVALID_RECORD_INDEX");
                recordBlock.SetRecord(recordIndex, value);
            }
            else
            {
                throw new ValidationException("Records not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return ApiHelpers.SuccessMessage("Record updated successfully");
        });
    }
}
