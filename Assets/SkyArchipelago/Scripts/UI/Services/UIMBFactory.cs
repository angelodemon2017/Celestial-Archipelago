using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UIMBFactory<InitModel, MonoBehObj>
    where MonoBehObj : MonoBehaviour, IPoolable<InitModel>
{
    private readonly DiContainer _container;
    private readonly Queue<MonoBehObj> _pool = new();

    public UIMBFactory(
        DiContainer container)
    {
        _container = container;
    }

    public MonoBehObj Create(InitModel itemModel, Transform parent = null)
    {
        MonoBehObj view;
        if (_pool.Count > 0)
        {
            view = _pool.Dequeue();
        }
        else
        {
            view = _container.Resolve<MonoBehObj>();
        }
        view.gameObject.SetActive(true);
        if (parent)
        {
            view.transform.SetParent(parent);
            if (view.TryGetComponent<RectTransform>(out var rect))
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;   // Left, Bottom
                rect.offsetMax = Vector2.zero;   // Right, Top
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.localScale = Vector3.one;
                rect.anchoredPosition = Vector2.zero;/**/
            }
        }
        view.OnSpawned(itemModel);
        return view;
    }

    public void Despawn(MonoBehObj view)
    {
        view.gameObject.SetActive(false);
        view.OnDespawned();
        _pool.Enqueue(view);
    }
}