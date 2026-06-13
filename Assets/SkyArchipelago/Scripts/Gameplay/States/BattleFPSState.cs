public class BattleFPSState : StateWithWindow<GameplayControllerView>
{
    public override bool CursorIsAvailable => false;

    public BattleFPSState(
        UIViewCoordinator uIViewCoordinator) :
        base(uIViewCoordinator)
    {

    }
}