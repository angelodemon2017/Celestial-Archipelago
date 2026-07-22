using System;
using System.Collections.Generic;
using System.IO;
using Zenject;

[Serializable]
public class ContainerData : BaseData<ContainerConfig>, IPoolable<ContainerConfig>
{
    [NonSerialized]
    private readonly ItemsCatalogManager _itemsCatalogManager;
    [NonSerialized]
    private readonly SimpleFactory<ItemConfig, ItemData> _itemDataFactory;
    [NonSerialized]
    private readonly ItemModelFactory _itemModelFactory;

    public int IdEntityOwner;
    public int Slots;
    public List<ItemData> itemDatas = new();
    public ContainerAvailabilityFlag CurrentAvailabilitys;

    [NonSerialized]
    private Dictionary<EItemType, List<ItemData>> _mapItemsByType = new();
    [NonSerialized]
    private Dictionary<EItemType, int> _countsByType = new();
    [NonSerialized]
    private Dictionary<EItemType, int> _availableSpace = new();
    [NonSerialized]
    public int EmptySlots = 0;

    public ContainerData(
        ItemsCatalogManager itemsCatalogManager,
        SimpleFactory<ItemConfig, ItemData> itemDataFactory,
        ItemModelFactory itemModelFactory)
    {
        _itemsCatalogManager = itemsCatalogManager;
        _itemDataFactory = itemDataFactory;
        _itemModelFactory = itemModelFactory;
    }

    public void OnSpawned(ContainerConfig p1)
    {
        InitConfig(p1);
    }

    public override void InitConfig(ContainerConfig config)
    {
        base.InitConfig(config);
        Slots = config.BaseSlots;
        for (int i = 0; i < Slots; i++)
        {
            var item = _itemModelFactory.GetEmptyItemData();
            item.SlotId = i;
            itemDatas.Add(item);
            AddItem(item);
        }
        CurrentAvailabilitys = config.AvailabilityFlag;
    }

    public bool GetItemsByType(EItemType itemType, out List<ItemData> itemDatas)
    {
        return _mapItemsByType.TryGetValue(itemType, out itemDatas);
    }

    public int GetItemCountByType(EItemType itemType)
    {
        return _countsByType.ContainsKey(itemType) ? _countsByType[itemType] : 0;
    }

    public int GetAvailableSpaceByType(EItemType itemType)
    {
        return _availableSpace.ContainsKey(itemType) ? _availableSpace[itemType] : 0;
    }

    public void ReplaceItem(int index, ItemData item, bool withDespawn = true)
    {
        var oldData = itemDatas[index];
        _mapItemsByType[oldData.TypeItem].Remove(oldData);
        if (withDespawn)
            _itemDataFactory.Despawn(oldData);
        item.SlotId = index;
        itemDatas[index] = item;
        AddItem(item);
    }

    private void AddItem(ItemData itemData)
    {
        if (!_mapItemsByType.ContainsKey(itemData.TypeItem))
            _mapItemsByType.Add(itemData.TypeItem, new());
        _mapItemsByType[itemData.TypeItem].Add(itemData);
        EmptySlots += itemData.TypeItem == EItemType.None ? 1 : -1;
    }

    public void UpdateCountsByType(EItemType it)
    {
        var list = _mapItemsByType[it];
        var slotsCount = list.Count;
        if (slotsCount == 0)
        {
            _countsByType[it] = 0;
            _availableSpace[it] = 0;
            return;
        }
        int totalAmount = 0;
        var stackSize = 
            Config.CustomStackSize == 0 
                ? list[0].Config.MaxStack
                : Config.CustomStackSize;
        for (int i = 0; i < slotsCount; i++)
            totalAmount += list[i].Amount;
        _countsByType[it] = totalAmount;
        _availableSpace[it] = stackSize * (slotsCount) - totalAmount;
    }

    public void OnDespawned()
    {
        for (int i = 0; i < Slots; i++)
            _itemDataFactory.Despawn(itemDatas[i]);
        itemDatas.Clear();
        _mapItemsByType.Clear();
        _countsByType.Clear();
        _availableSpace.Clear();
        EmptySlots = 0;
        Slots = 0;
    }

    public override void LoadFromBinary(BinaryReader binaryReader)
    {
        base.LoadFromBinary(binaryReader);
        Slots = binaryReader.ReadInt32();
        IdEntityOwner = binaryReader.ReadInt32();
        for (int i = 0; i < Slots; i++)
        {
            var typeItem = (EItemType)binaryReader.ReadByte();
            _itemsCatalogManager.TryGetConfigByKey(typeItem, out var itemConfig);
            var item = _itemDataFactory.Spawn(itemConfig);
            item.LoadFromBinary(binaryReader);
            item.SlotId = i;
            itemDatas.Add(item);
            AddItem(item);
        }
    }

    public override void SaveToBinary(BinaryWriter writer)
    {
        base.SaveToBinary(writer);
        writer.Write(Slots);
        writer.Write(IdEntityOwner);
        itemDatas.ForEach(i => i.SaveToBinary(writer));
    }
}