using System.Runtime.InteropServices.JavaScript;
using PKHeX.Core;
using PKHeX.Helpers;
using PKHeX.Models;

namespace PKHeX.Api;

// Item Operations
public partial class PKHeXApi
{
    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string GetPouchItems(int handle)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            var pouchList = new List<PouchData>();
            var inventory = save.Inventory;

            for (int pouchIndex = 0; pouchIndex < inventory.Count; pouchIndex++)
            {
                var pouch = inventory[pouchIndex];
                var itemSlots = new List<ItemSlot>();

                foreach (var item in pouch.Items)
                {
                    if (item.Index == 0 || item.Count == 0)
                        continue;

                    var itemName = GameInfo.Strings.Item[item.Index];
                    itemSlots.Add(new ItemSlot(item.Index, itemName, item.Count));
                }

                pouchList.Add(new PouchData(
                    pouch.Type.ToString(),
                    pouchIndex,
                    itemSlots,
                    pouch.Items.Length
                ));
            }

            return pouchList;
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string AddItemToPouch(int handle, int itemId, int count, int pouchIndex)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (itemId < 0 || itemId > save.MaxItemID)
                throw new ValidationException($"Item ID {itemId} is out of range (0-{save.MaxItemID})", "INVALID_ITEM_ID");

            if (count <= 0)
                throw new ValidationException($"Count {count} must be greater than 0", "INVALID_COUNT");

            var inventory = save.Inventory;
            if (pouchIndex < 0 || pouchIndex >= inventory.Count)
                throw new ValidationException($"Pouch index {pouchIndex} is out of range (0-{inventory.Count - 1})", "INVALID_POUCH_INDEX");

            var pouch = inventory[pouchIndex];

            if (!pouch.Info.IsLegal(pouch.Type, itemId, count))
                throw new ValidationException($"Item {itemId} is not legal for pouch type {pouch.Type}", "ILLEGAL_ITEM");

            var existingItem = pouch.Items.FirstOrDefault(i => i.Index == itemId);
            if (existingItem != null)
            {
                var newCount = Math.Min(existingItem.Count + count, pouch.MaxCount);
                existingItem.Count = newCount;
            }
            else
            {
                var emptySlot = pouch.Items.FirstOrDefault(i => i.Index == 0);
                if (emptySlot == null)
                    throw new ValidationException("No empty slots available in pouch", "POUCH_FULL");

                emptySlot.Index = itemId;
                emptySlot.Count = Math.Min(count, pouch.MaxCount);
            }

            return new SuccessMessage(true, "Item added to pouch successfully");
        });
    }

    [JSExport]
    [return: JSMarshalAs<JSType.String>]
    public static string RemoveItemFromPouch(int handle, int itemId, int count)
    {
        return ApiHelpers.ExecuteWithErrorHandling(() =>
        {
            var save = ApiHelpers.GetValidatedSave(handle);

            if (itemId < 0 || itemId > save.MaxItemID)
                throw new ValidationException($"Item ID {itemId} is out of range (0-{save.MaxItemID})", "INVALID_ITEM_ID");

            if (count <= 0)
                throw new ValidationException($"Count {count} must be greater than 0", "INVALID_COUNT");

            var inventory = save.Inventory;
            bool itemFound = false;

            foreach (var pouch in inventory)
            {
                var item = pouch.Items.FirstOrDefault(i => i.Index == itemId);
                if (item != null && item.Count > 0)
                {
                    itemFound = true;
                    item.Count = Math.Max(0, item.Count - count);
                    if (item.Count == 0)
                        item.Index = 0;
                    break;
                }
            }

            if (!itemFound)
                throw new ValidationException($"Item {itemId} not found in inventory", "ITEM_NOT_FOUND");

            return new SuccessMessage(true, "Item removed from pouch successfully");
        });
    }
}
