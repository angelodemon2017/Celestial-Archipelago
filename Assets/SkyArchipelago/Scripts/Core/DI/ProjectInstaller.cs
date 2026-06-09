using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        InstallConfigs();
        InstallServices();
        InstallSignals();
    }

    private void InstallConfigs()
    {

    }

    private void InstallServices()
    {
        Container.BindInterfacesAndSelfTo<ProceduralMeshService>().AsSingle();
        
    }

    private void InstallSignals()
    {
        SignalBusInstaller.Install(Container);
    }
}