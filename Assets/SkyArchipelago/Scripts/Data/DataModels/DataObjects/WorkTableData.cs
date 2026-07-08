using System.IO;

[System.Serializable]
public class WorkTableData : EntityData
{
    public WorkTableData() => EntityType = EEntityType.WorkTable;

    public override EntityModel CreateModel()
    {
        return new WorkTableModel(this);
    }

    public override void SaveToBinary(BinaryWriter writer)
    {
        base.SaveToBinary(writer);
    }

    public override void LoadFromBinary(BinaryReader binaryReader)
    {
        base.LoadFromBinary(binaryReader);
    }
}