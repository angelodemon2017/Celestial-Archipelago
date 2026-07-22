[System.Serializable]
public class BuildWallData : BaseBuildPartData
{
    public BuildWallData() => EntityType = EEntityType.Wall;
    public override EntityModel CreateModel()
    {
        return new BuildPartModel(this);
    }
}