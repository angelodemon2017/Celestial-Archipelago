using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class SpawnerDropsData : EntityData
{
    public int CurrentCountDrops;
    public List<int> OwnItems = new();

    public SpawnerDropsData() => EntityType = EEntityType.SpawnerDrops;
    public override EntityModel CreateModel()
    {
        return new SpawnerDropsModel(this);
    }

    public override void LoadFromBinary(BinaryReader binaryReader)
    {
        base.LoadFromBinary(binaryReader);
        CurrentCountDrops = binaryReader.ReadInt32();
        for (int i = 0; i < CurrentCountDrops; i++)
            OwnItems.Add(binaryReader.ReadInt32());
    }

    public override void SaveToBinary(BinaryWriter writer)
    {
        base.SaveToBinary(writer);
        writer.Write(CurrentCountDrops);
        for (int i = 0; i < CurrentCountDrops; i++)
            writer.Write(OwnItems[i]);
    }
}