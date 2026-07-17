using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[Serializable]
public class ContainerModel : BaseModel<ContainerData, ContainerConfig>, IPoolable<ContainerData>
{
    private readonly ItemModelFactory _itemModelFactory;

    public int IdEntityOwner => _dataModel.IdEntityOwner;
    public override string ModelName => string.Empty;
    public string TitleContainer => ConfigModel.KeyName;
    public List<ItemModel> itemModels = new();
    private Dictionary<int, ItemModel> _itemsBySlots = new();

    public bool IsEmpty => _dataModel.EmptySlots == _dataModel.Slots;
    public ContainerAvailabilityFlag Availabilities => _dataModel.CurrentAvailabilitys;
    public Action<int> ChangedSlotId;

    public ContainerModel(
        ItemModelFactory itemModelFactory)
    {
        _itemModelFactory = itemModelFactory;
    }

    public void OnSpawned(ContainerData containerData)
    {
        _dataModel = containerData;
        CleanList();
        for (int i = 0; i < _dataModel.Slots; i++)
        {
            var newIM = _itemModelFactory.Spawn(_dataModel.itemDatas[i]);
            itemModels.Add(newIM);
            _itemsBySlots.Add(newIM.SlotId, newIM);
        }
    }

    public bool RefreshModelsAfterDataChange(List<int> changedSlotIndices)
    {
        if (changedSlotIndices.Count > 0)
        {
            int count = changedSlotIndices.Count;
            for (int i = 0; i < count; i++)
                RefreshModelBySlot(changedSlotIndices[i]);
            return true;
        }
        else
            return false;
    }

    public void ClearSlot(int slotId)
    {
        var emptData = _itemModelFactory.GetEmptyItemData();
        _dataModel.ReplaceItem(slotId, emptData);
        RefreshModelBySlot(slotId);
    }

    public void RefreshModelBySlot(int slot)
    {
        var dataInSlot = _dataModel.itemDatas[slot];
        var modelItem = _itemModelFactory.Spawn(dataInSlot);
        ReplaceItemModel(slot, modelItem);
    }

    public void ReplaceItemModel(int slotId, ItemModel newModel)
    {//TODO return previous model?
        var oldModel = itemModels[slotId];
        if (oldModel == newModel)
            return;

        _itemModelFactory.Despawn(oldModel);
        newModel._dataModel.SlotId = slotId;
        itemModels[slotId] = newModel;
        _itemsBySlots[slotId] = newModel;
        ChangedSlotId?.Invoke(slotId);
    }

    public ItemModel ExchangeItemModel(int slotId, ItemModel newModel)
    {//Problem method - itemModel return to fabric or not?
        var oldModel = itemModels[slotId];
        _dataModel.ReplaceItem(slotId, newModel._dataModel, false);
        _dataModel.UpdateCountsByType(newModel.TypeItem);
        _dataModel.UpdateCountsByType(oldModel.TypeItem);
        itemModels[slotId] = newModel;
        _itemsBySlots[slotId] = newModel;
        ChangedSlotId?.Invoke(slotId);
        return oldModel;
    }

    public ItemModel GetItemBySlot(int slotId)
    {
        return _itemsBySlots[slotId];
    }

    private void CleanList()
    {
        var count = itemModels.Count;
        for (int i = 0; i < count; i++)
            _itemModelFactory.Despawn(itemModels[i]);
        itemModels.Clear();
        _itemsBySlots.Clear();
    }

    public void OnDespawned()
    {
        CleanList();
        Changed = null;
    }
}