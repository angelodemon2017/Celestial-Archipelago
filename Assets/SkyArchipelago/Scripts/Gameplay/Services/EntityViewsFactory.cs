using System.Collections.Generic;
using Zenject;

public class EntityViewsFactory
{
    private readonly DiContainer _container;
    private readonly EntityViewCatalog _entityViewCatalog;
    private readonly UIMBFactory<EntityRootHandlerMB, EntityViewMB> _entityViewFactory;

    private Dictionary<int, Queue<EntityRootHandlerMB>> _poolEntityViews = new();

    public EntityViewsFactory(
        DiContainer container,
        EntityViewCatalog entityViewCatalog,
        UIMBFactory<EntityRootHandlerMB, EntityViewMB> entityViewFactory)
    {
        _container = container;
        _entityViewCatalog = entityViewCatalog;
        _entityViewFactory = entityViewFactory;
    }

    public EntityViewMB Spawn(EntityModel model)
    {
        if (model == null) return null;

        var confHand = _entityViewCatalog.GetModelConfigByType(model.EntType);
        var idObj = confHand.modelConfig.Uid;

        EntityRootHandlerMB erh;
        if (_poolEntityViews.TryGetValue(idObj, out var pool) && pool.Count > 0)
        {
            erh = pool.Dequeue();
        }
        else
        {
            erh = _container.InstantiatePrefabForComponent<EntityRootHandlerMB>(confHand.entityRootHandlerPrefab);
        }
        erh.OnSpawned(model);

        var view = _entityViewFactory.Create(erh);
        view.transform.position = model.Position;
        view.transform.rotation = model.Rotation;

        return view;
    }

    public void Despawn(EntityViewMB entityView)
    {
        if (entityView == null) return;

        var idObj = _entityViewCatalog.GetModelConfigByType(entityView.EntType).modelConfig.Uid;

        if (!_poolEntityViews.ContainsKey(idObj))
            _poolEntityViews[idObj] = new Queue<EntityRootHandlerMB>();

        _poolEntityViews[idObj].Enqueue(entityView.EntityRootHandler);
        _entityViewFactory.Despawn(entityView);
    }
}