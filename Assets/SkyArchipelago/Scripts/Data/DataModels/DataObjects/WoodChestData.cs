using System.IO;

[System.Serializable]
public class WoodChestData : EntityData
{
    public int ContainerId = -1;

    public WoodChestData() => EntityType = EEntityType.WoodChest;

    public override EntityModel CreateModel()
    {
        return new WoodChestModel(this);
    }

    public override void SaveToBinary(BinaryWriter writer)
    {
        base.SaveToBinary(writer);
        writer.Write(ContainerId);
    }

    public override void LoadFromBinary(BinaryReader binaryReader)
    {
        base.LoadFromBinary(binaryReader);
        ContainerId = binaryReader.ReadInt32();
    }
}