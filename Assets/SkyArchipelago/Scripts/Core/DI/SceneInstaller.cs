using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private Camera _camera;

    public static DiContainer diContainer;
    [SerializeField] private List<GameObject> gameObjects;

    public override void InstallBindings()
    {
        diContainer = Container;
        gameObjects.ForEach(go => Container.Inject(go));

        var cs = diContainer.Resolve<CameraService>();
        cs.SetCamera(_camera);
    }

    public override void Start()
    {
        base.Start();
    }
}