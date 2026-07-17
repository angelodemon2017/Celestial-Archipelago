using System.IO;

[System.Serializable]
public class DemoNPCData : EntityData
{
    public string NpcId = "Demo NPC";
    public DemoNPCData() => EntityType = EEntityType.DemoNPC;
    public override EntityModel CreateModel()
    {
        return new DemoNPCModel(this);
    }

    public override void LoadFromBinary(BinaryReader binaryReader)
    {
        base.LoadFromBinary(binaryReader);

        NpcId = binaryReader.ReadString();
    }

    public override void SaveToBinary(BinaryWriter writer)
    {
        base.SaveToBinary(writer);

        writer.Write(NpcId ?? string.Empty);
    }

    public override void InitConfig(ModelConfig config)
    {
        base.InitConfig(config);
        AvailableFlags |= CtxFlag.UIHave;
        NpcId = config.ContentName;
    }
}