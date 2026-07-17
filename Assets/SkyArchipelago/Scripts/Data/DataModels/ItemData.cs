using System;
using System.IO;
using Zenject;

[System.Serializable]
public class ItemData : BaseData<ItemConfig>, IPoolable<ItemConfig>
{
    public EItemType TypeItem;
    public int Amount;
    public string SomePrefix = string.Empty;//enchant

    [NonSerialized]
    public int SlotId;

    public override void Copy<T2>(T2 data)
    {
        base.Copy(data);
        if (data is ItemData item)
        {
            TypeItem = item.TypeItem;
            Amount = item.Amount;
            SomePrefix = item.SomePrefix;
        }
    }

    public void OnSpawned(ItemConfig memoryPool)
    {
        InitConfig(memoryPool);
    }

    public override void InitConfig(ItemConfig config)
    {
        base.InitConfig(config);
        EntityType = EEntityType.Item;
        TypeItem = config.TypeItem;
    }

    public void OnDespawned()
    {
        TypeItem = EItemType.None;
        Amount = 0;
        SomePrefix = string.Empty;
    }

    public override void LoadFromBinary(BinaryReader binaryReader)
    {
        base.LoadFromBinary(binaryReader);
        Amount = binaryReader.ReadInt32();
    }

    public override void SaveToBinary(BinaryWriter writer)
    {
        base.SaveToBinary(writer);
        writer.Write((byte)TypeItem);
        writer.Write(Amount);
    }
}