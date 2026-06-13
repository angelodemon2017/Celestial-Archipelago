public class StateWithWindow<T> : BaseGameplayState
    where T : UIWindowBase
{
    private UIViewCoordinator _uIViewCoordinator;

    protected StateWithWindow(
        UIViewCoordinator uIViewCoordinator)
    {
        _uIViewCoordinator = uIViewCoordinator;
    }

    public override void StateOn()
    {
        base.StateOn();
        _uIViewCoordinator.ChangeWindow<T>();
    }
}