using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private List<GameObject> gameObjects;

    public override void InstallBindings()
    {
        gameObjects.ForEach(go => Container.Inject(go));
    }

    public override void Start()
    {
        base.Start();
    }
}