using System;
using System.Collections.Generic;

public class ContainerOperationsService
{
    private readonly ItemModelFactory _itemModelFactory;

    public ContainerOperationsService(
        ItemModelFactory itemModelFactory)
    {
        _itemModelFactory = itemModelFactory;
    }

    public List<byte> TryAddToContainer(
        ContainerData container,
        ItemData source)
    {
        var maxStack = GetMaxStack(container.Config, source.Config.MaxStack);
        var changedSlots = new List<byte>();

        var left = TryStackExisting(container, source, maxStack, changedSlots);

        if (left > 0)
            PlaceInEmptySlots(container, source, maxStack, changedSlots);

        return changedSlots;
    }

    private int TryStackExisting(ContainerData container, ItemData source, int maxStack, List<byte> changes)
    {
        for (byte i = 0; i < container.Slots && source.Amount > 0; i++)
        {
            var itemData = container.itemDatas[i];
            if (itemData.TypeItem == source.TypeItem && itemData.Amount < maxStack)
            {
                int canAdd = Math.Min(source.Amount, maxStack - itemData.Amount);
                itemData.Amount += canAdd;
                source.Amount -= canAdd;
                changes.Add(i);
            }
        }
        return source.Amount;
    }

    private int PlaceInEmptySlots(ContainerData container, ItemData source, int maxStack, List<byte> changes)
    {
        for (byte i = 0; i < container.Slots && source.Amount > 0; i++)
        {
            var slot = container.itemDatas[i];
            if (slot.TypeItem != EItemType.None) continue;

            changes.Add(i);
            if (source.Amount <= maxStack)
            {
                var copySource = _itemModelFactory.GetDuplicate(source);
                container.SwapItem(i, copySource);
                source.Amount = 0;
                return 0;
            }
            else
            {
                var split = _itemModelFactory.SplitItem(source, maxStack);
                container.SwapItem(i, split);
            }
        }
        return source.Amount;
    }

    private int GetMaxStack(ContainerConfig containerConfig, int maxStack)
    {
        return containerConfig.CustomStackSize != 0 ?
            Math.Min(maxStack, containerConfig.CustomStackSize) :
            maxStack;
    }
}