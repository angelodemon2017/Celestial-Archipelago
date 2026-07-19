using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [Header("Configs")]
    [SerializeField] private UIConfig _uIConfig;
    [SerializeField] private PlayerConfig _playerConfig;
    [SerializeField] private SystemSO _systemSO;
    [SerializeField] private DayNightSO _dayNightSO;
    [SerializeField] private BuildFPSStateConfig _buildFPSState;
    [SerializeField] private CatalogIslandConfigs _catalogIslandConfigs;
    [SerializeField] private EntityViewCatalog _entityViewCatalog;
    [SerializeField] private ItemsCatalogConfig _itemsCatalogConfig;
    [SerializeField] private ContainersCatalogConfig _containersCatalogConfig;
    [SerializeField] private RecipesCatalogConfig _recipesCatalogConfig;
    [SerializeField] private EntitiesRecipesCatalogConfig _entitiesRecipesCatalogConfig;

    [SerializeField] private WorldGeneratorConfig _worldGeneratorConfig;

    [Header("Prefabs")]
    [SerializeField] private HitSource _hitSource;

    [Header("MonoBehaviours")]
    [SerializeField] private CoroutineRunner _coroutineRunner;

    public override void InstallBindings()
    {
        InstallConfigs();
        InstallManagersOfCatalogs();
        InstallBrefabs();
        InstallPoolsAndFabrics();
        InstallGameStates();
        InstallModels();
        InstallMonoService();
        InstallHandlers();
        InstallServices();
        InstallUI();
        InstallSignals();
    }

    private void InstallConfigs()
    {
        Container.BindInstance(_playerConfig).AsSingle();
        Container.BindInstance(_systemSO).AsSingle();
        Container.BindInstance(_dayNightSO).AsSingle();
        Container.BindInstance(_buildFPSState).AsSingle();
        Container.BindInstance(_catalogIslandConfigs).AsSingle();
        Container.BindInstance(_entityViewCatalog).AsSingle();
        Container.BindInstance(_itemsCatalogConfig).AsSingle();
        Container.BindInstance(_containersCatalogConfig).AsSingle();
        Container.BindInstance(_recipesCatalogConfig).AsSingle();
        Container.BindInstance(_entitiesRecipesCatalogConfig).AsSingle();
        Container.BindInstance(_worldGeneratorConfig).AsSingle();
    }

    private void InstallManagersOfCatalogs()
    {
        Container.Bind<RecipeCatalogManager>().AsSingle();
        Container.Bind<EntityRecipeCatalogManager>().AsSingle();
        Container.Bind<ItemsCatalogManager>().AsSingle();
        Container.Bind<ContainersCatalogManager>().AsSingle();
        Container.Bind<EntitiesCatalogManager>().AsSingle();
    }

    private void InstallBrefabs()
    {
        Container.Bind<EntityViewMB>().FromComponentInNewPrefab(_entityViewCatalog.entityViewPrefab).AsTransient();
        Container.Bind<HitSource>().FromComponentInNewPrefab(_hitSource).AsTransient();
        Container.Bind<BuildMarkerMB>().FromComponentInNewPrefab(_buildFPSState.buildMarkerPrefab).AsSingle();
        Container.BindInstance(_uIConfig._canvas).WithId(Dicts.DiPrefabIds.Canvas);
    }

    private void InstallPoolsAndFabrics()
    {
        Container.Bind<SimpleFactory<ItemConfig, ItemData>>().AsSingle();
        Container.Bind<SimpleFactory<ContainerConfig, ContainerData>>().AsSingle();

        Container.BindInterfacesAndSelfTo<ItemModelFactory>().AsSingle();
        Container.Bind<SimpleFactory<ContainerData, ContainerModel>>().AsSingle();

        Container.Bind<SimpleFactory<ItemConfig, BurningFuelData>>().AsSingle();
        Container.Bind<SimpleFactory<RecipeConfig, CraftProcessData>>().AsSingle();
        Container.Bind<SimpleFactory<CraftProcessData, CraftProcessModel>>().AsSingle();

        Container.Bind<UIMBFactory<HitSourceInitModel, HitSource>>().AsSingle();
        Container.Bind<UIMBFactory<EntityRootHandlerMB, EntityViewMB>>().AsSingle();
        Container.BindInterfacesAndSelfTo<EntityViewsFactory>().AsSingle();
    }

    private void InstallModels()
    {
        Container.Bind<RecipeGlossaryRepository>().AsSingle();
        Container.Bind<GameplayLocalFPSModel>().AsSingle();
        Container.Bind<MainMenuModel>().AsSingle();
        Container.Bind<FPSCommonModel>().AsSingle();
        Container.BindInterfacesAndSelfTo<BuildingModel>().AsSingle();
        Container.Bind<DialogModel>().AsSingle();
        Container.Bind<MenuOfEntityModel>().AsSingle();
        Container.Bind<DayNightModel>().AsSingle();
    }

    private void InstallHandlers()
    {
        Container.BindInterfacesAndSelfTo<CancellingMaquetteHandler>().AsSingle();
        Container.BindInterfacesAndSelfTo<FullingMaquetteHandler>().AsSingle();
        Container.BindInterfacesAndSelfTo<DamagableHandler>().AsSingle();
        Container.BindInterfacesAndSelfTo<PickUpHandler>().AsSingle();
        Container.BindInterfacesAndSelfTo<HarvestHandler>().AsSingle();
        Container.BindInterfacesAndSelfTo<DebugLabelHandlers>().AsSingle();
        Container.BindInterfacesAndSelfTo<ShowUIHandler>().AsSingle();
    }
    
    private void InstallServices()
    {
        Container.Bind<EntityUIviewFactory>().AsSingle();
        //Common
        Container.BindInterfacesAndSelfTo<HinterService>().AsSingle();
        Container.BindInterfacesAndSelfTo<UIDragAndDropItemsService>().AsSingle();

        Container.BindInterfacesAndSelfTo<DataService>().AsSingle();
        Container.BindInterfacesAndSelfTo<ContainersService>().AsSingle();
        Container.BindInterfacesAndSelfTo<ContainerOperationsService>().AsSingle();
        Container.BindInterfacesAndSelfTo<InventoryTransactionsService>().AsSingle();
        Container.BindInterfacesAndSelfTo<SelectingCraftService>().AsSingle();
        Container.BindInterfacesAndSelfTo<CraftingOperationService>().AsSingle();
        Container.BindInterfacesAndSelfTo<CraftProcessRepository>().AsSingle();
        Container.BindInterfacesAndSelfTo<RuntimeCraftHandlerService>().AsSingle();
        Container.BindInterfacesAndSelfTo<RecipesGlossaryService>().AsSingle();
        Container.BindInterfacesAndSelfTo<EntityRuntimeService>().AsSingle();
        
        Container.BindInterfacesAndSelfTo<SpawnerDropItemsService>().AsSingle();
        Container.BindInterfacesAndSelfTo<PeriodicSpawningSystem>().AsSingle();
        Container.BindInterfacesAndSelfTo<BurningFuelsRepository>().AsSingle();
        Container.BindInterfacesAndSelfTo<RuntimeBurningsHandlerService>().AsSingle();

        Container.BindInterfacesAndSelfTo<CursorService>().AsSingle();
        Container.BindInterfacesAndSelfTo<InputService>().AsSingle();
        Container.BindInterfacesAndSelfTo<RaycastService>().AsSingle();
        Container.BindInterfacesAndSelfTo<CameraService>().AsSingle();
        Container.BindInterfacesAndSelfTo<HitsCoordinatorService>().AsSingle();
        Container.BindInterfacesAndSelfTo<InteractableCoordinatorService>().AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerInteractionService>().AsSingle();
        Container.BindInterfacesAndSelfTo<InteractionHandlerService>().AsSingle();
        Container.BindInterfacesAndSelfTo<MaquettePlacementService>().AsSingle();
        Container.BindInterfacesAndSelfTo<MaquetteReleaseService>().AsSingle();
        
        Container.BindInterfacesAndSelfTo<MachingCubesMeshGenerator>().AsSingle();
        Container.BindInterfacesAndSelfTo<WorldGeneratorService>().AsSingle();
        Container.BindInterfacesAndSelfTo<WorldShowerService>().AsSingle();
        Container.BindInterfacesAndSelfTo<GameplayStateService>().AsSingle();
        Container.BindInterfacesAndSelfTo<UIViewCoordinator>().AsSingle();

        Container.BindInterfacesAndSelfTo<GameTimeService>().AsSingle();
        Container.BindInterfacesAndSelfTo<DayNightService>().AsSingle();
        Container.BindInterfacesAndSelfTo<ProceduralMeshService>().AsSingle();
        Container.BindInterfacesAndSelfTo<SceneLoadingService>().AsSingle();
    }

    private void InstallGameStates()
    {
        Container.BindInterfacesAndSelfTo<BattleFPSState>().AsSingle();
        Container.BindInterfacesAndSelfTo<BuildingFPSState>().AsSingle();
        Container.BindInterfacesAndSelfTo<DialogMenuState>().AsSingle();
        Container.BindInterfacesAndSelfTo<LaunchWorldState>().AsSingle();
        Container.BindInterfacesAndSelfTo<MainFPSState>().AsSingle();
        Container.BindInterfacesAndSelfTo<MainMenuState>().AsSingle();
        Container.BindInterfacesAndSelfTo<ManagerMenuState>().AsSingle();
        Container.BindInterfacesAndSelfTo<MenuOfEntityState>().AsSingle();
        Container.BindInterfacesAndSelfTo<PauseMenuState>().AsSingle();
    }

    private void InstallMonoService()
    {
        Container.BindInstance(_coroutineRunner).AsSingle();
    }

    private void InstallUI()
    {
        _uIConfig.InstallPrefabs(Container);
    }
    
    private void InstallSignals()
    {
        SignalBusInstaller.Install(Container);
        
        Container.DeclareSignal<StartPlaceEntity>();
        Container.DeclareSignal<SelectRecipeEntity>();
        Container.DeclareSignal<SelectEntityCategory>();
        Container.DeclareSignal<SelectRecipe>();
        Container.DeclareSignal<HandleCraft>();
        Container.DeclareSignal<ExchangeItemContainersSignal>();
        Container.DeclareSignal<MoveItemBetweenContainersSignal>();
        Container.DeclareSignal<MoveAmountBetweenSlotsSignal>();
        Container.DeclareSignal<ContainerOfEntityRequest>();
        Container.DeclareSignal<ContainerUpdatedSignal>();
        Container.DeclareSignal<EntitiesUpdatedSignal>();
        Container.DeclareSignal<EntityDeleteRequestSignal>();
        Container.DeclareSignal<InteractContext>();
        Container.DeclareSignal<TimeUpdateSignal>();
        Container.DeclareSignal<TimeSecondSignal>();
        Container.DeclareSignal<LoadSceneSignal>();
        Container.DeclareSignal<SceneLoadedSignal>();
        Container.DeclareSignal<SceneInstalledSignal>();
        Container.DeclareSignal<SceneLoadingProgressSignal>();
    }
}