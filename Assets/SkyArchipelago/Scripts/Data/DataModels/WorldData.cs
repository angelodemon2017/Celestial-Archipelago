using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData
{
    public int Seed;

    public List<Vector2Int> LoadedChunks = new();

    public DataCollection<IslandData, ModelConfig> StaticIslands = new();

    public DataCollection<ContainerData, ContainerConfig> ContainerDatas = new();

    public WorldData(int newSeed)
    {
        Seed = newSeed;
    }

    public int AddNewContainer(ContainerData newContainerData)
    {
        return ContainerDatas.AddNewData(newContainerData);
    }

    public ContainerData GetContainer(int id) 
    {
        return ContainerDatas.GetElement(id);
    }
}