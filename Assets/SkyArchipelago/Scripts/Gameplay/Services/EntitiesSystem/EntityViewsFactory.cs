using System;
using System.Collections.Generic;
using Zenject;

public class EntityViewsFactory : IInitializable, IDisposable
{
    private readonly DiContainer _container;
    private readonly SignalBus _signalBus;
    private readonly EntitiesCatalogManager _entitiesCatalogManager;
    private readonly UIMBFactory<EntityRootHandlerMB, EntityViewMB> _entityViewFactory;

    private Dictionary<int, EntityViewMB> _activViews = new();
    private Dictionary<int, Queue<EntityRootHandlerMB>> _poolEntityViews = new();

    public EntityViewsFactory(
        DiContainer container,
        SignalBus signalBus,
        EntitiesCatalogManager entitiesCatalogManager,
        UIMBFactory<EntityRootHandlerMB, EntityViewMB> entityViewFactory)
    {
        _container = container;
        _signalBus = signalBus;
        _entitiesCatalogManager = entitiesCatalogManager;
        _entityViewFactory = entityViewFactory;
    }

    public EntityViewMB Spawn(EntityModel model)
    {
        if (model == null) return null;

        _entitiesCatalogManager.TryGetConfigByKey(model.EntType, out var confHand);
        if(!confHand.entityRootHandlerPrefab)
            return null;

        var idObj = confHand.modelConfig.Uid;

        EntityRootHandlerMB erh = SpawnEntityRootHandler(confHand);
        erh.OnSpawned(model);

        var view = _entityViewFactory.Create(erh);
        view.transform.position = model.Position;
        view.transform.rotation = model.Rotation;
        _activViews[model.Id] = view;

        return view;
    }

    public EntityRootHandlerMB SpawnEntityRootHandler(RootViewHandler pare)
    {
        if (_poolEntityViews.TryGetValue(pare.modelConfig.Uid, out var pool) && pool.Count > 0)
        {
            var erh = pool.Dequeue();
            erh.gameObject.SetActive(true);
            return erh;
        }
        else
            return _container.InstantiatePrefabForComponent<EntityRootHandlerMB>(pare.entityRootHandlerPrefab);
    }

    public void Despawn(EntityViewMB entityView)
    {
        if (entityView == null) return;

        _entitiesCatalogManager.TryGetConfigByKey(entityView.EntType, out var entityCon);
        var idObj = entityCon.modelConfig.Uid;

        Despawn(idObj, entityView.EntityRootHandler);
        _entityViewFactory.Despawn(entityView);
    }

    public void Despawn(int uid, EntityRootHandlerMB entityRootHandler)
    {
        entityRootHandler.transform.SetParent(null);
        entityRootHandler.gameObject.SetActive(false);
        if (!_poolEntityViews.ContainsKey(uid))
            _poolEntityViews[uid] = new Queue<EntityRootHandlerMB>();
        _poolEntityViews[uid].Enqueue(entityRootHandler);
    }

    private void OnHandle(EntityDeleteRequestSignal entityDeleteRequest)
    {
        if (_activViews.TryGetValue(entityDeleteRequest.IdEntity, out var entView))
        {
            Despawn(entView);
            _activViews.Remove(entityDeleteRequest.IdEntity);
        }
    }

    public void Initialize()
    {
        _signalBus.Subscribe<EntityDeleteRequestSignal>(OnHandle);
    }

    public void Dispose()
    {
        _signalBus?.Unsubscribe<EntityDeleteRequestSignal>(OnHandle);
    }
}