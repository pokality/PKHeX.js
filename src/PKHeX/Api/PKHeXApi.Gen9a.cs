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
