using System.IO;
using UnityEngine;

[System.Serializable]
public class EntityData : BaseData<ModelConfig>
{
    public int ConfigId;
    public Vector3 position;
    public Quaternion rotation = Quaternion.identity;
    public CtxFlag AvailableFlags;

    public virtual string DebugLog => $"EntityData.{EntityType}.{Id}";
    public virtual EntityModel CreateModel()
    {
        return new EntityModel();
    }

    public override void InitConfig(ModelConfig config)
    {
        base.InitConfig(config);
        ConfigId = config.Uid;
        AvailableFlags = CtxFlag.None;
        for (int i = 0; i < config.ModuleConfigs.Count; i++)
            AvailableFlags |= config.ModuleConfigs[i].KeyFlag;
    }

    public override void LoadFromBinary(BinaryReader binaryReader)
    {
        base.LoadFromBinary(binaryReader);
        ConfigId = binaryReader.ReadInt32();
        position = new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
        rotation = new Quaternion(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
    }

    public override void SaveToBinary(BinaryWriter writer)
    {
        base.SaveToBinary(writer);
        writer.Write(ConfigId);
        writer.Write(position.x);
        writer.Write(position.y);
        writer.Write(position.z);
        writer.Write(rotation.x);
        writer.Write(rotation.y);
        writer.Write(rotation.z);
        writer.Write(rotation.w);
    }

    public virtual void ResetData()
    {
        Id = -1;
    }
}