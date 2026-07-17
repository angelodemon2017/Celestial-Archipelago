using System;
using System.Collections.Generic;
using Zenject;

public class ItemModelFactory : IInitializable
{
    private readonly DiContainer _container;
    private readonly ItemsCatalogConfig _itemsCatalogConfig;
    private readonly ItemsCatalogManager _itemsCatalogManager;
    private readonly SimpleFactory<ItemConfig, ItemData> _itemDataFactory;

    private readonly Dictionary<EItemType, Queue<ItemModel>> _pools = new();
    private readonly Dictionary<EItemType, Type> _modelTypes = new();

    private readonly HashSet<EItemType> _simpleTypes = new();
    private readonly Queue<SimpleItem> _simpleItemsPool = new();

    [Inject]
    public ItemModelFactory(
        DiContainer container,
        ItemsCatalogConfig itemsCatalogConfig,
        ItemsCatalogManager itemsCatalogManager,
        SimpleFactory<ItemConfig, ItemData> itemDataFactory)
    {
        _container = container;
        _itemsCatalogConfig = itemsCatalogConfig;
        _itemsCatalogManager = itemsCatalogManager;
        _itemDataFactory = itemDataFactory;
    }

    public void Initialize()
    {
        // Маппинг типов
        _modelTypes[EItemType.None] = typeof(EmptyItemModel);
        _modelTypes[EItemType.Shovel] = typeof(ShovelModel);//for example
        var items = _itemsCatalogConfig.Elements;
        var count = items.Count;
        for (int i = 0; i < count; i++)
        {
            var item = items[i];
            if (!_modelTypes.ContainsKey(item.TypeItem))
                _simpleTypes.Add(item.TypeItem);
        }
    }

    public ItemData SplitItem(ItemData source, int amountToTake)
    {
        var newData = _itemDataFactory.Spawn(source.Config);
        newData.Copy(source);
        newData.Amount = amountToTake;
        source.Amount -= amountToTake;
        return newData;
    }

    public ItemModel SplitItem(ItemModel source, int amountToTake)
    {
        return Spawn(SplitItem(source._dataModel, amountToTake));
    }

    public ItemData GetDuplicate(ItemData itemData)
    {
        var duplItemData = _itemDataFactory.Spawn(itemData.Config);
        duplItemData.Copy(itemData);
        return duplItemData;
    }

    public ItemModel GetDuplicate(ItemModel itemModel)
    {
        return Spawn(GetDuplicate(itemModel._dataModel));
    }

    public ItemData GetEmptyItemData()
    {
        _itemsCatalogManager.TryGetConfigByKey(EItemType.None, out var empSlot);
        return _itemDataFactory.Spawn(empSlot);
    }

    public ItemModel GetEmptySlotModel()
    {
        return Spawn(GetEmptyItemData());
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

        model.OnDespawned();

        if (model is SimpleItem simpleItem &&
            _simpleTypes.Contains(itemType))
        {
            _simpleItemsPool.Enqueue(simpleItem);
            return;
        }

        if (!_pools.ContainsKey(itemType))
            _pools[itemType] = new Queue<ItemModel>();
        _pools[itemType].Enqueue(model);
    }

    public void ClearPool(EItemType type)
    {
        if (_pools.TryGetValue(type, out var pool))
            pool.Clear();
    }
}