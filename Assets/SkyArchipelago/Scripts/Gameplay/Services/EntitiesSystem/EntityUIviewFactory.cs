using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EntityUIviewFactory
{
    private readonly DiContainer _container;
    private readonly EntitiesCatalogManager _entitiesCatalogManager;

    private Dictionary<int, BaseViewOfModelEntity> _poolViewByEntityUid = new();

    public EntityUIviewFactory(
        DiContainer container,
        EntitiesCatalogManager entitiesCatalogManager)
    {
        _container = container;
        _entitiesCatalogManager = entitiesCatalogManager;
    }

    public BaseViewOfModelEntity Spawn(EntityModel entity, Transform parent = null)
    {
        BaseViewOfModelEntity view = null;
        if (_poolViewByEntityUid.TryGetValue(entity.ConfigId, out var result))
        {
            view = result;
        }
        else
        {
            if (_entitiesCatalogManager.TryGetModule(entity.ConfigId, CtxFlag.UIHave, out ModuleConfig module) &&
                module is UIModuleConfig uiModule)
                view = _container.InstantiatePrefabForComponent<BaseViewOfModelEntity>(uiModule.View);
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
        //view.OnSpawned(itemModel);//method for only Init ?
        view.UpdateView(entity);
        return view;
    }

    public void Despawn(BaseViewOfModelEntity view)
    {
        view.OnDespawned();
        view.gameObject.SetActive(false);
        view.transform.SetParent(null);
    }
}