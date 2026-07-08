using System;
using Zenject;

public class InventoryTransactionsService : IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly DataService _dataService;
    private readonly ItemModelFactory _itemModelFactory;
    private readonly ContainersService _containersService;

    public InventoryTransactionsService(
        SignalBus signalBus,
        ItemModelFactory itemModelFactory,
        ContainersService containersService,
        DataService dataService)
    {
        _signalBus = signalBus;
        _itemModelFactory = itemModelFactory;
        _containersService = containersService;
        _dataService = dataService;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<MoveItemBetweenContainersSignal>(OnHandle);
    }

    public bool TryPickItemToContainer(ContainerModel containerModel, ItemModel itemModel)
    {
        return containerModel.TryAddItemModel(itemModel);
    }

    private void OnHandle(MoveItemBetweenContainersSignal moveItemBetweenContainers)
    {
        var contFrom = _containersService.GetContainerModelById(moveItemBetweenContainers.ContainerIdFrom);
        var contTo = _containersService.GetContainerModelById(moveItemBetweenContainers.ContainerIdTo);
        var itemModel = contFrom.GetItemModel(moveItemBetweenContainers.FromIdSlot);

        MoveItem(contFrom, contTo, itemModel);
    }

    /// <summary>
    /// move by one way
    /// </summary>
    /// <param name="fromContainer"></param>
    /// <param name="toContainer"></param>
    /// <param name="itemModel"></param>
    public void MoveItem(ContainerModel fromContainer, ContainerModel toContainer, ItemModel itemModel)
    {
        var fromSlot = itemModel.SlotId;
        var result = toContainer.TryAddItemModel(itemModel);

        if (result)
        {
            if (itemModel.Count == 0)
            {
                fromContainer.ClearSlot(fromSlot);
            }
            else
            {
                itemModel.SetCount(itemModel.Count);
            }
            fromContainer.Changed?.Invoke();
        }
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<MoveItemBetweenContainersSignal>(OnHandle);
    }
}