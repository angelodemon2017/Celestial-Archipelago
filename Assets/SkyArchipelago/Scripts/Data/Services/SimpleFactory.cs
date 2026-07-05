using System.Collections.Generic;
using Zenject;

public class SimpleFactory<InitModel, ModelOfFabric>
    where ModelOfFabric : IPoolable<InitModel>
{
    private readonly DiContainer _container;
    private readonly Queue<ModelOfFabric> _pool = new Queue<ModelOfFabric>();

    public SimpleFactory(
        DiContainer container)
    {
        _container = container;
    }

    public ModelOfFabric Create(InitModel initModel)
    {
        ModelOfFabric instance;

        if (_pool.Count > 0)
        {
            instance = _pool.Dequeue();
        }
        else
        {
            instance = _container.Instantiate<ModelOfFabric>();
        }

        instance.OnSpawned(initModel);
        return instance;
    }

    public void Despawn(ModelOfFabric data)
    {
        data.OnDespawned();
        _pool.Enqueue(data);
    }
}