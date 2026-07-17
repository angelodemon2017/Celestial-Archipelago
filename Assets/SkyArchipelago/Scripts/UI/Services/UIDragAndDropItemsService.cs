using Zenject;

public class UIDragAndDropItemsService
{
    private readonly SignalBus _signalBus;
    private readonly FPSCommonModel _fPSCommonModel;

    public UIDragAndDropItemsService(
        SignalBus signalBus,
        FPSCommonModel fPSCommonModel)
    {
        _signalBus = signalBus;
        _fPSCommonModel = fPSCommonModel;
    }

    public void ClickLMBBySlot(int slotId, ContainerModel containerModel)
    {
        var slotCont = containerModel.GetItemBySlot(slotId);
        var dragItem = _fPSCommonModel.ContainerDragUIOfPlayer.GetItemBySlot(0);
        if (slotCont.TypeItem == EItemType.None &&
            dragItem.TypeItem == EItemType.None)
            return;

        if (slotCont.TypeItem != EItemType.None &&
            dragItem.TypeItem == EItemType.None)
        {
            _signalBus.Fire(new MoveAmountBetweenSlotsSignal(
                containerModel.Id, _fPSCommonModel.ContainerDragUIOfPlayer.Id,
                slotId, 0, slotCont.Count));
            return;
        }

        if (slotCont.TypeItem != EItemType.None &&
            dragItem.TypeItem != slotCont.TypeItem)
        {
            _signalBus.Fire(new ExchangeItemContainersSignal(
                containerModel.Id,
                _fPSCommonModel.ContainerDragUIOfPlayer.Id,
                slotId, 0));
            return;
        }

        if (dragItem.TypeItem != EItemType.None)
        {
            _signalBus.Fire(new MoveAmountBetweenSlotsSignal(
                _fPSCommonModel.ContainerDragUIOfPlayer.Id,
                containerModel.Id, 0, slotId, dragItem.Count));
            return;
        }
    }

    public void ClickRMBBySlot(int slotId, ContainerModel containerModel)
    {
        var slotCont = containerModel.GetItemBySlot(slotId);
        var dragItem = _fPSCommonModel.ContainerDragUIOfPlayer.GetItemBySlot(0);
        if (slotCont.TypeItem == EItemType.None &&
            dragItem.TypeItem == EItemType.None)
            return;

        if (slotCont.TypeItem != EItemType.None &&
            dragItem.TypeItem == EItemType.None &&
            slotCont.Count > 1)
        {
            _signalBus.Fire(new MoveAmountBetweenSlotsSignal(
                containerModel.Id, _fPSCommonModel.ContainerDragUIOfPlayer.Id,
                slotId, 0, slotCont.Count / 2));
            return;
        }

        if ((slotCont.TypeItem == EItemType.None ||
            slotCont.TypeItem == dragItem.TypeItem) &&
            dragItem.TypeItem != EItemType.None)
            _signalBus.Fire(new MoveAmountBetweenSlotsSignal(
                _fPSCommonModel.ContainerDragUIOfPlayer.Id,
                containerModel.Id, 0, slotId, 1));
    }

    public void DragReturnToSelfContainer()
    {
        var item = _fPSCommonModel.ContainerDragUIOfPlayer.GetItemBySlot(0);
        if (item.TypeItem != EItemType.None)
        {
            _signalBus.Fire(new MoveItemBetweenContainersSignal(
                _fPSCommonModel.ContainerDragUIOfPlayer.Id,
                _fPSCommonModel.ContainerModel.Id,
                0));
        }
    }
}