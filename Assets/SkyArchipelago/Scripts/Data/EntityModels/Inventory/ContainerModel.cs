using System;
using System.Collections.Generic;
using Zenject;

public class ContainerModel : BaseModel<ContainerData, ContainerConfig>, IPoolable<ContainerData>
{
    private readonly SimpleFactory<ContainerConfig, ContainerData> _containerDataFactory;
    private readonly ItemModelFactory _itemModelFactory;
    private readonly ContainerOperationsService _containerOperationsService;

    public override string ModelName => string.Empty;
    public byte Slots => _dataModel.Slots;
    public string TitleContainer => _configModel.KeyName;
    public List<ItemModel> itemModels = new();
    private Dictionary<byte, ItemModel> _itemsBySlots = new();

    public Action<int, byte> TestActionBySlot;
    public Action<byte> ChangedSlotId;

    public ContainerModel(
        SimpleFactory<ContainerConfig, ContainerData> containerDataFactory,
        ContainerOperationsService containerOperationsService,
        ItemModelFactory itemModelFactory)
    {
        _containerDataFactory = containerDataFactory;
        _containerOperationsService = containerOperationsService;
        _itemModelFactory = itemModelFactory;
    }

    public void OnSpawned(ContainerData containerData)
    {
        InitContainer(containerData);
    }

    public void InitContainer(ContainerData data)
    {
        _dataModel = data;
        CleanList();
        for (byte i = 0; i < _dataModel.Slots; i++)
        {
            var newIM = _itemModelFactory.Spawn(_dataModel.itemDatas[i]);
            newIM.SlotId = i;
            itemModels.Add(newIM);
            _itemsBySlots.Add(newIM.SlotId, newIM);
        }
    }

    public bool TryAddItemModel(ItemModel incomingItem)
    {
        if (incomingItem == null || incomingItem.TypeItem == EItemType.None)
            return false;

        var changes = _containerOperationsService.TryAddToContainer(_dataModel, incomingItem._dataModel);

        if (changes.Count > 0)
        {
            RefreshModelsAfterDataChange(changes);
//            Changed?.Invoke();
            return true;
        }
        else
            return false;
    }

    public void ClearSlot(byte slotId)
    {
        var emptData = _itemModelFactory.GetEmptyItemData();
        _dataModel.SwapItem(slotId, emptData);     
        RefreshModelBySlot(slotId);
    }

    private void RefreshModelsAfterDataChange(List<byte> changedSlotIndices)
    {
        foreach (var slot in changedSlotIndices)
        {
            RefreshModelBySlot(slot);
        }
    }

    private void RefreshModelBySlot(byte slot)
    {
        var dataInSlot = _dataModel.itemDatas[slot];
        var modelItem = _itemModelFactory.Spawn(dataInSlot);
        modelItem.SlotId = slot;
        SwapItemModel(slot, modelItem);
        ChangedSlotId?.Invoke(slot);
    }

    public void SwapItemModel(byte slotId, ItemModel newModel)
    {
        var oldModel = itemModels[slotId];
        if (oldModel != newModel)
            _itemModelFactory.Despawn(oldModel);

        newModel.SlotId = slotId;
        itemModels[slotId] = newModel;
        _itemsBySlots[slotId] = newModel;
    }

    public ItemModel GetItemModel(byte slotId)
    {
        return _itemsBySlots[slotId];
    }

    private void CleanList()
    {
        itemModels.ForEach(i => _itemModelFactory.Despawn(i));
        itemModels.Clear();
        _itemsBySlots.Clear();
    }

    public void OnDespawned()
    {
        _containerDataFactory.Despawn(_dataModel);
        CleanList();
        Changed = null;
    }
}