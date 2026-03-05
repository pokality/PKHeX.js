using PKHeX.Core;
using PkhexWorld;
using PkhexWorld.wit.exports.pokality.pkhex;

namespace PkhexWorld.wit.exports.pokality.pkhex;

public class ItemOpsImpl : IItemOps
{
    public static List<IItemOps.PouchData> GetPouchItems(uint handle)
    {
        var save = ImplHelpers.GetSave(handle);
        var bag = save.Inventory;
        var pouches = bag.Pouches;
        var result = new List<IItemOps.PouchData>();

        for (int pouchIndex = 0; pouchIndex < pouches.Count; pouchIndex++)
        {
            var pouch = pouches[pouchIndex];
            var slots = new List<IItemOps.ItemSlot>();

            foreach (var item in pouch.Items)
            {
                if (item.Index == 0 || item.Count == 0)
                    continue;

                var itemId = (ushort)item.Index;
                var name = itemId < GameInfo.Strings.Item.Count
                    ? GameInfo.Strings.Item[itemId]
                    : $"Item {itemId}";

                slots.Add(new IItemOps.ItemSlot(itemId, name, item.Count));
            }

            result.Add(new IItemOps.PouchData(
                pouchType: pouch.Type.ToString(),
                pouchIndex: pouchIndex,
                items: slots,
                maxSlots: pouch.Items.Length
            ));
        }

        return result;
    }

    public static void AddItemToPouch(uint handle, ushort itemId, int count, int pouchIndex)
    {
        var save = ImplHelpers.GetSave(handle);
        var bag = save.Inventory;
        var pouches = bag.Pouches;

        if (itemId == 0 || itemId > save.MaxItemID)
            throw ImplHelpers.Validation($"Item ID {itemId} out of range (1-{save.MaxItemID})");
        if (count <= 0)
            throw ImplHelpers.Validation("Count must be greater than 0");
        if (pouchIndex < 0 || pouchIndex >= pouches.Count)
            throw ImplHelpers.Validation($"Pouch index {pouchIndex} out of range (0-{pouches.Count - 1})");

        var pouch = pouches[pouchIndex];

        var existingIndex = -1;
        var emptyIndex = -1;
        for (int i = 0; i < pouch.Items.Length; i++)
        {
            if (pouch.Items[i].Index == itemId)
            {
                existingIndex = i;
                break;
            }
            if (emptyIndex < 0 && pouch.Items[i].Index == 0)
                emptyIndex = i;
        }

        if (existingIndex >= 0)
        {
            var item = pouch.Items[existingIndex];
            var newCount = item.Count + count;
            if (newCount > pouch.MaxCount)
                newCount = pouch.MaxCount;
            item.Count = newCount;
        }
        else if (emptyIndex >= 0)
        {
            var item = pouch.Items[emptyIndex];
            item.Index = itemId;
            var clamped = count > pouch.MaxCount ? pouch.MaxCount : count;
            item.Count = clamped;
        }
        else
        {
            throw ImplHelpers.Validation("No empty slots available in the specified pouch");
        }

        bag.CopyTo(save);
    }

    public static void RemoveItemFromPouch(uint handle, ushort itemId, int count)
    {
        var save = ImplHelpers.GetSave(handle);
        var bag = save.Inventory;
        var pouches = bag.Pouches;

        foreach (var pouch in pouches)
        {
            foreach (var item in pouch.Items)
            {
                if (item.Index != itemId || item.Count == 0)
                    continue;

                item.Count -= count;
                if (item.Count <= 0)
                {
                    item.Count = 0;
                    item.Index = 0;
                }

                bag.CopyTo(save);
                return;
            }
        }

        throw ImplHelpers.Validation($"Item {itemId} not found in any pouch");
    }

    public static IItemOps.ItemSearchResult HasItem(uint handle, ushort itemId)
    {
        var save = ImplHelpers.GetSave(handle);
        var bag = save.Inventory;
        var pouches = bag.Pouches;

        for (int pouchIndex = 0; pouchIndex < pouches.Count; pouchIndex++)
        {
            var pouch = pouches[pouchIndex];
            foreach (var item in pouch.Items)
            {
                if (item.Index == itemId && item.Count > 0)
                {
                    return new IItemOps.ItemSearchResult(
                        found: true,
                        pouchIndex: pouchIndex,
                        pouchType: pouch.Type.ToString(),
                        count: item.Count
                    );
                }
            }
        }

        return new IItemOps.ItemSearchResult(
            found: false,
            pouchIndex: -1,
            pouchType: "",
            count: 0
        );
    }

    public static int GetFirstEmptySlot(uint handle, int pouchIndex)
    {
        var save = ImplHelpers.GetSave(handle);
        var bag = save.Inventory;
        var pouches = bag.Pouches;

        if (pouchIndex < 0 || pouchIndex >= pouches.Count)
            throw ImplHelpers.Validation($"Pouch index {pouchIndex} out of range (0-{pouches.Count - 1})");

        var pouch = pouches[pouchIndex];
        return pouch.FindIndexFirstEmptySlot();
    }
}
