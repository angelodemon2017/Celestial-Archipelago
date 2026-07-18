using System.IO;

[System.Serializable]
public class DroppedItemData : EntityData
{
    public int Count = 1;
    public EItemType TypeItem;

    public DroppedItemData() => EntityType = EEntityType.DroppedItem;

    public override EntityModel CreateModel()
    {
        return new DroppedItemModel(this);
    }

    public override void InitConfig(ModelConfig config)
    {
        base.InitConfig(config);
        AvailableFlags = CtxFlag.Item;
    }

    public override void SaveToBinary(BinaryWriter writer)
    {
        base.SaveToBinary(writer);
        writer.Write(Count);
        writer.Write((int)TypeItem);
    }

    public override void LoadFromBinary(BinaryReader binaryReader)
    {
        base.LoadFromBinary(binaryReader);
        Count = binaryReader.ReadInt32();
        TypeItem = (EItemType)binaryReader.ReadInt32();
    }
}