public class InventoryMenuState : StateWithWindow<InventoryView>
{
    public InventoryMenuState(
        UIViewCoordinator uIViewCoordinator) :
        base(uIViewCoordinator)
    {

    }

    public override void StateOn()
    {
        base.StateOn();

//        _inventoryModel.GetMyContainer = //place of local player container
    }
}