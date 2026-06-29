using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData
{
    public int Seed;

    public List<Vector2Int> LoadedChunks = new();

    public DataCollection<IslandData> StaticIslands = new();

    public DataCollection<ContainerData> ContainerDatas = new();

    public WorldData(int newSeed)
    {
        Seed = newSeed;
    }
}