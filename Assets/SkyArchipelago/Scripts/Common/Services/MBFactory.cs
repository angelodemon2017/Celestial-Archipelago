using UnityEngine;
using Zenject;

public class MBFactory<InitModel, MonoBehObj>
    where MonoBehObj : MonoBehaviour
{
    private readonly MemoryPool<InitModel, MonoBehObj> _pool;

    public MBFactory(MemoryPool<InitModel, MonoBehObj> pool)
    {
        _pool = pool;
    }

    public MonoBehObj Create(InitModel itemModel, Transform parent = null)
    {
        var view = _pool.Spawn(itemModel);
        if (parent != null)
            view.transform.SetParent(parent, false);
        return view;
    }

    public void Despawn(MonoBehObj view) => _pool.Despawn(view);
}