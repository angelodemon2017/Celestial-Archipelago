using System;
using System.Collections.Generic;

public class ContainerOperationsService
{
    private readonly ItemModelFactory _itemModelFactory;

    private List<int> _changedSlots = new List<int>();

    public ContainerOperationsService(
        ItemModelFactory itemModelFactory)
    {
        _itemModelFactory = itemModelFactory;
    }

    public List<int> TryAddItemToContainer(
        ContainerData container,
        ItemData source)
    {
        var maxStack = GetMaxStack(container.Config, source.Config.MaxStack);
        _changedSlots.Clear();

        var left = TryStackExisting(container, source, maxStack, _changedSlots);

        if (left > 0)
            PlaceInEmptySlots(container, source, maxStack, _changedSlots, left);

        return _changedSlots;
    }

    public List<int> TryRemoveFromContainer(
        ContainerData container,
        ItemConfig itemConf,
        int amount)
    {
        _changedSlots.Clear();
        if (container.GetItemCountByType(itemConf.TypeItem) < amount)
            return _changedSlots;

        if (container.GetItemsByType(itemConf.TypeItem, out List<ItemData> itemDatas))
        {
            var count = itemDatas.Count;
            for (int i = 0; i < count && amount > 0; i++)
            {
                var itemData = itemDatas[i];
                int amountForSlot = Math.Min(amount, itemData.Amount);
                itemData.Amount -= amountForSlot;
                amount -= amountForSlot;
                _changedSlots.Add(itemData.SlotId);
            }
        }

        return _changedSlots;
    }

    private int TryStackExisting(ContainerData container, ItemData source, int maxStack,
        List<int> changes)
    {
        if (container.GetItemsByType(source.TypeItem, out List<ItemData> itemDatas))
        {
            var count = itemDatas.Count;
            for (int i = 0; i < count && source.Amount > 0; i++)
            {
                var itemData = itemDatas[i];
                if (itemData.Amount < maxStack)
                {
                    int canAdd = Math.Min(source.Amount, maxStack - itemData.Amount);
                    itemData.Amount += canAdd;
                    source.Amount -= canAdd;
                    changes.Add(itemData.SlotId);
                }
            }
        }
        return source.Amount;
    }

    private int PlaceInEmptySlots(ContainerData container, ItemData source, int maxStack,
        List<int> changes, int workAmount)
    {
        var count = container.itemDatas.Count;
        for (int i = 0; i < count && source.Amount > 0 && container.EmptySlots > 0; i++)
        {
            var itemData = container.itemDatas[i];
            if (itemData.TypeItem != EItemType.None)
                continue;
            changes.Add(itemData.SlotId);
            if (source.Amount <= maxStack)
            {
                var copySource = _itemModelFactory.GetDuplicate(source);
                container.ReplaceItem(itemData.SlotId, copySource);
                source.Amount = 0;
                return 0;
            }
            else
            {
                var split = _itemModelFactory.SplitItem(source, maxStack);
                container.ReplaceItem(itemData.SlotId, split);
            }
        }
        return source.Amount;
    }

    private int GetMaxStack(ContainerConfig containerConfig, int maxStack)
    {
        return containerConfig.CustomStackSize != 0 ?
            containerConfig.CustomStackSize : maxStack;
    }

    public bool TryMoveAmountFromSlotToSlot(
        ContainerData fromContainer,
        int fromSlotId,
        ContainerData toContainer,
        int toSlotId,
        int amountToMove)
    {
        _changedSlots.Clear();

        if (amountToMove <= 0) return false;

        var sourceItem = fromContainer.itemDatas[fromSlotId];
        // Получаем предмет из исходного слота
        if (sourceItem.TypeItem == EItemType.None || sourceItem.Amount <= 0)
            return false;

        int actualMove = Math.Min(amountToMove, sourceItem.Amount);
        if (actualMove <= 0) return false;

        var maxStack = GetMaxStack(toContainer.Config, sourceItem.Config.MaxStack);

        bool added = TryAddToSpecificSlot(toContainer, toSlotId, sourceItem, actualMove, maxStack, out List<int> toChanges);
        _changedSlots.AddRange(toChanges);

        // Если в источнике осталось 0 — слот будет очищен выше (в InventoryTransactionsService)
        return true;
    }

    // Вспомогательный метод
    private bool TryAddToSpecificSlot(
        ContainerData container,
        int slotId,
        ItemData sourceItem,
        int amount,
        int maxStack,
        out List<int> changedSlots)
    {
        changedSlots = new List<int>();

        var targetItem = container.itemDatas[slotId];

        // Если слот пустой — помещаем новый предмет
        if (targetItem.TypeItem == EItemType.None)
        {
            var toPlace = _itemModelFactory.GetDuplicate(sourceItem);
            toPlace.Amount = Math.Min(amount, maxStack);
            sourceItem.Amount -= toPlace.Amount;
            container.ReplaceItem(slotId, toPlace);
            changedSlots.Add(slotId);
            return true;
        }

        // Если тот же тип — пытаемся стакировать
        if (targetItem.TypeItem == sourceItem.TypeItem &&
                targetItem.Amount < maxStack)
        {
            int canAdd = Math.Min(amount, maxStack - targetItem.Amount);
            if (canAdd > 0)
            {
                targetItem.Amount += canAdd;
                sourceItem.Amount -= canAdd;
                changedSlots.Add(slotId);
                return true;
            }
        }

        // Иначе — не удалось добавить (конфликт типов). Можно расширить позже (swap и т.д.)
        return false;
    }
}