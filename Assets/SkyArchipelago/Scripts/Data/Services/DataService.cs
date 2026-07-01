using System.Collections.Generic;
using UnityEngine;

public class DataService
{
    private WorldData _worldData;

    private int GetNewSeed => Random.Range(1000, 9999);

    public int GetSeed => _worldData.Seed;
    public (int, int) GetCurrentChunk => (0, 0);
    public List<IslandData> GetAllIslands => _worldData.StaticIslands.Datas;

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

    public List<IslandData> GetIslandsByChunk(int x, int y)
    {
        //calc data by arguments
        return _worldData.StaticIslands.Datas;
    }
}