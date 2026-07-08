using System;
using Zenject;

public class MenuEntityWithInventoryModel
{
    private readonly SignalBus _signalBus;
    private readonly ContainersService _containersService;

    public IHaveContainer TargetModel;
    public ContainerModel ContainerModelOfEntity;
    public ContainerModel ContainerModelOfPlayer;

    public Action OnTryClosed;

    public MenuEntityWithInventoryModel(
        SignalBus signalBus,
        ContainersService containersService)
    {
        _signalBus = signalBus;
        _containersService = containersService;
    }

    public void SetTargetEntity<T>(T targetModel, T firstModel)
        where T : IHaveContainer
    {
        TargetModel = targetModel;
        ContainerModelOfEntity = _containersService.GetContainerModel(targetModel);
        ContainerModelOfPlayer = _containersService.GetContainerModel(firstModel);

        ContainerModelOfEntity.TestActionBySlot += MoveItem;
        ContainerModelOfPlayer.TestActionBySlot += MoveItem;
    }

    public void MoveItem(int idContainer, byte idSlot)
    {
        int idTo = idContainer == ContainerModelOfEntity.Id ?
            ContainerModelOfPlayer.Id :
            ContainerModelOfEntity.Id;

        _signalBus.Fire(new MoveItemBetweenContainersSignal(idContainer, idTo, idSlot));
    }

    public void CleanByExitState()
    {
        ContainerModelOfEntity.TestActionBySlot -= MoveItem;
        ContainerModelOfPlayer.TestActionBySlot -= MoveItem;
    }
}