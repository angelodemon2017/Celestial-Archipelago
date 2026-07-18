using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldShowerService
{
    private readonly CatalogIslandConfigs _catalogConfig;
    private readonly DataService _dataService;
    private readonly MachingCubesMeshGenerator _machingCubesMeshGenerator;
    private readonly EntityRuntimeService _entityRuntimeService;
    private readonly EntityViewsFactory _entityViewsFactory;

    private List<IslandViewMB> _islandViewMBs = new();
    private List<EntityViewMB> _entityViewMBs = new();
    private Dictionary<(int,int), Queue<int>> _entsByChunk = new();

    public WorldShowerService(
        CatalogIslandConfigs catalogConfig,
        DataService dataService,
        MachingCubesMeshGenerator machingCubesMeshGenerator,
        EntityRuntimeService entityRuntimeService,
        EntityViewsFactory entityViewsFactory)
    {
        _catalogConfig = catalogConfig;
        _dataService = dataService;
        _machingCubesMeshGenerator = machingCubesMeshGenerator;
        _entityRuntimeService = entityRuntimeService;
        _entityViewsFactory = entityViewsFactory;
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

        foreach (var model in _entityRuntimeService.AllModels)
        {
            SpawnViewModelEntity(model);
        }
    }

    public void SpawnViewModelEntity(EntityModel entity)
    {
        var view = _entityViewsFactory.Spawn(entity);
        _entityViewMBs.Add(view);
    }

    private Queue<int> GetPoolByChunk(int x, int y)
    {
        if (!_entsByChunk.ContainsKey((x, y)))
            _entsByChunk[(x, y)] = new();
        return _entsByChunk[(x, y)];
    }
}