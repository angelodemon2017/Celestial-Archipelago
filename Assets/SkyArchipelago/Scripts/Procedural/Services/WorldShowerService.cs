using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldShowerService
{
    private readonly CatalogIslandConfigs _catalogConfig;
    private readonly CatalogEntityConfig _catalogEntityConfig;
    private readonly DataService _dataService;
    private readonly MachingCubesMeshGenerator _machingCubesMeshGenerator;
    private readonly EntityRuntimeService _entityRuntimeService;

    private List<IslandViewMB> _islandViewMBs = new();
    private List<EntityViewMB> _entityViewMBs = new();

    public WorldShowerService(
        CatalogIslandConfigs catalogConfig,
        CatalogEntityConfig catalogEntityConfig,
        DataService dataService,
        MachingCubesMeshGenerator machingCubesMeshGenerator,
        EntityRuntimeService entityRuntimeService)
    {
        _catalogConfig = catalogConfig;
        _catalogEntityConfig = catalogEntityConfig;
        _dataService = dataService;
        _machingCubesMeshGenerator = machingCubesMeshGenerator;
        _entityRuntimeService = entityRuntimeService;
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
            var view = GameObject.Instantiate(_catalogEntityConfig.entityViewMB, model.Position, model.Rotation);
            var itemConf = _catalogEntityConfig.GetItemByType(model.EntType);
            var viewModel = GameObject.Instantiate(itemConf.ViewModelPrefab);
            view.InitAndUpdate(model, viewModel);
            _entityViewMBs.Add(view);
        }
    }
}