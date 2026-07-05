[System.Serializable]
public class SpawnPointData : EntityData
{
    public SpawnPointData() => EntityType = EEntityType.SpawnPoint;
    public override EntityModel CreateModel()
    {
        return new SpawnPointModel(this);
    }
}