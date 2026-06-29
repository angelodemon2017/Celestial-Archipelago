using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldShowerService
{
    private readonly CatalogIslandConfigs _catalogConfig;
    private readonly DataService _dataService;
    private readonly MachingCubesMeshGenerator _machingCubesMeshGenerator;

    private List<IslandViewMB> _islandViewMBs = new();

    public WorldShowerService(
        CatalogIslandConfigs catalogConfig,
        DataService dataService,
        MachingCubesMeshGenerator machingCubesMeshGenerator)
    {
        _catalogConfig = catalogConfig;
        _dataService = dataService;
        _machingCubesMeshGenerator = machingCubesMeshGenerator;
    }

    public void ShowChunk(int x, int y)
    {
        var islands = _dataService.GetIslandsByChunk(x, y);

        foreach (var isl in islands)
        {
            var config = _catalogConfig.Countering.FirstOrDefault(c => c.IdConfig == isl.ConfigId);
            var mesh = _machingCubesMeshGenerator.GenerateMeshFromDensity(isl.density, config.surfaceLevel, 1f, config);
            var island = GameObject.Instantiate(_catalogConfig.IslandPrefab, isl.Position, Quaternion.identity);
            island.InitAndUpdate(isl, mesh);

            _islandViewMBs.Add(island);
        }
    }
}