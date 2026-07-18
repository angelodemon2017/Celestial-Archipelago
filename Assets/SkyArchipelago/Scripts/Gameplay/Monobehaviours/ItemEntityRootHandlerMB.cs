using UnityEngine;
using Zenject;

public class ItemEntityRootHandlerMB : EntityRootHandlerMB
{
    [SerializeField] private Transform _root;

    [Inject] private ItemsCatalogManager _itemsCatalogManager;

    private EItemType _itemType;
    private GameObject _itemView;

    public override void Init(EntityModel model)
    {
        base.Init(model);
        if (model is DroppedItemModel droppedItem)
        {
            ShowModelOfItem(droppedItem.eItemType);
        }
    }

    private void ShowModelOfItem(EItemType itemType)
    {
        _itemType = itemType;
        _itemView = _itemsCatalogManager.SpawnAndSet(itemType, _root);
    }

    public override void OnDespawned()
    {
        base.OnDespawned();
        _itemsCatalogManager.Despawn(_itemType, _itemView);
        _itemView = null;
    }
}