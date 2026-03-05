using PKHeX.Core;
using PkhexWorld;
using PkhexWorld.wit.exports.pokality.pkhex;

namespace PkhexWorld.wit.exports.pokality.pkhex;

public class FeatureOpsImpl : IFeatureOps
{
    public static List<IFeatureOps.MysteryGiftCard> GetMysteryGifts(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        if (save is not IMysteryGiftStorage giftStorage)
            throw ImplHelpers.Unsupported("Mystery Gifts are not supported for this save type");

        var cards = new List<IFeatureOps.MysteryGiftCard>();
        for (int i = 0; i < giftStorage.GiftCountMax; i++)
        {
            var gift = giftStorage.GetMysteryGift(i);
            if (gift.Species == 0 && gift.ItemID == 0)
                continue;

            cards.Add(CreateCard(i, gift));
        }

        return cards;
    }

    public static IFeatureOps.MysteryGiftCard GetMysteryGiftCard(uint handle, int index)
    {
        var save = ImplHelpers.GetSave(handle);
        if (save is not IMysteryGiftStorage giftStorage)
            throw ImplHelpers.Unsupported("Mystery Gifts are not supported for this save type");

        if (index < 0 || index >= giftStorage.GiftCountMax)
            throw ImplHelpers.Validation($"Mystery Gift index must be between 0 and {giftStorage.GiftCountMax - 1}");

        var gift = giftStorage.GetMysteryGift(index);
        return CreateCard(index, gift);
    }

    public static void DeleteMysteryGift(uint handle, int index)
    {
        var save = ImplHelpers.GetSave(handle);
        if (save is not IMysteryGiftStorage giftStorage)
            throw ImplHelpers.Unsupported("Mystery Gifts are not supported for this save type");

        if (index < 0 || index >= giftStorage.GiftCountMax)
            throw ImplHelpers.Validation($"Mystery Gift index must be between 0 and {giftStorage.GiftCountMax - 1}");

        var gift = giftStorage.GetMysteryGift(index);
        gift.Clear();
        giftStorage.SetMysteryGift(index, gift);
    }

    public static List<bool> GetMysteryGiftFlags(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        if (save is not IMysteryGiftFlags giftFlags)
            throw ImplHelpers.Unsupported("Mystery Gift flags are not supported for this save type");

        var flags = new List<bool>(giftFlags.MysteryGiftReceivedFlagMax);
        for (int i = 0; i < giftFlags.MysteryGiftReceivedFlagMax; i++)
            flags.Add(giftFlags.GetMysteryGiftReceivedFlag(i));

        return flags;
    }

    public static List<IFeatureOps.HallOfFameEntry> GetHallOfFame(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        var entries = new List<IFeatureOps.HallOfFameEntry>();

        switch (save)
        {
            case SAV1 sav1:
            {
                var reader = sav1.HallOfFame;
                for (int t = 0; t < HallOfFameReader1.TeamCount; t++)
                {
                    int memberCount = reader.GetTeamMemberCount(t);
                    if (memberCount == 0)
                        continue;

                    var team = new List<ITypes.PokemonSummary>();
                    for (int s = 0; s < memberCount; s++)
                    {
                        var entity = reader.GetEntity(t, s);
                        string speciesName = entity.Species < GameInfo.Strings.Species.Count
                            ? GameInfo.Strings.Species[entity.Species]
                            : $"Species_{entity.Species}";

                        team.Add(new ITypes.PokemonSummary(
                            box: 0,
                            slot: s,
                            species: entity.Species,
                            speciesName: speciesName,
                            level: entity.Level,
                            isEgg: false,
                            isShiny: false
                        ));
                    }
                    entries.Add(new IFeatureOps.HallOfFameEntry(t, team));
                }
                break;
            }
            case SAV3 sav3:
            {
                var hofEntries = HallFame3Entry.GetEntries(sav3);
                for (int t = 0; t < hofEntries.Length; t++)
                {
                    var hofTeam = hofEntries[t].Team;
                    var team = new List<ITypes.PokemonSummary>();
                    bool hasMembers = false;

                    for (int s = 0; s < hofTeam.Length; s++)
                    {
                        var member = hofTeam[s];
                        if (member.Species == 0)
                            continue;

                        hasMembers = true;
                        string speciesName = member.Species < GameInfo.Strings.Species.Count
                            ? GameInfo.Strings.Species[member.Species]
                            : $"Species_{member.Species}";

                        team.Add(new ITypes.PokemonSummary(
                            box: 0,
                            slot: s,
                            species: member.Species,
                            speciesName: speciesName,
                            level: (byte)member.Level,
                            isEgg: false,
                            isShiny: member.IsShiny
                        ));
                    }

                    if (hasMembers)
                        entries.Add(new IFeatureOps.HallOfFameEntry(t, team));
                }
                break;
            }
            default:
                throw ImplHelpers.Unsupported("Hall of Fame is not supported for this save type");
        }

        return entries;
    }

    public static IFeatureOps.BattleFacilityStats GetBattleFacilityStats(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        var stats = new List<(string, int)>();

        switch (save)
        {
            case SAV5 sav5:
            {
                var subway = sav5.BattleSubway;
                stats.Add(("bp", subway.BP));
                break;
            }
            case SAV6 sav6:
            {
                stats.Add(("bp", sav6.BP));
                break;
            }
            case SAV7 sav7:
            {
                stats.Add(("bp", (int)sav7.Misc.BP));
                break;
            }
            default:
                throw ImplHelpers.Unsupported("Battle Facility stats are not supported for this save type");
        }

        return new IFeatureOps.BattleFacilityStats(stats);
    }

    public static IFeatureOps.SecretBaseData GetSecretBase(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);

        switch (save)
        {
            case SAV3 sav3:
                return new IFeatureOps.SecretBaseData(
                    trainerName: sav3.OT,
                    trainerId: (int)sav3.DisplayTID,
                    secretId: (int)sav3.DisplaySID,
                    gender: sav3.Gender,
                    language: sav3.Language,
                    locationName: "Secret Base",
                    locationId: 0
                );
            case SAV4 sav4:
                return new IFeatureOps.SecretBaseData(
                    trainerName: sav4.OT,
                    trainerId: (int)sav4.DisplayTID,
                    secretId: (int)sav4.DisplaySID,
                    gender: sav4.Gender,
                    language: sav4.Language,
                    locationName: "Underground Base",
                    locationId: 0
                );
            case SAV6AO sav6ao:
            {
                var sb = sav6ao.SecretBase;
                var self = sb.GetSecretBaseSelf();
                return new IFeatureOps.SecretBaseData(
                    trainerName: sav6ao.OT,
                    trainerId: (int)sav6ao.DisplayTID,
                    secretId: (int)sav6ao.DisplaySID,
                    gender: sav6ao.Gender,
                    language: sav6ao.Language,
                    locationName: "Secret Base",
                    locationId: sb.SecretBaseSelfLocation
                );
            }
            default:
                throw ImplHelpers.Unsupported("Secret Base is not supported for this save type");
        }
    }

    private static IFeatureOps.MysteryGiftCard CreateCard(int index, DataMysteryGift gift)
    {
        return new IFeatureOps.MysteryGiftCard(
            index: index,
            giftType: gift.Type,
            cardTitle: gift.CardTitle,
            isItem: gift.IsItem,
            isPokemon: gift.IsEntity,
            itemId: gift.ItemID,
            species: gift.Species,
            level: gift.Level,
            isShiny: gift.IsShiny,
            isEgg: gift.IsEgg
        );
    }
}
