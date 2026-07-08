using System;
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
    [SerializeField] private WorldGeneratorConfig _worldGeneratorConfig;

    [Header("Prefabs")]
    [SerializeField] private IconViewMB _iconViewPrefab;
    [SerializeField] private ItemsListViewMB _itemsListViewMB;
    [SerializeField] private HitSource _hitSource;

    [Header("MonoBehaviours")]
    [SerializeField] private CoroutineRunner _coroutineRunner;

    public override void InstallBindings()
    {
        InstallConfigs();
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
        Container.BindInstance(_catalogIslandConfigs).AsSingle();
        Container.BindInstance(_entityViewCatalog).AsSingle();
        Container.BindInstance(_itemsCatalogConfig).AsSingle();
        Container.BindInstance(_containersCatalogConfig).AsSingle();
        Container.BindInstance(_worldGeneratorConfig).AsSingle();
    }

    private void InstallBrefabs()
    {
        Container.Bind<EntityViewMB>().FromComponentInNewPrefab(_entityViewCatalog.entityViewPrefab).AsTransient();
        Container.Bind<IconViewMB>().FromComponentInNewPrefab(_iconViewPrefab).AsTransient();
        Container.Bind<ItemsListViewMB>().FromComponentInNewPrefab(_itemsListViewMB).AsTransient();
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

        Container.Bind<UIMBFactory<ItemModel, IconViewMB>>().AsSingle();
        Container.Bind<UIMBFactory<ContainerModel, ItemsListViewMB>>().AsSingle();

        Container.Bind<UIMBFactory<HitSourceInitModel, HitSource>>().AsSingle();
        Container.Bind<UIMBFactory<EntityRootHandlerMB, EntityViewMB>>().AsSingle();
        Container.Bind<EntityViewsFactory>().AsSingle();
    }

    private void InstallModels()
    {
        Container.Bind<MainMenuModel>().AsSingle();
        Container.Bind<FPSCommonModel>().AsSingle();
        Container.Bind<BuildingModel>().AsSingle();
        Container.Bind<DialogModel>().AsSingle();
        Container.Bind<MenuEntityWithInventoryModel>().AsSingle();
        Container.Bind<DayNightModel>().AsSingle();
    }

    private void InstallHandlers()
    {
        Container.BindInterfacesAndSelfTo<DamagableHandler>().AsSingle();
        Container.BindInterfacesAndSelfTo<PickUpHandler>().AsSingle();
        Container.BindInterfacesAndSelfTo<HarvestHandler>().AsSingle();
        Container.BindInterfacesAndSelfTo<DebugLabelHandlers>().AsSingle();
        Container.BindInterfacesAndSelfTo<ShowUIHandler>().AsSingle();
    }

    private void InstallServices()
    {
        //Common
        Container.BindInterfacesAndSelfTo<HinterService>().AsSingle();

        Container.BindInterfacesAndSelfTo<DataService>().AsSingle();
        Container.BindInterfacesAndSelfTo<ContainerOperationsService>().AsSingle();        
        Container.BindInterfacesAndSelfTo<InventoryTransactionsService>().AsSingle();
        Container.BindInterfacesAndSelfTo<ContainersService>().AsSingle();
        Container.BindInterfacesAndSelfTo<EntityRuntimeService>().AsSingle();

        Container.BindInterfacesAndSelfTo<CursorService>().AsSingle();
        Container.BindInterfacesAndSelfTo<InputService>().AsSingle();
        Container.BindInterfacesAndSelfTo<RaycastService>().AsSingle();
        Container.BindInterfacesAndSelfTo<CameraService>().AsSingle();
        Container.BindInterfacesAndSelfTo<HitsCoordinatorService>().AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerInteractionService>().AsSingle();
        Container.BindInterfacesAndSelfTo<InteractionHandlerService>().AsSingle();
        
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
        Container.BindInterfacesAndSelfTo<InventoryMenuState>().AsSingle();
        Container.BindInterfacesAndSelfTo<LaunchWorldState>().AsSingle();
        Container.BindInterfacesAndSelfTo<MainFPSState>().AsSingle();
        Container.BindInterfacesAndSelfTo<MainMenuState>().AsSingle();
        Container.BindInterfacesAndSelfTo<ManagerMenuState>().AsSingle();
        Container.BindInterfacesAndSelfTo<MenuOfEntityWithInventoryState>().AsSingle();
        Container.BindInterfacesAndSelfTo<PauseMenuState>().AsSingle();
    }

    private void InstallMonoService()
    {
        Container.BindInstance(_coroutineRunner).AsSingle();
    }

    private void InstallUI()
    {
        Container.Bind<MenuOfEntityWithInventoryView>().FromComponentInNewPrefab(_uIConfig.menuOfEntityWithInventory).AsSingle();
        Container.Bind<MenuOfManagerView>().FromComponentInNewPrefab(_uIConfig.menuOfManagerView).AsSingle();
        Container.Bind<GameplayControllerView>().FromComponentInNewPrefab(_uIConfig.gameplayControllerView).AsSingle();
        Container.Bind<InventoryView>().FromComponentInNewPrefab(_uIConfig.inventoryView).AsSingle();
        Container.Bind<MainMenuControllerView>().FromComponentInNewPrefab(_uIConfig.mainMenuControllerView).AsSingle();
        Container.Bind<DialogMenuUI>().FromComponentInNewPrefab(_uIConfig.dialogMenuUI).AsSingle();
        Container.Bind<PauseMenuUI>().FromComponentInNewPrefab(_uIConfig.pauseMenuUI).AsSingle();
    }

    private void InstallSignals()
    {
        SignalBusInstaller.Install(Container);
        
        Container.DeclareSignal<InteractContext>();
        Container.DeclareSignal<TimeUpdateSignal>();
        Container.DeclareSignal<TimeSecondSignal>();
        Container.DeclareSignal<MoveItemBetweenContainersSignal>();
        Container.DeclareSignal<LoadSceneSignal>();
        Container.DeclareSignal<SceneLoadedSignal>();
        Container.DeclareSignal<SceneInstalledSignal>();
        Container.DeclareSignal<SceneLoadingProgressSignal>();
    }
}