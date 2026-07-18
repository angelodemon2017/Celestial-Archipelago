using System.IO;

[System.Serializable]
public class FurnaceData : EntityData
{
    public int ContainerInputId = -1;
    public int ContainerOutputId = -1;
    public int ContainerFuelId = -1;
    public int CraftIdProcess = -1;
    public int BurnIdProcess = -1;

    public FurnaceData() => EntityType = EEntityType.Furnace;

    public override EntityModel CreateModel()
    {
        return new FurnaceModel(this);
    }

    public override void InitConfig(ModelConfig config)
    {
        base.InitConfig(config);
        AvailableFlags |= CtxFlag.HaveContainers;
    }

    public override void SaveToBinary(BinaryWriter writer)
    {
        base.SaveToBinary(writer);
        writer.Write(ContainerInputId);
        writer.Write(ContainerOutputId);
        writer.Write(ContainerFuelId);
        writer.Write(CraftIdProcess);
        writer.Write(BurnIdProcess);
    }

    public override void LoadFromBinary(BinaryReader binaryReader)
    {
        base.LoadFromBinary(binaryReader);
        ContainerInputId = binaryReader.ReadInt32();
        ContainerOutputId = binaryReader.ReadInt32();
        ContainerFuelId = binaryReader.ReadInt32();
        CraftIdProcess = binaryReader.ReadInt32();
        BurnIdProcess = binaryReader.ReadInt32();
    }
}