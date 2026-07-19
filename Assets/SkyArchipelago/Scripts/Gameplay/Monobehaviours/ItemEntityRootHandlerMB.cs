using UnityEngine;
using Zenject;

public class ItemEntityRootHandlerMB : EntityRootHandlerMB
{
    [SerializeField] private Transform _root;

    [Inject] private ItemsCatalogManager _itemsCatalogManager;

    private EItemType _itemType;
    private CollidersContainerMB _collidersContainerItemView;

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
        _collidersContainerItemView = _itemsCatalogManager.SpawnAndSet(itemType, _root);
        for (int i = 0; i < _collidersContainerItemView.GOsOfColliders.Count; i++)
            _interactableCoordinatorService.Register(_collidersContainerItemView.GOsOfColliders[i], _interactHandler);
    }

    public override void OnDespawned()
    {
        base.OnDespawned();
        if (_collidersContainerItemView)
        {
            for (int i = 0; i < _collidersContainerItemView.GOsOfColliders.Count; i++)
                _interactableCoordinatorService.Unregister(_collidersContainerItemView.GOsOfColliders[i]);
            _itemsCatalogManager.Despawn(_itemType, _collidersContainerItemView);
            _collidersContainerItemView = null;
        }
    }
}