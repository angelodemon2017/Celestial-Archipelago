using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [Header("Configs")]
    [SerializeField] private UIConfig _uIConfig;
    [SerializeField] private PlayerConfig _playerConfig;
    [SerializeField] private SystemSO _systemSO;
    [SerializeField] private DayNightSO _dayNightSO;

    [Header("MonoBehaviours")]
    [SerializeField] private CoroutineRunner _coroutineRunner;

    public override void InstallBindings()
    {
        InstallConfigs();
        InstallBrefabs();
        InstallGameStates();
        InstallModels();
        InstallMonoService();
        InstallServices();
        InstallUI();
        InstallSignals();
    }

    private void InstallConfigs()
    {
        Container.BindInstance(_playerConfig).AsSingle();
        Container.BindInstance(_systemSO).AsSingle();
        Container.BindInstance(_dayNightSO).AsSingle();
    }

    private void InstallBrefabs()
    {
        Container.BindInstance(_uIConfig._canvas).WithId(Dicts.DiPrefabIds.Canvas);
    }

    private void InstallModels()
    {
        Container.Bind<DialogModel>().AsSingle();
        Container.Bind<DayNightModel>().AsSingle();
    }

    private void InstallServices()
    {
        //Common
        Container.BindInterfacesAndSelfTo<HinterService>().AsSingle();
        Container.BindInterfacesAndSelfTo<PointsRepository>().AsSingle();
        
        Container.BindInterfacesAndSelfTo<CursorService>().AsSingle();
        Container.BindInterfacesAndSelfTo<InputService>().AsSingle();
        Container.BindInterfacesAndSelfTo<RaycastService>().AsSingle();
        Container.BindInterfacesAndSelfTo<CameraService>().AsSingle();
        Container.BindInterfacesAndSelfTo<PlayerInteractionService>().AsSingle();
        Container.BindInterfacesAndSelfTo<InteractionHandlerService>().AsSingle();

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
        Container.BindInterfacesAndSelfTo<MainFPSState>().AsSingle();
        Container.BindInterfacesAndSelfTo<MainMenuState>().AsSingle();
        Container.BindInterfacesAndSelfTo<ManagerMenuState>().AsSingle();
        Container.BindInterfacesAndSelfTo<PauseMenuState>().AsSingle();
    }

    private void InstallMonoService()
    {
        Container.BindInstance(_coroutineRunner).AsSingle();
    }

    private void InstallUI()
    {
        Container.Bind<MenuOfManagerView>().FromComponentInNewPrefab(_uIConfig.menuOfManagerView).AsSingle();
        Container.Bind<GameplayControllerView>().FromComponentInNewPrefab(_uIConfig.gameplayControllerView).AsSingle();
        Container.Bind<MainMenuControllerView>().FromComponentInNewPrefab(_uIConfig.mainMenuControllerView).AsSingle();
        Container.Bind<DialogMenuUI>().FromComponentInNewPrefab(_uIConfig.dialogMenuUI).AsSingle();
        Container.Bind<PauseMenuUI>().FromComponentInNewPrefab(_uIConfig.pauseMenuUI).AsSingle();
    }

    private void InstallSignals()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<TimeUpdateSignal>();
        Container.DeclareSignal<TimeSecondSignal>();
        Container.DeclareSignal<LoadSceneSignal>();
        Container.DeclareSignal<SceneLoadedSignal>();
        Container.DeclareSignal<SceneLoadingProgressSignal>();

        Container.DeclareSignal<LaunchSpawnPointSignal>();
    }
}