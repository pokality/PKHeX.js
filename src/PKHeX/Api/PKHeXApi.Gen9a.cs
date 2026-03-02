using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;

namespace PKHeX.Api;

// Gen9a (Legends Z-A) Specific Operations
public partial class PKHeXApi
{
    /// <summary>
    /// Collects all Colorful Screws in Legends Z-A and updates the inventory count.
    /// </summary>
    /// <param name="handle">Save file handle</param>
    /// <returns>JSON response with the number of screws collected</returns>
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string CollectColorfulScrews(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (save is not SAV9ZA sav9za)
                throw new ValidationException("Colorful Screws are only available in Legends Z-A saves", "UNSUPPORTED_GENERATION");

            var count = ColorfulScrew9a.CollectScrews(sav9za);

            return new
            {
                success = true,
                screwsCollected = count,
                message = $"Collected {count} Colorful Screws"
            };
        });
    }

    /// <summary>
    /// Gets the locations of Colorful Screws by their collection state.
    /// </summary>
    /// <param name="handle">Save file handle</param>
    /// <param name="collected">True to get collected screws, false to get uncollected screws</param>
    /// <returns>JSON response with screw locations</returns>
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetColorfulScrewLocations(int handle, bool collected)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (save is not SAV9ZA sav9za)
                throw new ValidationException("Colorful Screws are only available in Legends Z-A saves", "UNSUPPORTED_GENERATION");

            var locations = ColorfulScrew9a.GetScrewLocations(sav9za, collected);
            var screwList = new List<object>();

            foreach (var (fieldItem, point) in locations)
            {
                screwList.Add(new
                {
                    fieldItem,
                    x = point.X,
                    y = point.Y,
                    z = point.Z
                });
            }

            return new
            {
                success = true,
                collected,
                count = screwList.Count,
                locations = screwList
            };
        });
    }

    /// <summary>
    /// Sets the text speed in ConfigSave for Legends Z-A.
    /// </summary>
    /// <param name="handle">Save file handle</param>
    /// <param name="speed">Text speed value (0=Slow, 1=Normal, 2=Fast, 3=Instant)</param>
    /// <returns>JSON response indicating success</returns>
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetTextSpeed(int handle, int speed)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (save is not SAV9ZA sav9za)
                throw new ValidationException("Text speed setting is only available in Legends Z-A saves", "UNSUPPORTED_GENERATION");

            if (speed < 0 || speed > 3)
                throw new ValidationException($"Text speed must be between 0 and 3, got {speed}", "INVALID_TEXT_SPEED");

            var config = sav9za.Config;
            config.TextSpeed = (TextSpeedOption)speed;

            return new SuccessMessage(true, $"Text speed set to {speed}");
        });
    }

    /// <summary>
    /// Gets the current text speed setting from ConfigSave.
    /// </summary>
    /// <param name="handle">Save file handle</param>
    /// <returns>JSON response with text speed value</returns>
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetTextSpeed(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (save is not SAV9ZA sav9za)
                throw new ValidationException("Text speed setting is only available in Legends Z-A saves", "UNSUPPORTED_GENERATION");

            var config = sav9za.Config;
            var speed = (int)config.TextSpeed;

            return new
            {
                success = true,
                textSpeed = speed,
                speedName = speed switch
                {
                    0 => "Slow",
                    1 => "Normal",
                    2 => "Fast",
                    3 => "Instant",
                    _ => "Unknown"
                }
            };
        });
    }

    /// <summary>
    /// Unlocks all fashion items in a specific category for Legends Z-A.
    /// </summary>
    /// <param name="handle">Save file handle</param>
    /// <param name="category">Fashion category (tops, bottoms, allinone, headwear, eyewear, gloves, legwear, footwear, satchels, earrings)</param>
    /// <returns>JSON response indicating success</returns>
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string UnlockFashionCategory(int handle, string category)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (save is not SAV9ZA sav9za)
                throw new ValidationException("Fashion items are only available in Legends Z-A saves", "UNSUPPORTED_GENERATION");

            if (string.IsNullOrWhiteSpace(category))
                throw new ValidationException("Category cannot be empty", "EMPTY_CATEGORY");

            var blockKey = category.ToLowerInvariant() switch
            {
                "tops" => SaveBlockAccessor9ZA.KFashionTops,
                "bottoms" => SaveBlockAccessor9ZA.KFashionBottoms,
                "allinone" => SaveBlockAccessor9ZA.KFashionAllInOne,
                "headwear" => SaveBlockAccessor9ZA.KFashionHeadwear,
                "eyewear" => SaveBlockAccessor9ZA.KFashionEyewear,
                "gloves" => SaveBlockAccessor9ZA.KFashionGloves,
                "legwear" => SaveBlockAccessor9ZA.KFashionLegwear,
                "footwear" => SaveBlockAccessor9ZA.KFashionFootwear,
                "satchels" => SaveBlockAccessor9ZA.KFashionSatchels,
                "earrings" => SaveBlockAccessor9ZA.KFashionEarrings,
                _ => throw new ValidationException($"Unknown fashion category: {category}", "INVALID_CATEGORY")
            };

            var block = sav9za.Blocks.GetBlock(blockKey);
            if (block == null)
                throw new ValidationException($"Fashion block not found for category: {category}", "BLOCK_NOT_FOUND");

            // Set all items in the category as owned
            var data = block.Data;
            FashionItem9a.ModifyAll(data, item =>
            {
                item.IsOwned = true;
                item.IsNew = false;
            });

            return new SuccessMessage(true, $"Unlocked all items in {category} category");
        });
    }

    /// <summary>
    /// Unlocks all fashion items in all categories for Legends Z-A.
    /// </summary>
    /// <param name="handle">Save file handle</param>
    /// <returns>JSON response indicating success</returns>
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string UnlockAllFashion(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (save is not SAV9ZA sav9za)
                throw new ValidationException("Fashion items are only available in Legends Z-A saves", "UNSUPPORTED_GENERATION");

            var categories = new[]
            {
                ("tops", SaveBlockAccessor9ZA.KFashionTops),
                ("bottoms", SaveBlockAccessor9ZA.KFashionBottoms),
                ("allinone", SaveBlockAccessor9ZA.KFashionAllInOne),
                ("headwear", SaveBlockAccessor9ZA.KFashionHeadwear),
                ("eyewear", SaveBlockAccessor9ZA.KFashionEyewear),
                ("gloves", SaveBlockAccessor9ZA.KFashionGloves),
                ("legwear", SaveBlockAccessor9ZA.KFashionLegwear),
                ("footwear", SaveBlockAccessor9ZA.KFashionFootwear),
                ("satchels", SaveBlockAccessor9ZA.KFashionSatchels),
                ("earrings", SaveBlockAccessor9ZA.KFashionEarrings)
            };

            int totalUnlocked = 0;

            foreach (var (name, blockKey) in categories)
            {
                var block = sav9za.Blocks.GetBlock(blockKey);
                if (block == null)
                    continue;

                var data = block.Data;
                FashionItem9a.ModifyAll(data, item =>
                {
                    if (!item.IsOwned)
                    {
                        item.IsOwned = true;
                        item.IsNew = false;
                        totalUnlocked++;
                    }
                });
            }

            return new
            {
                success = true,
                itemsUnlocked = totalUnlocked,
                message = $"Unlocked {totalUnlocked} fashion items across all categories"
            };
        });
    }

    /// <summary>
    /// Unlocks all hair and makeup options for Legends Z-A.
    /// </summary>
    /// <param name="handle">Save file handle</param>
    /// <returns>JSON response indicating success</returns>
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string UnlockAllHairMakeup(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (save is not SAV9ZA sav9za)
                throw new ValidationException("Hair/Makeup options are only available in Legends Z-A saves", "UNSUPPORTED_GENERATION");

            var hairMakeKeys = new[]
            {
                SaveBlockAccessor9ZA.KHairMake00StyleHair,
                SaveBlockAccessor9ZA.KHairMake01StyleBangs,
                SaveBlockAccessor9ZA.KHairMake02ColorHair,
                SaveBlockAccessor9ZA.KHairMake03ColorHair,
                SaveBlockAccessor9ZA.KHairMake04ColorHair,
                SaveBlockAccessor9ZA.KHairMake05StyleEyebrow,
                SaveBlockAccessor9ZA.KHairMake06ColorEyebrow,
                SaveBlockAccessor9ZA.KHairMake07StyleEyes,
                SaveBlockAccessor9ZA.KHairMake08ColorEyes,
                SaveBlockAccessor9ZA.KHairMake09StyleEyelash,
                SaveBlockAccessor9ZA.KHairMake10ColorEyelash,
                SaveBlockAccessor9ZA.KHairMake11Lips,
                SaveBlockAccessor9ZA.KHairMake12BeautyMark,
                SaveBlockAccessor9ZA.KHairMake13Freckles,
                SaveBlockAccessor9ZA.KHairMake14DarkCircles
            };

            int totalUnlocked = 0;

            foreach (var blockKey in hairMakeKeys)
            {
                var block = sav9za.Blocks.GetBlock(blockKey);
                if (block == null)
                    continue;

                var data = block.Data;
                for (int i = 0; i < data.Length; i += HairMakeItem9a.SIZE)
                {
                    if (i + HairMakeItem9a.SIZE > data.Length)
                        break;

                    var item = HairMakeItem9a.Read(data.Slice(i, HairMakeItem9a.SIZE));
                    var flags = item.Flags;

                    // Set IsOwned flag (bit 4)
                    item.Flags = flags | 0x10u;
                    // Clear IsNew flag (bit 0)
                    item.Flags = item.Flags & ~0x1u;

                    item.Write(data.Slice(i, HairMakeItem9a.SIZE));
                    totalUnlocked++;
                }
            }

            return new
            {
                success = true,
                itemsUnlocked = totalUnlocked,
                message = $"Unlocked {totalUnlocked} hair and makeup options"
            };
        });
    }

    /// <summary>
    /// Collects all Technical Machines in a Legends Z-A save.
    /// </summary>
    /// <param name="handle">Save file handle</param>
    /// <returns>JSON response with the number of TMs collected</returns>
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string CollectTechnicalMachines(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (save is not SAV9ZA sav9za)
                throw new ValidationException("Technical Machines collection is only available in Legends Z-A saves", "UNSUPPORTED_GENERATION");

            var count = TechnicalMachine9a.SetAllTechnicalMachines(sav9za);

            return new TmsCollectedResponse(true, count, $"Collected {count} Technical Machines");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetHyperspaceSurveyPoints(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (save is not SAV9ZA sav9za)
                throw new ValidationException("Hyperspace Survey Points are only available in Legends Z-A saves", "UNSUPPORTED_GENERATION");

            if (sav9za.SaveRevision == 0)
                throw new ValidationException("Hyperspace Survey Points require the Mega Dimension DLC (save revision 1+)", "UNSUPPORTED_REVISION");

            var points = sav9za.GetValue<uint>(SaveBlockAccessor9ZA.KHyperspaceSurveyPoints);

            return new SurveyPointsResponse(true, points);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetHyperspaceSurveyPoints(int handle, int points)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (save is not SAV9ZA sav9za)
                throw new ValidationException("Hyperspace Survey Points are only available in Legends Z-A saves", "UNSUPPORTED_GENERATION");

            if (sav9za.SaveRevision == 0)
                throw new ValidationException("Hyperspace Survey Points require the Mega Dimension DLC (save revision 1+)", "UNSUPPORTED_REVISION");

            if (points < 0)
                throw new ValidationException("Points cannot be negative", "INVALID_ARGUMENT");

            sav9za.SetValue(SaveBlockAccessor9ZA.KHyperspaceSurveyPoints, (uint)points);

            return new SuccessMessage(true, $"Hyperspace Survey Points set to {points}");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetStreetName(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (save is not SAV9ZA sav9za)
                throw new ValidationException("Street Name is only available in Legends Z-A saves", "UNSUPPORTED_GENERATION");

            if (sav9za.SaveRevision == 0)
                throw new ValidationException("Street Name requires the Mega Dimension DLC (save revision 1+)", "UNSUPPORTED_REVISION");

            var block = sav9za.Blocks.GetBlock(SaveBlockAccessor9ZA.KStreetName);
            var streetName = sav9za.GetString(block.Data);

            return new StreetNameResponse(true, streetName, 18);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetStreetName(int handle, string name)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (save is not SAV9ZA sav9za)
                throw new ValidationException("Street Name is only available in Legends Z-A saves", "UNSUPPORTED_GENERATION");

            if (sav9za.SaveRevision == 0)
                throw new ValidationException("Street Name requires the Mega Dimension DLC (save revision 1+)", "UNSUPPORTED_REVISION");

            if (string.IsNullOrEmpty(name))
                throw new ValidationException("Street name cannot be empty", "EMPTY_NAME");

            if (name.Length > 18)
                throw new ValidationException("Street name cannot exceed 18 characters", "NAME_TOO_LONG");

            var block = sav9za.Blocks.GetBlock(SaveBlockAccessor9ZA.KStreetName);
            sav9za.SetString(block.Data, name.AsSpan(), 18, StringConverterOption.ClearZero);

            return new SuccessMessage(true, $"Street name set to '{name}'");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetDonuts(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (save is not SAV9ZA sav9za)
                throw new ValidationException("Donuts are only available in Legends Z-A saves", "UNSUPPORTED_GENERATION");

            var donuts = sav9za.Donuts;
            var donutList = new List<DonutEntry>();

            for (int i = 0; i < DonutPocket9a.MaxCount; i++)
            {
                var donut = donuts.GetDonut(i);
                if (donut.Donut == 0 && donut.Calories == 0)
                    continue;

                donutList.Add(new DonutEntry(i, donut.Donut, donut.Calories, donut.Stars, donut.LevelBoost, donut.BerryName));
            }

            return new DonutPocketResponse(true, donutList, donutList.Count, DonutPocket9a.MaxCount);
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetAllDonutsShiny(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (save is not SAV9ZA sav9za)
                throw new ValidationException("Donuts are only available in Legends Z-A saves", "UNSUPPORTED_GENERATION");

            sav9za.Donuts.SetAllAsShinyTemplate();

            return new DonutsShinyResponse(true, DonutPocket9a.MaxCount, $"Set all {DonutPocket9a.MaxCount} donuts to shiny template");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string CompressDonuts(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (save is not SAV9ZA sav9za)
                throw new ValidationException("Donuts are only available in Legends Z-A saves", "UNSUPPORTED_GENERATION");

            sav9za.Donuts.Compress();

            return new SuccessMessage(true, "Donut pocket compressed (empty slots removed)");
        });
    }

    /// <summary>
    /// Gets Infinite Royale ticket points for Legends Z-A.
    /// </summary>
    /// <param name="handle">Save file handle</param>
    /// <returns>JSON response with ticket points</returns>
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetInfiniteRoyalePoints(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (save is not SAV9ZA sav9za)
                throw new ValidationException("Infinite Royale is only available in Legends Z-A saves", "UNSUPPORTED_GENERATION");

            return new
            {
                success = true,
                royalePoints = sav9za.TicketPointsRoyale,
                infiniteRoyalePoints = sav9za.TicketPointsRoyaleInfinite
            };
        });
    }

    /// <summary>
    /// Sets Infinite Royale ticket points for Legends Z-A.
    /// </summary>
    /// <param name="handle">Save file handle</param>
    /// <param name="royalePoints">Regular Royale ticket points</param>
    /// <param name="infinitePoints">Infinite Royale ticket points</param>
    /// <returns>JSON response indicating success</returns>
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string SetInfiniteRoyalePoints(int handle, int royalePoints, int infinitePoints)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (save is not SAV9ZA sav9za)
                throw new ValidationException("Infinite Royale is only available in Legends Z-A saves", "UNSUPPORTED_GENERATION");

            sav9za.TicketPointsRoyale = (uint)royalePoints;
            sav9za.TicketPointsRoyaleInfinite = (uint)infinitePoints;

            return new SuccessMessage(true, $"Set Royale points to {royalePoints} and Infinite Royale points to {infinitePoints}");
        });
    }
}
