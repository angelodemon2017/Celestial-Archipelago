using System.IO;
using Zenject;

[System.Serializable]
public class CraftProcessData : BaseData<RecipeConfig>, IPoolable<RecipeConfig>
{
    public int RecipeId;
    public int Process;
    public int TargetEntityId;
    public int SourceContainerId;
    public int ProductionContainerId;

    public void OnSpawned(RecipeConfig config)
    {
        base.InitConfig(config);
        RecipeId = config.Uid;
        Process = 0;
    }

    public void OnDespawned()
    {

    }

    public override void LoadFromBinary(BinaryReader binaryReader)
    {
        base.LoadFromBinary(binaryReader);
        RecipeId = binaryReader.ReadInt32();
        Process = binaryReader.ReadInt32();
        TargetEntityId = binaryReader.ReadInt32();
        SourceContainerId = binaryReader.ReadInt32();
        ProductionContainerId = binaryReader.ReadInt32();
    }

    public override void SaveToBinary(BinaryWriter writer)
    {
        base.SaveToBinary(writer);
        writer.Write(RecipeId);
        writer.Write(Process);
        writer.Write(TargetEntityId);
        writer.Write(SourceContainerId);
        writer.Write(ProductionContainerId);
    }
}