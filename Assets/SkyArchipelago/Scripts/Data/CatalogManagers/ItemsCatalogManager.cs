using System.Collections.Generic;
using UnityEngine;

public class ItemsCatalogManager :
    BaseCatalogManager<ItemsCatalogConfig, ItemConfig, EItemType>
{
    private Dictionary<EItemType, Queue<CollidersContainerMB>> _poolItems = new();

    public ItemsCatalogManager(
        ItemsCatalogConfig catalog) :
        base(catalog)
    {
    }

    public CollidersContainerMB SpawnAndSet(EItemType itemType, Transform parent)
    {
        CollidersContainerMB itemView = null;
        var pool = GetPool(itemType);
        if (pool.Count > 0)
        {
            itemView = pool.Dequeue();
            itemView.gameObject.SetActive(true);
            itemView.transform.SetParent(parent);
        }
        else if (TryGetConfigByKey(itemType, out var config))
        {
            itemView = GameObject.Instantiate(config.PrefabOfItem, parent);
        }
        itemView.transform.localPosition = Vector3.zero;
        itemView.transform.localRotation =
            Quaternion.Euler(0, Random.Range(0, 360), 0);
        return itemView;
    }

    public void Despawn(EItemType itemType, CollidersContainerMB view)
    {
        view.gameObject.SetActive(false);
        view.transform.SetParent(null);
        var pool = GetPool(itemType);
        pool.Enqueue(view);
    }

    private Queue<CollidersContainerMB> GetPool(EItemType itemType)
    {
        if (!_poolItems.ContainsKey(itemType))
        {
            _poolItems[itemType] = new();
        }
        return _poolItems[itemType];
    }
}