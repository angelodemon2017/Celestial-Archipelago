using System.Collections.Generic;

public class StateWithWindow<T> : BaseGameplayState
    where T : UIWindowBase
{
    private UIViewCoordinator _uIViewCoordinator;
    protected T _viewOfState;

    protected List<string> KeyHints = new();

    protected StateWithWindow(
        UIViewCoordinator uIViewCoordinator)
    {
        KeyHints.Add("F1 - this hint");
        _uIViewCoordinator = uIViewCoordinator;
    }

    public override void StateOn()
    {
        base.StateOn();
        _viewOfState = _uIViewCoordinator.ChangeWindow<T>(KeyHints);
    }

    public override void ProcessToggleKeyHints(bool lmb)
    {
        if(lmb)
            _uIViewCoordinator.ToggleHints();
    }
}