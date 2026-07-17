using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class WorldData
{
    public int Seed;

    private int _loadedChunksCount;
    public List<Vector2Int> LoadedChunks = new();

    public DataCollection<IslandData, ModelConfig> StaticIslands = new();

    public DataCollection<ContainerData, ContainerConfig> ContainerDatas = new();

    public DataCollection<CraftProcessData, RecipeConfig> CraftProcesses = new();

    public DataCollection<BurningFuelData, ItemConfig> Burnings = new();

    public WorldData(int newSeed)
    {
        Seed = newSeed;
    }

    public void SaveToBinary(BinaryWriter writer)
    {
        writer.Write(Seed);
        _loadedChunksCount = LoadedChunks.Count;
        writer.Write(_loadedChunksCount);
        for (int i = 0; i < _loadedChunksCount; i++)
        {
            writer.Write(LoadedChunks[i].x);
            writer.Write(LoadedChunks[i].y);
        }
        StaticIslands.SaveToBinary(writer);
        ContainerDatas.SaveToBinary(writer);
        CraftProcesses.SaveToBinary(writer);
        Burnings.SaveToBinary(writer);
    }

    public void LoadFromBinary(BinaryReader binaryReader)
    {
        Seed = binaryReader.ReadInt32();
        _loadedChunksCount = binaryReader.ReadInt32();
        for (int i = 0; i < _loadedChunksCount; i++)
        {
            LoadedChunks.Add(new Vector2Int(
                binaryReader.ReadInt32(),
                binaryReader.ReadInt32()));
        }
        StaticIslands.LoadFromBinary(binaryReader);
        ContainerDatas.LoadFromBinary(binaryReader);
        CraftProcesses.LoadFromBinary(binaryReader);
        Burnings.LoadFromBinary(binaryReader);
    }
}