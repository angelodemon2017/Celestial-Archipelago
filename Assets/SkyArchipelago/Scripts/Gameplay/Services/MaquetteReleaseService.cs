using Zenject;

public class MaquetteReleaseService
{
    private readonly SignalBus _signalBus;
    private readonly ContainersService _containersService;
    private readonly EntityRecipeCatalogManager _entityRecipeCatalogManager;
    private readonly EntitiesCatalogManager _entitiesCatalogManager;
    private readonly EntityRuntimeService _entityRuntimeService;
    private readonly DataService _dataService;
    private readonly WorldShowerService _worldShowerService;
    private readonly CraftingOperationService _craftingOperationService;

    public MaquetteReleaseService(
        SignalBus signalBus,
        ContainersService containersService,
        EntityRecipeCatalogManager entityRecipeCatalogManager,
        EntitiesCatalogManager entitiesCatalogManager,
        EntityRuntimeService entityRuntimeService,
        DataService dataService,
        CraftingOperationService craftingOperationService,
        WorldShowerService worldShowerService)
    {
        _signalBus = signalBus;
        _containersService = containersService;
        _entityRecipeCatalogManager = entityRecipeCatalogManager;
        _entitiesCatalogManager = entitiesCatalogManager;
        _entityRuntimeService = entityRuntimeService;
        _dataService = dataService;
        _craftingOperationService = craftingOperationService;
        _worldShowerService = worldShowerService;
    }

    public bool TryReleaseRecipeEntity(int idMaquette, int idContainer)
    {
        if (!(_entityRuntimeService.TryGetEntityById(idMaquette, out var entity) &&
            entity is MaquetteOfEntityModel moem))
            return false;

        if(!(_entityRecipeCatalogManager.TryGetConfigByKey(moem.GetIdRecipe, out var recipeConfig)))
            return false;

        var container = _containersService.GetContainerModelById(idContainer);

        if(_craftingOperationService.GetAvailableProductionByRecipe(recipeConfig, container._dataModel) <= 0)
            return false;

        _craftingOperationService.DeleteInputItems(container, recipeConfig);
        ReleaseMaquette(idMaquette);

        return true;
    }

    public void ReleaseMaquette(int idMaquette)
    {
        if (!(_entityRuntimeService.TryGetEntityById(idMaquette, out var entity) &&
            entity is MaquetteOfEntityModel moem))
                return;

        if (!(_entitiesCatalogManager.TryGetConfigByKey(moem.GetIdEntity, out var entityPare)))
            return;

        var entData = EntityDataMap.CreateData(entityPare.modelConfig.eEntityType);
        entData.InitConfig(entityPare.modelConfig);
        entData.position = moem.Position;
        entData.rotation = moem.Rotation;

        _signalBus.Fire(new EntityDeleteRequestSignal(idMaquette));
        _dataService.worldData.StaticIslands.Datas[0].entities.AddNewData(entData);
        var entModel = _entityRuntimeService.CreateEntityModel(entData);
        _entityRuntimeService.AddModel(entModel);
        _worldShowerService.SpawnViewModelEntity(entModel, false);
    }
}