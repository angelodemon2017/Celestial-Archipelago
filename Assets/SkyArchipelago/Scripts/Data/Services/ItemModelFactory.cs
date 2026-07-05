using System;
using System.Collections.Generic;
using Zenject;

public class ItemModelFactory : IInitializable
{
    private readonly DiContainer _container;
    private readonly Dictionary<EItemType, Queue<ItemModel>> _pools = new();
    private readonly Dictionary<EItemType, Type> _modelTypes = new();

    private readonly HashSet<EItemType> _simpleTypes = new();
    private readonly Queue<SimpleItem> _simpleItemsPool = new();

    [Inject]
    public ItemModelFactory(DiContainer container)
    {
        _container = container;
    }

    public void Initialize()
    {
        // Маппинг типов
        _modelTypes[EItemType.None] = typeof(EmptyItemModel);
        _modelTypes[EItemType.Coal] = typeof(CoalModel);
        _modelTypes[EItemType.Shovel] = typeof(ShovelModel);
        _simpleTypes.Add(EItemType.Rock);
        _simpleTypes.Add(EItemType.Wood);
        // Добавляй остальные типы здесь
    }

    public ItemModel Spawn(ItemData data)
    {
        if (data == null) return null;

        var itemType = data.TypeItem;

        if (_simpleTypes.Contains(itemType))
        {
            if (_simpleItemsPool.Count > 0)
            {
                var model = _simpleItemsPool.Dequeue();
                model.OnSpawned(data);
                return model;
            }
            var newModelItem = _container.Instantiate<SimpleItem>();
            newModelItem.OnSpawned(data);
            return newModelItem;
        }
        else if (_pools.TryGetValue(itemType, out var pool) && pool.Count > 0)
        {
            var model = pool.Dequeue();
            model.OnSpawned(data);
            return model;
        }

        // Создаём новый
        var modelType = _modelTypes.GetValueOrDefault(itemType) ?? typeof(ItemModel);
        var newModel = (ItemModel)_container.Instantiate(modelType);
        newModel.OnSpawned(data);
        return newModel;
    }

    public void Despawn(ItemModel model)
    {
        if (model == null) return;

        var itemType = model.TypeItem;

        if (model is SimpleItem simpleItem &&
            _simpleTypes.Contains(itemType))
        {
            simpleItem.OnDespawned();
            _simpleItemsPool.Enqueue(simpleItem);
            return;
        }

        if (!_pools.ContainsKey(itemType))
            _pools[itemType] = new Queue<ItemModel>();

        model.OnDespawned();
        _pools[itemType].Enqueue(model);
    }

    public void ClearPool(EItemType type)
    {
        if (_pools.TryGetValue(type, out var pool))
            pool.Clear();
    }
}