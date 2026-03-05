using PKHeX.Core;
using PkhexWorld;
using PkhexWorld.wit.exports.pokality.pkhex;

namespace PkhexWorld.wit.exports.pokality.pkhex;

public class ZaOpsImpl : IZaOps
{
    private static SAV9ZA GetZaSave(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        if (save is not SAV9ZA sav9za)
            throw ImplHelpers.Unsupported("This feature is only available in Legends Z-A saves");
        return sav9za;
    }

    private static readonly Dictionary<string, uint> FashionCategoryKeys = new()
    {
        ["tops"] = SaveBlockAccessor9ZA.KFashionTops,
        ["bottoms"] = SaveBlockAccessor9ZA.KFashionBottoms,
        ["allinone"] = SaveBlockAccessor9ZA.KFashionAllInOne,
        ["headwear"] = SaveBlockAccessor9ZA.KFashionHeadwear,
        ["eyewear"] = SaveBlockAccessor9ZA.KFashionEyewear,
        ["gloves"] = SaveBlockAccessor9ZA.KFashionGloves,
        ["legwear"] = SaveBlockAccessor9ZA.KFashionLegwear,
        ["footwear"] = SaveBlockAccessor9ZA.KFashionFootwear,
        ["satchels"] = SaveBlockAccessor9ZA.KFashionSatchels,
        ["earrings"] = SaveBlockAccessor9ZA.KFashionEarrings,
    };

    private static readonly uint[] AllFashionKeys =
    [
        SaveBlockAccessor9ZA.KFashionTops,
        SaveBlockAccessor9ZA.KFashionBottoms,
        SaveBlockAccessor9ZA.KFashionAllInOne,
        SaveBlockAccessor9ZA.KFashionHeadwear,
        SaveBlockAccessor9ZA.KFashionEyewear,
        SaveBlockAccessor9ZA.KFashionGloves,
        SaveBlockAccessor9ZA.KFashionLegwear,
        SaveBlockAccessor9ZA.KFashionFootwear,
        SaveBlockAccessor9ZA.KFashionSatchels,
        SaveBlockAccessor9ZA.KFashionEarrings,
    ];

    private static readonly uint[] AllHairMakeKeys =
    [
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
        SaveBlockAccessor9ZA.KHairMake14DarkCircles,
    ];

    public static void UnlockFashionCategory(uint handle, string category)
    {
        var sav9za = GetZaSave(handle);
        var key = category.ToLowerInvariant();
        if (!FashionCategoryKeys.TryGetValue(key, out uint blockKey))
            throw ImplHelpers.Validation($"Unknown fashion category: {category}. Valid categories: {string.Join(", ", FashionCategoryKeys.Keys)}");

        var block = sav9za.Blocks.GetBlock(blockKey);
        FashionItem9a.ModifyAll(block.Data, item =>
        {
            item.IsOwned = true;
            item.IsNew = false;
        });
    }

    public static int UnlockAllFashion(uint handle)
    {
        var sav9za = GetZaSave(handle);
        int total = 0;

        foreach (var blockKey in AllFashionKeys)
        {
            var block = sav9za.Blocks.GetBlock(blockKey);
            int count = block.Data.Length / FashionItem9a.SIZE;
            FashionItem9a.ModifyAll(block.Data, item =>
            {
                item.IsOwned = true;
                item.IsNew = false;
            });
            total += count;
        }

        return total;
    }

    public static int UnlockAllHairMakeup(uint handle)
    {
        var sav9za = GetZaSave(handle);
        int total = 0;

        foreach (var blockKey in AllHairMakeKeys)
        {
            var block = sav9za.Blocks.GetBlock(blockKey);
            var items = HairMakeItem9a.GetArray(block.Data);
            foreach (var item in items)
            {
                if (item.Value != HairMakeItem9a.None)
                {
                    item.IsNew = false;
                    total++;
                }
            }
            HairMakeItem9a.SetArray(items, block.Data);
        }

        return total;
    }

    public static int CollectColorfulScrews(uint handle)
    {
        var sav9za = GetZaSave(handle);
        return ColorfulScrew9a.CollectScrews(sav9za);
    }

    public static List<IZaOps.ScrewLocation> GetColorfulScrewLocations(uint handle, bool collected)
    {
        var sav9za = GetZaSave(handle);
        var locations = new List<IZaOps.ScrewLocation>();

        foreach (var (fieldItem, point) in ColorfulScrew9a.GetScrewLocations(sav9za, collected))
        {
            int fieldItemHash = fieldItem.GetHashCode();
            locations.Add(new IZaOps.ScrewLocation(fieldItemHash, point.X, point.Y, point.Z));
        }

        return locations;
    }

    public static int CollectTechnicalMachines(uint handle)
    {
        var sav9za = GetZaSave(handle);
        return TechnicalMachine9a.SetAllTechnicalMachines(sav9za, true);
    }

    public static int GetTextSpeed(uint handle)
    {
        var sav9za = GetZaSave(handle);
        return (int)sav9za.Config.TextSpeed;
    }

    public static void SetTextSpeed(uint handle, int speed)
    {
        var sav9za = GetZaSave(handle);
        if (speed < 0 || speed > 3)
            throw ImplHelpers.Validation("Text speed must be between 0 (Slow) and 3 (Instant)");
        sav9za.Config.TextSpeed = (TextSpeedOption)speed;
    }

    public static List<IZaOps.DonutEntry> GetDonuts(uint handle)
    {
        var sav9za = GetZaSave(handle);
        var donuts = sav9za.Donuts;
        var entries = new List<IZaOps.DonutEntry>();

        for (int i = 0; i < DonutPocket9a.MaxCount; i++)
        {
            var donut = donuts.GetDonut(i);
            if (donut.IsEmpty)
                continue;

            string berryName = donut.BerryName < GameInfo.Strings.Item.Count
                ? GameInfo.Strings.Item[donut.BerryName]
                : $"Item_{donut.BerryName}";

            entries.Add(new IZaOps.DonutEntry(
                slot: i,
                donut: donut.Donut,
                calories: donut.Calories,
                stars: donut.Stars,
                levelBoost: donut.LevelBoost,
                berryName: berryName
            ));
        }

        return entries;
    }

    public static void SetAllDonutsShiny(uint handle)
    {
        var sav9za = GetZaSave(handle);
        sav9za.Donuts.SetAllAsShinyTemplate();
    }

    public static void CompressDonuts(uint handle)
    {
        var sav9za = GetZaSave(handle);
        sav9za.Donuts.Compress();
    }

    public static uint GetHyperspaceSurveyPoints(uint handle)
    {
        var sav9za = GetZaSave(handle);
        if (sav9za.SaveRevision < 1)
            throw ImplHelpers.Unsupported("Hyperspace Survey Points require Mega Dimension save revision");
        return sav9za.GetValue<uint>(SaveBlockAccessor9ZA.KHyperspaceSurveyPoints);
    }

    public static void SetHyperspaceSurveyPoints(uint handle, uint points)
    {
        var sav9za = GetZaSave(handle);
        if (sav9za.SaveRevision < 1)
            throw ImplHelpers.Unsupported("Hyperspace Survey Points require Mega Dimension save revision");
        sav9za.SetValue(SaveBlockAccessor9ZA.KHyperspaceSurveyPoints, points);
    }

    public static string GetStreetName(uint handle)
    {
        var sav9za = GetZaSave(handle);
        if (sav9za.SaveRevision < 1)
            throw ImplHelpers.Unsupported("Street Name requires Mega Dimension save revision");
        var block = sav9za.Blocks.GetBlock(SaveBlockAccessor9ZA.KStreetName);
        return sav9za.GetString(block.Data);
    }

    public static void SetStreetName(uint handle, string name)
    {
        var sav9za = GetZaSave(handle);
        if (sav9za.SaveRevision < 1)
            throw ImplHelpers.Unsupported("Street Name requires Mega Dimension save revision");
        if (name.Length > 18)
            throw ImplHelpers.Validation("Street name must be 18 characters or fewer");
        var block = sav9za.Blocks.GetBlock(SaveBlockAccessor9ZA.KStreetName);
        sav9za.SetString(block.Data, name.AsSpan(), 18, StringConverterOption.ClearZero);
    }

    public static IZaOps.RoyalePoints GetInfiniteRoyalePoints(uint handle)
    {
        var sav9za = GetZaSave(handle);
        return new IZaOps.RoyalePoints(sav9za.TicketPointsRoyale, sav9za.TicketPointsRoyaleInfinite);
    }

    public static void SetInfiniteRoyalePoints(uint handle, uint royale, uint infinite)
    {
        var sav9za = GetZaSave(handle);
        sav9za.TicketPointsRoyale = royale;
        sav9za.TicketPointsRoyaleInfinite = infinite;
    }
}
