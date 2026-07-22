[System.Serializable]
public class BuildFloorData : BaseBuildPartData
{
    public BuildFloorData() => EntityType = EEntityType.Floor;
    public override EntityModel CreateModel()
    {
        return new BuildPartModel(this);
    }
}