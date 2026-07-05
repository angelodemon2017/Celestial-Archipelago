using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Containers/ItemsCatalog Config")]
public class ItemsCatalogConfig : BaseDataConfig
{
    [SerializeField] private List<ItemConfig> items = new ();

    private Dictionary<EItemType, ItemConfig> _cacheItems = new();

    public ItemConfig GetItemConfig(EItemType containerType)
    {
        if (!_cacheItems.ContainsKey(containerType))
        {
            _cacheItems.Clear();
            items.ForEach(c => _cacheItems.Add(c.TypeItem, c));
        }

        return _cacheItems[containerType];
    }
}