public class DialogMenuState : StateWithWindow<DialogMenuUI>
{
    private readonly GameplayStateService _gameplayStateService;
    private readonly DialogModel _dialogModel;

    public DialogMenuState(
        GameplayStateService gameplayStateService,
        DialogModel dialogModel,
        UIViewCoordinator uIViewCoordinator) :
        base(uIViewCoordinator)
    {
        KeyHints.Add("Q - Close this menu");
        _gameplayStateService = gameplayStateService;
        _dialogModel = dialogModel;
    }

    public override void StateOn()
    {
        base.StateOn();
        _dialogModel.OnTryClosed += GoToMainGameplay;
    }

    public override void ProcessTryClose(bool isClosing)
    {
        if (isClosing)
            GoToMainGameplay();
    }

    private void GoToMainGameplay()
    {
        _gameplayStateService.SetState<MainFPSState>();
    }

    public override void StateOff()
    {
        base.StateOff();
        _dialogModel.OnTryClosed -= GoToMainGameplay;
    }
}