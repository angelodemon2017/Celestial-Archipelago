[System.Serializable]
public class DebugLabelData : EntityData
{
    public DebugLabelData() => EntityType = EEntityType.DebugLabel;
    public override EntityModel CreateModel()
    {
        return new DebugLabelModel(this);
    }
}