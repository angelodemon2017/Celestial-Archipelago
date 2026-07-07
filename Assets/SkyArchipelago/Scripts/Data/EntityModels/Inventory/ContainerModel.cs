using System;
using System.Collections.Generic;
using Zenject;

public class ContainerModel : BaseModel<ContainerData, ContainerConfig>, IPoolable<ContainerData>
{
    private readonly SimpleFactory<ContainerConfig, ContainerData> _containerDataFactory;
    private readonly ItemModelFactory _itemModelFactory;

    public override string ModelName => string.Empty;
    public string TitleContainer => _configModel.KeyName;
    public List<ItemModel> itemModels = new();

    public ContainerModel(
        SimpleFactory<ContainerConfig, ContainerData> containerDataFactory,
        ItemModelFactory itemModelFactory)
    {
        _containerDataFactory = containerDataFactory;
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
        for (int i = 0; i < _dataModel.Slots; i++)
        {
            itemModels.Add(_itemModelFactory.Spawn(_dataModel.itemDatas[i]));
        }
    }

    public bool AvailableSlots(ItemModel itemModel)
    {
        if ((_configModel.Availableitems & itemModel.GetTag) == CtxFlag.None)
            return false;

        if (!_dataModel.AvailableSlots(itemModel._dataModel))
            return false;

        return true;
    }

    public int AddItem(ItemModel addedItem)
    {
        int maxStack = _configModel.CustomStackSize != 0 ?
            Math.Min(addedItem._configModel.MaxStack, _configModel.CustomStackSize) :
            addedItem._configModel.MaxStack;

        int workAmount = addedItem._dataModel.Amount;
        for (var i = 0; i < _dataModel.Slots; i++)
        {
            var im = itemModels[i];
            if (im.TypeItem == addedItem._dataModel.TypeItem &&
                im._dataModel.Amount < maxStack)
            {
                int amountToAdd = Math.Min(workAmount, maxStack - im._dataModel.Amount);
                im._dataModel.Amount += amountToAdd;
                workAmount -= amountToAdd;
                if (workAmount > 0)
                    continue;
                else
                    return workAmount;
            }
        }
        addedItem._dataModel.Amount = workAmount;

        for (var i = 0; i < _dataModel.Slots; i++)
        {
            var im = itemModels[i];
            if (im.TypeItem == EItemType.None)
            {
                _dataModel.SwapItem(i, addedItem._dataModel);
                SwapItem(i, addedItem);
                workAmount = 0;
                return workAmount;
            }
        }

        return workAmount;
    }

    private void SwapItem(int index, ItemModel itemModel)
    {
        var oldData = itemModels[index];
        _itemModelFactory.Despawn(oldData);
        itemModels[index] = itemModel;
    }

    private void CleanList()
    {
        itemModels.ForEach(i => _itemModelFactory.Despawn(i));
        itemModels.Clear();
    }

    public void OnDespawned()
    {
        _containerDataFactory.Despawn(_dataModel);
        CleanList();
        Changed = null;
    }
}