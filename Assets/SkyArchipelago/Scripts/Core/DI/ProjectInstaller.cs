using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [Header("Configs")]
    [SerializeField] private SystemSO _systemSO;
    [SerializeField] private DayNightSO _dayNightSO;

    [Header("MonoBehaviours")]
    [SerializeField] private CoroutineRunner _coroutineRunner;

    public override void InstallBindings()
    {
        InstallConfigs();
        InstallModels();
        InstallMonoService();
        InstallServices();
        InstallSignals();
    }

    private void InstallConfigs()
    {
        Container.BindInstance(_systemSO).AsSingle();
        Container.BindInstance(_dayNightSO).AsSingle();
    }

    private void InstallModels()
    {
        Container.Bind<DayNightModel>().AsSingle();
    }

    private void InstallServices()
    {
        Container.BindInterfacesAndSelfTo<GameTimeService>().AsSingle();
//        Container.BindInterfacesAndSelfTo<GameTimeService_demo>().AsSingle();
        Container.BindInterfacesAndSelfTo<DayNightService>().AsSingle();
        Container.BindInterfacesAndSelfTo<ProceduralMeshService>().AsSingle();
        Container.BindInterfacesAndSelfTo<SceneLoadingService>().AsSingle();
    }

    private void InstallMonoService()
    {
        Container.BindInstance(_coroutineRunner).AsSingle();
    }

    private void InstallSignals()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<TimeUpdateSignal>();
        Container.DeclareSignal<TimeSecondSignal>();
        Container.DeclareSignal<LoadSceneSignal>();
        Container.DeclareSignal<SceneLoadedSignal>();
        Container.DeclareSignal<SceneLoadingProgressSignal>();
    }
}