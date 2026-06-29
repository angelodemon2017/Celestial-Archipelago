using UnityEngine;

[System.Serializable]
public class IslandData : BaseData
{
    public int ConfigId;
    public Vector3 Position;
    public Vector3Int Center;
    public Vector3Int IslandSize;
    public float[,,] density;
    public DataCollection<EntityData> entities = new();
}
