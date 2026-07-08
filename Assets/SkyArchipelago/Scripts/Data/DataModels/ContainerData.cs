using System;
using System.Collections.Generic;
using System.IO;
using Zenject;

[System.Serializable]
public class ContainerData : BaseData<ContainerConfig>, IPoolable<ContainerConfig>
{
    private readonly ItemsCatalogConfig _itemsCatalogConfig;
    private readonly SimpleFactory<ItemConfig, ItemData> _itemDataFactory;

    public byte Slots;
    public List<ItemData> itemDatas = new();

    public ContainerData(
        ItemsCatalogConfig itemsCatalogConfig,
        SimpleFactory<ItemConfig, ItemData> itemDataFactory)
    {
        _itemsCatalogConfig = itemsCatalogConfig;
        _itemDataFactory = itemDataFactory;
    }

    public void OnSpawned(ContainerConfig p1)
    {
        InitConfig(p1);
    }

    public override void InitConfig(ContainerConfig config)
    {
        base.InitConfig(config);
        Slots = config.BaseSlots;
        var noneConfig = _itemsCatalogConfig.GetItemConfig(EItemType.None);
        for (int i = 0; i < Slots; i++)
        {
            itemDatas.Add(_itemDataFactory.Create(noneConfig));
        }
    }

    public int GetAllItemsCount(EItemType itemType)
    {
        int count = 0;
        foreach(var item in itemDatas)
            if (item.TypeItem == itemType)
                count += item.Amount;

        return count;
    }

    public bool AvailableSlots(ItemData item)
    {
        int maxStack = Config.CustomStackSize == 0 ?
            Math.Min(item.Config.MaxStack, Config.CustomStackSize) :
            Config.CustomStackSize;

        foreach (var i in itemDatas)
        {
            if (i.TypeItem == EItemType.None)
                return true;

            if (i.TypeItem == item.TypeItem &&
                i.Amount < maxStack)
                return true;
        }

        return false;
    }

    public void SwapItem(int index, ItemData item)
    {
        var oldData = itemDatas[index];
        _itemDataFactory.Despawn(oldData);
        itemDatas[index] = item;
    }

    public void OnDespawned()
    {
        itemDatas.ForEach(i => _itemDataFactory.Despawn(i));
        itemDatas.Clear();
    }

    public override void LoadFromBinary(BinaryReader binaryReader)
    {
        base.LoadFromBinary(binaryReader);
        Slots = binaryReader.ReadByte();
        var noneConfig = _itemsCatalogConfig.GetItemConfig(EItemType.None);//??
        for (int i = 0; i < Slots; i++)
        {
            var item = _itemDataFactory.Create(noneConfig);//??
            item.LoadFromBinary(binaryReader);
            var itemConfig = _itemsCatalogConfig.GetItemConfig(item.TypeItem);
            item.OnSpawned(itemConfig);
            itemDatas.Add(item);
        }
    }

    public override void SaveToBinary(BinaryWriter writer)
    {
        base.SaveToBinary(writer);
        writer.Write(Slots);
        itemDatas.ForEach(i => i.SaveToBinary(writer));
    }
}