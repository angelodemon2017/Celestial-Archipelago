using System;
using Zenject;

public class InventoryTransactionsService : IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly ContainersService _containersService;
    private readonly ContainerOperationsService _containerOperationsService;
    private readonly ItemsCatalogManager _itemsCatalogManager;
    private readonly SimpleFactory<ItemConfig, ItemData> _itemDataFactory;
    private readonly ItemModelFactory _itemModelFactory;

    public InventoryTransactionsService(
        SignalBus signalBus,
        ItemsCatalogManager itemsCatalogManager,
        SimpleFactory<ItemConfig, ItemData> itemDataFactory,
        ItemModelFactory itemModelFactory,
        ContainersService containersService,
        ContainerOperationsService containerOperationsService)
    {
        _signalBus = signalBus;
        _containersService = containersService;
        _itemsCatalogManager = itemsCatalogManager;
        _itemDataFactory = itemDataFactory;
        _itemModelFactory = itemModelFactory;
        _containerOperationsService = containerOperationsService;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<MoveItemBetweenContainersSignal>(OnHandle);
        _signalBus.Subscribe<ExchangeItemContainersSignal>(OnHandle);
        _signalBus.Subscribe<MoveAmountBetweenSlotsSignal>(OnHandle);
    }

    public bool TryAddItem(ContainerModel containerModel, EItemType itemType, int count, ContainerAvailabilityFlag reason)
    {
        if (!(_itemsCatalogManager.TryGetConfigByKey(itemType, out var itemConfig)))
            return false;

        var itemData = _itemDataFactory.Spawn(itemConfig);
        itemData.Amount = count;
        var itemModel = _itemModelFactory.Spawn(itemData);
        var result = TryAddItemModel(containerModel, itemModel, reason);
        _itemDataFactory.Despawn(itemData);
        _itemModelFactory.Despawn(itemModel);
        return result;
    }

/*    public bool TryPickItemToContainer(ContainerModel containerModel, ItemModel itemModel)
    {
        return TryAddItemModel(containerModel, itemModel, ContainerAvailabilityFlag.CanHandleDrop);
    }/**/

    public bool TryAddItemModel(ContainerModel containerModel, ItemModel itemModel, ContainerAvailabilityFlag reason)
    {
        if (itemModel == null || itemModel.TypeItem == EItemType.None)
            return false;

        if ((reason & containerModel.Availabilities & ContainerAvailabilityFlag.DropActions) == ContainerAvailabilityFlag.None)
            return false;

        var changes = _containerOperationsService.TryAddItemToContainer(containerModel._dataModel, itemModel._dataModel);
        if (changes.Count > 0)
            containerModel._dataModel.UpdateCountsByType(itemModel.TypeItem);
        return containerModel.RefreshModelsAfterDataChange(changes);
    }

    public bool TryRemoveItem(ContainerModel containerModel, ItemConfig itemConf, int amount)
    {
        if (itemConf == null || itemConf.TypeItem == EItemType.None)
            return false;

        var changes = _containerOperationsService.TryRemoveFromContainer(containerModel._dataModel, itemConf, amount);
        if (changes.Count > 0)
            containerModel._dataModel.UpdateCountsByType(itemConf.TypeItem);

        for (int i = 0; i < changes.Count; i++)
        {
            var slotId = changes[i];
            if (containerModel._dataModel.itemDatas[slotId].Amount <= 0)
                containerModel.ClearSlot(slotId);
        }
        return containerModel.RefreshModelsAfterDataChange(changes);
    }

    private void OnHandle(MoveItemBetweenContainersSignal moveItemBetweenContainers)
    {
        var contFrom = _containersService.GetContainerModelById(moveItemBetweenContainers.ContainerIdFrom);
        var contTo = _containersService.GetContainerModelById(moveItemBetweenContainers.ContainerIdTo);
        var itemModel = contFrom.GetItemBySlot(moveItemBetweenContainers.FromIdSlot);

        MoveItem(contFrom, contTo, itemModel, ContainerAvailabilityFlag.HandleActions);
    }

    /// <summary>
    /// move by one way
    /// </summary>
    /// <param name="fromContainer"></param>
    /// <param name="toContainer"></param>
    /// <param name="itemModel"></param>
    public void MoveItem(ContainerModel fromContainer, ContainerModel toContainer, ItemModel itemModel, ContainerAvailabilityFlag reason)
    {
        if ((reason & fromContainer.Availabilities & toContainer.Availabilities) != reason)
            return;

        var fromSlot = itemModel.SlotId;
        var wasChanged = TryAddItemModel(toContainer, itemModel, reason);

        if (wasChanged)
        {
            var typeForUpdate = itemModel.TypeItem;
            if (itemModel.Count == 0)
            {
                fromContainer.ClearSlot(fromSlot);
            }
            else
            {
                fromContainer.RefreshModelBySlot(fromSlot);
            }
            fromContainer._dataModel.UpdateCountsByType(typeForUpdate);
            RecalcAndPushNotification(fromContainer);
            if (fromContainer.Id != toContainer.Id)
                RecalcAndPushNotification(toContainer);
        }
    }

    private void OnHandle(ExchangeItemContainersSignal signal)
    {
        var fromCont = _containersService.GetContainerModelById(signal.ContainerIdFrom);
        var toCont = _containersService.GetContainerModelById(signal.ContainerIdTo);
        ExchangeItems(fromCont, toCont, signal.FromIdSlot, signal.ToIdSlot, ContainerAvailabilityFlag.HandleActions);
    }

    public void ExchangeItems(ContainerModel fromContainer, ContainerModel toContainer,
        int idSlotFrom, int idSlotTo, ContainerAvailabilityFlag reason)
    {
        if ((reason & fromContainer.Availabilities & toContainer.Availabilities) != reason)
            return;

        var itemFrom = fromContainer.GetItemBySlot(idSlotFrom);
        var itemToForCheck = toContainer.GetItemBySlot(idSlotTo);

        if (fromContainer.ConfigModel.CustomStackSize != 0 && itemToForCheck.Count > fromContainer.ConfigModel.CustomStackSize ||
            toContainer.ConfigModel.CustomStackSize != 0 && itemFrom.Count > toContainer.ConfigModel.CustomStackSize)
            return;

        var itemTo = toContainer.ExchangeItemModel(idSlotTo, itemFrom);
        fromContainer.ExchangeItemModel(idSlotFrom, itemTo);

        RecalcAndPushNotification(fromContainer);
        RecalcAndPushNotification(toContainer);
    }

    private void RecalcAndPushNotification(ContainerModel container)
    {
        _signalBus.Fire(new ContainerOfEntityRequest(container.IdEntityOwner, container.Id));
    }

    public void OnHandle(MoveAmountBetweenSlotsSignal signal)
    {
        var fromCont = _containersService.GetContainerModelById(signal.ContainerIdFrom);
        var toCont = _containersService.GetContainerModelById(signal.ContainerIdTo);

        TryMoveAmount(fromCont, signal.FromSlotId, toCont, signal.ToSlotId, signal.Amount, ContainerAvailabilityFlag.HandleActions);
    }

    public bool TryMoveAmount(
        ContainerModel fromContainer,
        int fromSlotId,
        ContainerModel toContainer,
        int toSlotId,
        int amount,
        ContainerAvailabilityFlag reason)
    {

        if (fromContainer == null || toContainer == null || amount <= 0)
            return false;

        if (!((reason & toContainer.Availabilities & ContainerAvailabilityFlag.CanHandleDrop) == ContainerAvailabilityFlag.CanHandleDrop &&
            (reason & fromContainer.Availabilities & ContainerAvailabilityFlag.CanHandleDrag) == ContainerAvailabilityFlag.CanHandleDrag))
            return false;

        var itemInSlot = fromContainer.GetItemBySlot(fromSlotId);
        if (itemInSlot == null || itemInSlot.TypeItem == EItemType.None || itemInSlot.Count < amount)
            return false;

        var wasChanges = _containerOperationsService.TryMoveAmountFromSlotToSlot(
            fromContainer._dataModel,
            fromSlotId,
            toContainer._dataModel,
            toSlotId,
            amount);

        if (!wasChanges)
            return false;

        // Обновляем счётчики по типу
        var type = itemInSlot.TypeItem;
        fromContainer._dataModel.UpdateCountsByType(type);
        toContainer._dataModel.UpdateCountsByType(type);

        // Очищаем пустые слоты в источнике
        if (fromContainer._dataModel.itemDatas[fromSlotId].Amount <= 0)
            fromContainer.ClearSlot(fromSlotId);
        else
            fromContainer.RefreshModelBySlot(fromSlotId);
        toContainer.RefreshModelBySlot(toSlotId);

        if (wasChanges)
        {
            RecalcAndPushNotification(fromContainer);
            if (fromContainer.Id != toContainer.Id)
                RecalcAndPushNotification(toContainer);
        }

        return wasChanges;
    }

    public void Dispose()
    {
        _signalBus?.Unsubscribe<MoveItemBetweenContainersSignal>(OnHandle);
        _signalBus?.Unsubscribe<ExchangeItemContainersSignal>(OnHandle);
        _signalBus?.Unsubscribe<MoveAmountBetweenSlotsSignal>(OnHandle);
    }
}