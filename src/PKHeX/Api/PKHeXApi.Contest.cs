using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;
using static PKHeX.Models.ErrorCodes;

namespace PKHeX.Api;

// Contest Stats
public partial class PKHeXApi
{
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetContestStats(int handle, int box, int slot)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (pk is not IContestStatsReadOnly contestStats)
                throw new ValidationException("This Pokemon does not support contest stats", "NO_CONTEST_STATS");

            return new
            {
                cool = contestStats.ContestCool,
                beauty = contestStats.ContestBeauty,
                cute = contestStats.ContestCute,
                smart = contestStats.ContestSmart,
                tough = contestStats.ContestTough,
                sheen = contestStats.ContestSheen
            };
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetContestStat(int handle, int box, int slot, string statName, byte value)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);
            var pk = ApiHelpers.GetValidatedPokemon(save, box, slot);

            if (string.IsNullOrWhiteSpace(statName))
                throw new ValidationException("Stat name cannot be empty", "EMPTY_STAT_NAME");

            if (pk is not IContestStats contestStats)
                throw new ValidationException("This Pokemon does not support contest stats", "NO_CONTEST_STATS");

            switch (statName.ToLowerInvariant())
            {
                case "cool":
                    contestStats.ContestCool = value;
                    break;
                case "beauty":
                    contestStats.ContestBeauty = value;
                    break;
                case "cute":
                    contestStats.ContestCute = value;
                    break;
                case "smart":
                    contestStats.ContestSmart = value;
                    break;
                case "tough":
                    contestStats.ContestTough = value;
                    break;
                case "sheen":
                    contestStats.ContestSheen = value;
                    break;
                default:
                    throw new ValidationException($"Invalid contest stat name: {statName}", "INVALID_STAT_NAME");
            }

            pk.RefreshChecksum();
            save.SetBoxSlotAtIndex(pk, box, slot);

            return new SuccessMessage(true, "Contest stat set successfully");
        });
    }
}
