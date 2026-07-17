using System.Collections.Generic;
using UnityEngine;

public class DataService
{
    private WorldData _worldData;

    private int GetNewSeed => Random.Range(1000, 9999);
    public int GetSeed => _worldData.Seed;
    public (int, int) GetCurrentChunk => (0, 0);
    public List<IslandData> GetAllIslands => _worldData.StaticIslands.Datas;

    public WorldData worldData => _worldData;

    public DataService()
    {

    }

    public void CreateNewWorld()
    {
        _worldData = new WorldData(GetNewSeed);
    }

    public void SetChunk(int x, int y, List<IslandData> islands)
    {
        Vector2Int chunk = new Vector2Int(x, y);
        if (!_worldData.LoadedChunks.Contains(chunk))
        {
            islands.ForEach(i => _worldData.StaticIslands.AddNewData(i));
            _worldData.LoadedChunks.Add(chunk);
        }
    }

    public int AddNewContainer(ContainerData newContainerData)
    {   
        return _worldData.ContainerDatas.AddNewData(newContainerData);
    }

    public ContainerData GetContainer(int id)
    {
        return _worldData.ContainerDatas.GetElement(id);
    }

    public int AddNewCraft(CraftProcessData newCraftProcessData)
    {
        return _worldData.CraftProcesses.AddNewData(newCraftProcessData);
    }

    public bool TryGetCraft(int id, out CraftProcessData craftProcess)
    {
        return _worldData.CraftProcesses.TryGetElement(id, out craftProcess);
    }

    public void RemoveCraft(int craftId)
    {
        _worldData.CraftProcesses.RemoveData(craftId);
    }

    public int AddNewBurn(BurningFuelData newBurn)
    {
        return _worldData.Burnings.AddNewData(newBurn);
    }

    public bool TryGetBurn(int id, out BurningFuelData burnProcess)
    {
        return _worldData.Burnings.TryGetElement(id, out burnProcess);
    }

    public void RemoveBurn(int burnId)
    {
        _worldData.Burnings.RemoveData(burnId);
    }

    public List<IslandData> GetIslandsByChunk(int x, int y)
    {
        //calc data by arguments
        return _worldData.StaticIslands.Datas;
    }
}