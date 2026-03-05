using PKHeX.Core;
using PkhexWorld;
using PkhexWorld.wit.exports.pokality.pkhex;

namespace PkhexWorld.wit.exports.pokality.pkhex;

public class ProgressOpsImpl : IProgressOps
{
    public static IProgressOps.BadgeData GetBadges(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        int badgeBits = save switch
        {
            SAV1 sav1 => sav1.Badges,
            SAV2 sav2 => sav2.Badges,
            SAV3 sav3 => sav3.Badges,
            SAV4 sav4 => sav4.Badges,
            SAV5 sav5 => sav5.Misc.Badges,
            SAV6 sav6 => sav6.Badges,
            SAV7 sav7 => (int)sav7.Misc.Stamps,
            SAV8SWSH sav8 => sav8.Badges,
            _ => throw ImplHelpers.Unsupported("Badges are not supported for this save type"),
        };

        int badgeCount = save switch
        {
            SAV1 or SAV2 or SAV3 or SAV4 or SAV5 or SAV6 or SAV8SWSH => 8,
            SAV7 => 15,
            _ => 8,
        };

        var badges = new List<bool>(badgeCount);
        for (int i = 0; i < badgeCount; i++)
            badges.Add((badgeBits & (1 << i)) != 0);

        int count = 0;
        foreach (var b in badges)
        {
            if (b) count++;
        }

        return new IProgressOps.BadgeData(count, badges);
    }

    public static void SetBadge(uint handle, int badgeIndex, bool value)
    {
        var save = ImplHelpers.GetSave(handle);

        int maxBadges = save switch
        {
            SAV7 => 15,
            SAV1 or SAV2 or SAV3 or SAV4 or SAV5 or SAV6 or SAV8SWSH => 8,
            _ => throw ImplHelpers.Unsupported("Badges are not supported for this save type"),
        };

        if (badgeIndex < 0 || badgeIndex >= maxBadges)
            throw ImplHelpers.Validation($"Badge index must be between 0 and {maxBadges - 1}");

        int bit = 1 << badgeIndex;
        switch (save)
        {
            case SAV1 sav1:
                sav1.Badges = value ? (sav1.Badges | bit) : (sav1.Badges & ~bit);
                break;
            case SAV2 sav2:
                sav2.Badges = value ? (sav2.Badges | bit) : (sav2.Badges & ~bit);
                break;
            case SAV3 sav3:
                sav3.Badges = value ? (sav3.Badges | bit) : (sav3.Badges & ~bit);
                break;
            case SAV4 sav4:
                sav4.Badges = value ? (sav4.Badges | bit) : (sav4.Badges & ~bit);
                break;
            case SAV5 sav5:
                sav5.Misc.Badges = value ? (sav5.Misc.Badges | bit) : (sav5.Misc.Badges & ~bit);
                break;
            case SAV6 sav6:
                sav6.Badges = value ? (sav6.Badges | bit) : (sav6.Badges & ~bit);
                break;
            case SAV7 sav7:
                sav7.Misc.Stamps = value ? (sav7.Misc.Stamps | (uint)bit) : (sav7.Misc.Stamps & ~(uint)bit);
                break;
            case SAV8SWSH sav8:
                sav8.Badges = value ? (sav8.Badges | bit) : (sav8.Badges & ~bit);
                break;
            default:
                throw ImplHelpers.Unsupported("Badges are not supported for this save type");
        }
    }

    public static int GetBattlePoints(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        return save switch
        {
            SAV4 sav4 => sav4.BP,
            SAV5 sav5 => sav5.BattleSubway.BP,
            SAV6 sav6 => sav6.BP,
            SAV7 sav7 => (int)sav7.Misc.BP,
            _ => throw ImplHelpers.Unsupported("Battle Points are not supported for this save type"),
        };
    }

    public static void SetBattlePoints(uint handle, int bp)
    {
        var save = ImplHelpers.GetSave(handle);
        if (bp < 0)
            throw ImplHelpers.Validation("Battle Points must be non-negative");

        switch (save)
        {
            case SAV4 sav4:
                sav4.BP = bp;
                break;
            case SAV5 sav5:
                sav5.BattleSubway.BP = bp;
                break;
            case SAV6 sav6:
                sav6.BP = bp;
                break;
            case SAV7 sav7:
                sav7.Misc.BP = (uint)bp;
                break;
            default:
                throw ImplHelpers.Unsupported("Battle Points are not supported for this save type");
        }
    }

    public static int GetCoins(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        return save switch
        {
            SAV1 sav1 => (int)sav1.Coin,
            SAV2 sav2 => (int)sav2.Coin,
            SAV3 sav3 => (int)sav3.Coin,
            SAV4 sav4 => (int)sav4.Coin,
            _ => throw ImplHelpers.Unsupported("Coins are not supported for this save type"),
        };
    }

    public static void SetCoins(uint handle, int coins)
    {
        var save = ImplHelpers.GetSave(handle);
        if (coins < 0)
            throw ImplHelpers.Validation("Coins must be non-negative");

        switch (save)
        {
            case SAV1 sav1:
                sav1.Coin = (uint)Math.Min(coins, 9999);
                break;
            case SAV2 sav2:
                sav2.Coin = (uint)Math.Min(coins, 9999);
                break;
            case SAV3 sav3:
                sav3.Coin = (uint)Math.Min(coins, 9999);
                break;
            case SAV4 sav4:
                sav4.Coin = (uint)Math.Min(coins, 50000);
                break;
            default:
                throw ImplHelpers.Unsupported("Coins are not supported for this save type");
        }
    }

    public static List<(string, int)> GetRecords(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        var records = new List<(string, int)>();

        switch (save)
        {
            case SAV6 sav6:
            {
                var block = sav6.Records;
                for (int i = 0; i < RecordBlock6.RecordCount; i++)
                    records.Add(($"record_{i}", block.GetRecord(i)));
                break;
            }
            case SAV7 sav7:
            {
                var block = sav7.Records;
                for (int i = 0; i < RecordBlock6.RecordCount; i++)
                    records.Add(($"record_{i}", block.GetRecord(i)));
                break;
            }
            case SAV8SWSH sav8:
            {
                var block = sav8.Records;
                for (int i = 0; i < Record8.RecordCount; i++)
                    records.Add(($"record_{i}", block.GetRecord(i)));
                break;
            }
            default:
                throw ImplHelpers.Unsupported("Records are not supported for this save type");
        }

        return records;
    }

    public static void SetRecord(uint handle, int recordIndex, int value)
    {
        var save = ImplHelpers.GetSave(handle);

        switch (save)
        {
            case SAV6 sav6:
                sav6.Records.SetRecord(recordIndex, value);
                break;
            case SAV7 sav7:
                sav7.Records.SetRecord(recordIndex, value);
                break;
            case SAV8SWSH sav8:
                sav8.Records.SetRecord(recordIndex, value);
                break;
            default:
                throw ImplHelpers.Unsupported("Records are not supported for this save type");
        }
    }

    public static List<IProgressOps.PokedexEntry> GetPokedex(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        var entries = new List<IProgressOps.PokedexEntry>();

        for (ushort species = 1; species <= save.MaxSpeciesID; species++)
        {
            bool seen = save.GetSeen(species);
            bool caught = save.GetCaught(species);
            if (!seen && !caught)
                continue;

            string name = species < GameInfo.Strings.Species.Count
                ? GameInfo.Strings.Species[species]
                : $"Species_{species}";

            entries.Add(new IProgressOps.PokedexEntry(species, name, seen, caught));
        }

        return entries;
    }

    public static void SetPokedexSeen(uint handle, ushort species)
    {
        var save = ImplHelpers.GetSave(handle);
        if (species < 1 || species > save.MaxSpeciesID)
            throw ImplHelpers.Validation($"Species must be between 1 and {save.MaxSpeciesID}");
        save.SetSeen(species, true);
    }

    public static void SetPokedexCaught(uint handle, ushort species)
    {
        var save = ImplHelpers.GetSave(handle);
        if (species < 1 || species > save.MaxSpeciesID)
            throw ImplHelpers.Validation($"Species must be between 1 and {save.MaxSpeciesID}");
        save.SetCaught(species, true);
    }

    public static bool GetEventFlag(uint handle, int flagIndex)
    {
        var save = ImplHelpers.GetSave(handle);

        if (save is IEventFlagArray flagArray)
            return flagArray.GetEventFlag(flagIndex);
        if (save is IEventFlagProvider37 provider)
            return provider.EventWork.GetEventFlag(flagIndex);

        throw ImplHelpers.Unsupported("Event flags are not supported for this save type");
    }

    public static void SetEventFlag(uint handle, int flagIndex, bool value)
    {
        var save = ImplHelpers.GetSave(handle);

        if (save is IEventFlagArray flagArray)
        {
            flagArray.SetEventFlag(flagIndex, value);
            return;
        }
        if (save is IEventFlagProvider37 provider)
        {
            provider.EventWork.SetEventFlag(flagIndex, value);
            return;
        }

        throw ImplHelpers.Unsupported("Event flags are not supported for this save type");
    }

    public static int GetEventConst(uint handle, int constIndex)
    {
        var save = ImplHelpers.GetSave(handle);

        if (save is SAV2 sav2)
            return sav2.GetWork(constIndex);
        if (save is IEventFlagProvider37 { EventWork: IEventWorkArray<ushort> workArray })
            return workArray.GetWork(constIndex);

        throw ImplHelpers.Unsupported("Event constants are not supported for this save type");
    }

    public static void SetEventConst(uint handle, int constIndex, int value)
    {
        var save = ImplHelpers.GetSave(handle);

        if (save is SAV2 sav2)
        {
            sav2.SetWork(constIndex, (byte)value);
            return;
        }
        if (save is IEventFlagProvider37 { EventWork: IEventWorkArray<ushort> workArray })
        {
            workArray.SetWork(constIndex, (ushort)value);
            return;
        }

        throw ImplHelpers.Unsupported("Event constants are not supported for this save type");
    }
}
