using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;
using static PKHeX.Models.ErrorCodes;

namespace PKHeX.Api;

// Special Features Operations
public partial class PKHeXApi
{
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetBattleFacilityStats(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            var stats = new Dictionary<string, object>();

            if (save is SAV5 sav5)
            {
                var subway = sav5.BattleSubway;
                stats["bp"] = subway.BP;
                stats["superSingle"] = subway.SuperSingle;
                stats["superDouble"] = subway.SuperDouble;
                stats["superMulti"] = subway.SuperMulti;
                stats["singlePast"] = subway.SinglePast;
                stats["doublePast"] = subway.DoublePast;
                stats["multiNPCPast"] = subway.MultiNPCPast;
                stats["multiFriendsPast"] = subway.MultiFriendsPast;
                stats["superSinglePast"] = subway.SuperSinglePast;
                stats["superDoublePast"] = subway.SuperDoublePast;
                stats["superMultiNPCPast"] = subway.SuperMultiNPCPast;
                stats["superMultiFriendsPast"] = subway.SuperMultiFriendsPast;
                stats["singleRecord"] = subway.SingleRecord;
                stats["doubleRecord"] = subway.DoubleRecord;
                stats["multiNPCRecord"] = subway.MultiNPCRecord;
                stats["multiFriendsRecord"] = subway.MultiFriendsRecord;
                stats["superSingleRecord"] = subway.SuperSingleRecord;
                stats["superDoubleRecord"] = subway.SuperDoubleRecord;
                stats["superMultiNPCRecord"] = subway.SuperMultiNPCRecord;
                stats["superMultiFriendsRecord"] = subway.SuperMultiFriendsRecord;
            }
            else if (save is SAV6 sav6)
            {
                stats["bp"] = sav6.BP;
            }
            else if (save is SAV7 sav7)
            {
                stats["bp"] = sav7.Misc.BP;
            }
            else
            {
                throw new ValidationException("Battle Facility stats not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return stats;
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetHallOfFame(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            var hofEntries = new List<HallOfFameEntry>();

            if (save is SAV1 sav1)
            {
                var hof = sav1.HallOfFame;
                for (int team = 0; team < HallOfFameReader1.TeamCount; team++)
                {
                    var teamList = new List<PokemonSummary>();
                    var memberCount = hof.GetTeamMemberCount(team);

                    if (memberCount == 0)
                        continue;

                    for (int slot = 0; slot < memberCount; slot++)
                    {
                        var entity = hof.GetEntity(team, slot);
                        if (entity.Species == 0)
                            continue;

                        var summary = new PokemonSummary(
                            Box: -3,
                            Slot: slot,
                            Species: entity.Species,
                            SpeciesName: GameInfo.Strings.Species[entity.Species],
                            Level: entity.Level,
                            IsEgg: false,
                            IsShiny: false
                        );
                        teamList.Add(summary);
                    }

                    if (teamList.Count > 0)
                    {
                        var hofEntry = new HallOfFameEntry(
                            Index: team,
                            Timestamp: string.Empty,
                            Team: teamList.ToArray()
                        );
                        hofEntries.Add(hofEntry);
                    }
                }
            }
            else if (save is SAV3 sav3)
            {
                var entries = HallFame3Entry.GetEntries(sav3);
                for (int i = 0; i < entries.Length; i++)
                {
                    var entry = entries[i];
                    var team = entry.Team;
                    var teamList = new List<PokemonSummary>();

                    for (int j = 0; j < team.Length; j++)
                    {
                        var pk = team[j];
                        if (pk.Species == 0)
                            continue;

                        var summary = new PokemonSummary(
                            Box: -3,
                            Slot: j,
                            Species: pk.Species,
                            SpeciesName: GameInfo.Strings.Species[pk.Species],
                            Level: pk.Level,
                            IsEgg: false,
                            IsShiny: pk.IsShiny
                        );
                        teamList.Add(summary);
                    }

                    if (teamList.Count > 0)
                    {
                        var hofEntry = new HallOfFameEntry(
                            Index: i,
                            Timestamp: string.Empty,
                            Team: teamList.ToArray()
                        );
                        hofEntries.Add(hofEntry);
                    }
                }
            }
            else
            {
                throw new ValidationException("Hall of Fame not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new HallOfFameResponse(true, hofEntries);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetHallOfFameEntry(int handle, int index, string teamJson)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (index < 0)
                throw new ValidationException($"Index {index} must be non-negative", "INVALID_INDEX");

            if (string.IsNullOrWhiteSpace(teamJson))
                throw new ValidationException("Team data cannot be empty", "EMPTY_TEAM_DATA");

            var team = JsonSerializer.Deserialize<PokemonSummary[]>(teamJson, JsonOptions);
            if (team == null || team.Length == 0)
                throw new ValidationException("Invalid team data JSON", INVALID_JSON);

            if (save is SAV3 sav3)
            {
                var entries = HallFame3Entry.GetEntries(sav3);
                if (index >= entries.Length)
                    throw new ValidationException($"Index {index} is out of range (0-{entries.Length - 1})", "INVALID_INDEX");

                var entry = entries[index];
                var entryTeam = entry.Team;
                for (int i = 0; i < Math.Min(team.Length, entryTeam.Length); i++)
                {
                    var pkData = team[i];
                    var pk = entryTeam[i];
                    pk.Species = (ushort)pkData.Species;
                    pk.Level = pkData.Level;
                }

                HallFame3Entry.SetEntries(sav3, entries);
            }
            else
            {
                throw new ValidationException("Hall of Fame modification not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new SuccessMessage(true, "Hall of Fame entry updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetMailbox(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            throw new ValidationException("Mail functionality is not currently supported", "UNSUPPORTED_FEATURE");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetMailMessage(int handle, int index)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (index < 0)
                throw new ValidationException($"Mail index {index} must be non-negative", "INVALID_MAIL_INDEX");

            throw new ValidationException("Mail functionality is not currently supported", "UNSUPPORTED_FEATURE");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetMailMessage(int handle, int index, string mailJson)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (index < 0)
                throw new ValidationException($"Mail index {index} must be non-negative", "INVALID_MAIL_INDEX");

            if (string.IsNullOrWhiteSpace(mailJson))
                throw new ValidationException("Mail data cannot be empty", "EMPTY_MAIL_DATA");

            throw new ValidationException("Mail functionality is not currently supported", "UNSUPPORTED_FEATURE");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string DeleteMail(int handle, int index)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (index < 0)
                throw new ValidationException($"Mail index {index} must be non-negative", "INVALID_MAIL_INDEX");

            throw new ValidationException("Mail functionality is not currently supported", "UNSUPPORTED_FEATURE");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetMysteryGifts(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            var giftList = new List<MysteryGiftCard>();

            if (save is IMysteryGiftStorage giftStorage)
            {
                for (int i = 0; i < giftStorage.GiftCountMax; i++)
                {
                    var gift = giftStorage.GetMysteryGift(i);
                    if (gift.Species == 0 && gift.ItemID == 0)
                        continue;

                    var card = new MysteryGiftCard(
                        Index: i,
                        Type: gift.Type.ToString(),
                        CardTitle: gift.CardTitle,
                        IsItem: gift.IsItem,
                        IsPokemon: gift.IsEntity,
                        IsBP: false,
                        ItemId: gift.ItemID,
                        Species: gift.Species,
                        Level: gift.Level,
                        IsShiny: gift.IsShiny,
                        IsEgg: gift.IsEgg
                    );
                    giftList.Add(card);
                }
            }
            else
            {
                throw new ValidationException("Mystery Gifts not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new MysteryGiftsResponse(true, giftList);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetMysteryGiftCard(int handle, int index)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (index < 0)
                throw new ValidationException($"Gift index {index} must be non-negative", "INVALID_GIFT_INDEX");

            if (save is not IMysteryGiftStorage giftStorage)
                throw new ValidationException("Mystery Gifts not supported for this save file generation", "UNSUPPORTED_GENERATION");

            if (index >= giftStorage.GiftCountMax)
                throw new ValidationException($"Gift index {index} is out of range (0-{giftStorage.GiftCountMax - 1})", "INVALID_GIFT_INDEX");

            var gift = giftStorage.GetMysteryGift(index);
            var card = new MysteryGiftCard(
                Index: index,
                Type: gift.Type.ToString(),
                CardTitle: gift.CardTitle,
                IsItem: gift.IsItem,
                IsPokemon: gift.IsEntity,
                IsBP: false,
                ItemId: gift.ItemID,
                Species: gift.Species,
                Level: gift.Level,
                IsShiny: gift.IsShiny,
                IsEgg: gift.IsEgg
            );

            return new MysteryGiftCardResponse(true, card);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetMysteryGiftCard(int handle, int index, string cardJson)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (index < 0)
                throw new ValidationException($"Gift index {index} must be non-negative", "INVALID_GIFT_INDEX");

            if (string.IsNullOrWhiteSpace(cardJson))
                throw new ValidationException("Card data cannot be empty", "EMPTY_CARD_DATA");

            var cardData = JsonSerializer.Deserialize<MysteryGiftCard>(cardJson, JsonOptions);
            if (cardData == null)
                throw new ValidationException("Invalid card data JSON", INVALID_JSON);

            if (save is not IMysteryGiftStorage giftStorage)
                throw new ValidationException("Mystery Gifts not supported for this save file generation", "UNSUPPORTED_GENERATION");

            if (index >= giftStorage.GiftCountMax)
                throw new ValidationException($"Gift index {index} is out of range (0-{giftStorage.GiftCountMax - 1})", "INVALID_GIFT_INDEX");

            var gift = giftStorage.GetMysteryGift(index);
            gift.CardTitle = cardData.CardTitle;
            giftStorage.SetMysteryGift(index, gift);

            return new SuccessMessage(true, "Mystery Gift card updated successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetMysteryGiftFlags(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            var flags = new List<bool>();

            if (save is IMysteryGiftFlags giftFlags)
            {
                for (int i = 0; i < giftFlags.MysteryGiftReceivedFlagMax; i++)
                {
                    flags.Add(giftFlags.GetMysteryGiftReceivedFlag(i));
                }
            }
            else
            {
                throw new ValidationException("Mystery Gift flags not supported for this save file generation", "UNSUPPORTED_GENERATION");
            }

            return new MysteryGiftFlagsResponse(true, flags.ToArray());
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string DeleteMysteryGift(int handle, int index)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (index < 0)
                throw new ValidationException($"Gift index {index} must be non-negative", "INVALID_GIFT_INDEX");

            if (save is not IMysteryGiftStorage giftStorage)
                throw new ValidationException("Mystery Gifts not supported for this save file generation", "UNSUPPORTED_GENERATION");

            if (index >= giftStorage.GiftCountMax)
                throw new ValidationException($"Gift index {index} is out of range (0-{giftStorage.GiftCountMax - 1})", "INVALID_GIFT_INDEX");

            var gift = giftStorage.GetMysteryGift(index);
            gift.Data.Clear();
            giftStorage.SetMysteryGift(index, gift);

            return new SuccessMessage(true, "Mystery Gift deleted successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetSecretBase(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (save is SAV3 sav3)
            {
                var trainerName = sav3.OT;
                var trainerId = (int)sav3.TID16;
                var secretId = (int)sav3.SID16;
                var gender = sav3.Gender;
                var language = sav3.Language;

                var secretBase = new SecretBaseData(
                    TrainerName: trainerName,
                    TrainerID: trainerId,
                    SecretID: secretId,
                    Gender: gender,
                    Language: language,
                    LocationName: "Secret Base",
                    LocationID: 0
                );

                return new SecretBaseResponse(true, secretBase);
            }
            else if (save is SAV4 sav4)
            {
                var trainerName = sav4.OT;
                var trainerId = (int)sav4.TID16;
                var secretId = (int)sav4.SID16;
                var gender = sav4.Gender;
                var language = sav4.Language;

                var secretBase = new SecretBaseData(
                    TrainerName: trainerName,
                    TrainerID: trainerId,
                    SecretID: secretId,
                    Gender: gender,
                    Language: language,
                    LocationName: "Underground Base",
                    LocationID: 0
                );

                return new SecretBaseResponse(true, secretBase);
            }
            else if (save is SAV6AO sav6ao)
            {
                var secretBaseBlock = sav6ao.SecretBase;
                var selfBase = secretBaseBlock.GetSecretBaseSelf();

                var secretBase = new SecretBaseData(
                    TrainerName: selfBase.TrainerName,
                    TrainerID: (int)sav6ao.TID16,
                    SecretID: (int)sav6ao.SID16,
                    Gender: sav6ao.Gender,
                    Language: sav6ao.Language,
                    LocationName: "Secret Base",
                    LocationID: selfBase.BaseLocation
                );

                return new SecretBaseResponse(true, secretBase);
            }
            else
            {
                throw new ValidationException("Secret Base only supported for Gen 3, Gen 4, and Gen 6 (ORAS) saves", "UNSUPPORTED_GENERATION");
            }
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetEntralinkData(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (save is SAV5 sav5)
            {
                var forestLevel = 0;
                var missionsCompleted = 0;
                var whiteForestCount = 0;
                var blackCityCount = 0;

                if (sav5 is SAV5BW sav5bw)
                {
                    var entree = sav5bw.Entralink;
                    whiteForestCount = entree.WhiteForestLevel;
                    blackCityCount = entree.BlackCityLevel;
                }

                var entralinkData = new EntralinkData(
                    ForestLevel: forestLevel,
                    MissionsCompleted: missionsCompleted,
                    WhiteForestCount: whiteForestCount,
                    BlackCityCount: blackCityCount
                );

                return new EntralinkResponse(true, entralinkData);
            }
            else
            {
                throw new ValidationException("Entralink/Join Avenue only supported for Gen 5 saves", "UNSUPPORTED_GENERATION");
            }
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPokePelago(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (save is SAV7 sav7)
            {
                var pelago = sav7.ResortSave;
                var beansCount = 0;
                for (int i = 0; i < ResortSave7.BEANS_MAX; i++)
                {
                    beansCount += pelago.GetPokebeanCount(i);
                }

                var pokePelagoData = new PokePelagoData(
                    BeansCount: beansCount,
                    IsleAevelynDevelopment: 0,
                    IsleAphunDevelopment: 0,
                    IsleEvelupDevelopment: 0,
                    PokemonCount: 0
                );

                return new PokePelagoResponse(true, pokePelagoData);
            }
            else
            {
                throw new ValidationException("Poké Pelago only supported for Gen 7 saves", "UNSUPPORTED_GENERATION");
            }
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetFestivalPlaza(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (save is SAV7 sav7)
            {
                var plaza = sav7.Festa;
                var rank = plaza.FestaRank;
                var coins = plaza.FestaCoins;

                var festivalPlazaData = new FestivalPlazaData(
                    Rank: rank,
                    FestivalCoins: coins,
                    TotalVisitors: 0,
                    FacilityCount: 0
                );

                return new FestivalPlazaResponse(true, festivalPlazaData);
            }
            else
            {
                throw new ValidationException("Festival Plaza only supported for Gen 7 saves", "UNSUPPORTED_GENERATION");
            }
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPokeJobs(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling<object>(() =>
        {
            if (handle <= 0)
                throw new ValidationException("Handle cannot be zero", "INVALID_HANDLE");

            var save = SaveFileManager.GetSave(handle);
            if (save == null)
                throw new ValidationException($"Invalid save file handle", "INVALID_HANDLE");

            if (save is SAV8SWSH sav8)
            {
                var activeJobsCount = 0;
                var completedJobsCount = 0;

                var pokeJobsData = new PokeJobsData(
                    ActiveJobsCount: activeJobsCount,
                    CompletedJobsCount: completedJobsCount
                );

                return new PokeJobsResponse(true, pokeJobsData);
            }
            else
            {
                throw new ValidationException("Pokémon Jobs only supported for Gen 8 (Sword/Shield) saves", "UNSUPPORTED_GENERATION");
            }
        });
    }
}
