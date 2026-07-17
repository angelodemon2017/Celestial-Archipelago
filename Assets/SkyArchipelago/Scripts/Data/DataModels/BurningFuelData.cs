using System.IO;
using Zenject;

[System.Serializable]
public class BurningFuelData : BaseData<ItemConfig>, IPoolable<ItemConfig>
{
    public EItemType TypeItemConfigId;
    public int TargetEntityId;
    public int Storage;

    public void OnSpawned(ItemConfig p1)
    {
        base.InitConfig(p1);
        TypeItemConfigId = p1.TypeItem;
        Storage = p1.FuelStorage;
    }

    public void OnDespawned()
    {
    }

    public override void SaveToBinary(BinaryWriter writer)
    {
        base.SaveToBinary(writer);
        writer.Write((int)TypeItemConfigId);
        writer.Write(TargetEntityId);
        writer.Write(Storage);
    }

    public override void LoadFromBinary(BinaryReader binaryReader)
    {
        base.LoadFromBinary(binaryReader);
        TypeItemConfigId = (EItemType)binaryReader.ReadInt32();
        TargetEntityId = binaryReader.ReadInt32();
        Storage = binaryReader.ReadInt32();
    }
}