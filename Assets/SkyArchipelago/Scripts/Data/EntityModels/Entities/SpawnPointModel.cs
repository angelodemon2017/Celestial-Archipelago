using UnityEngine;

public class SpawnPointModel : EntityModel<SpawnPointData>
{
    public Vector3 SpawnPoint => Position + Vector3.up;

    public SpawnPointModel(SpawnPointData data) : base(data)
    {
    }
}