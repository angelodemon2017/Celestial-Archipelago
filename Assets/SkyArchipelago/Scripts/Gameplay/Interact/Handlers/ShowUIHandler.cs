public class ShowUIHandler : BaseInteractHandler
{
    private readonly DialogModel _dialogModel;
    private readonly GameplayStateService _gameplayStateService;

    public ShowUIHandler(
        DialogModel dialogModel,
        GameplayStateService gameplayStateService)
    {
        _dialogModel = dialogModel;
        _gameplayStateService = gameplayStateService;
    }

    public override int Priority => 1;

    public override bool CanHandle(ItemModel item, EntityModel target)
    {
        if (target is IUIshowable uIshowable)
            return uIshowable.UIAvailable;

        return false;
    }

    public override bool TryExecute(EntityModel source, ItemModel item, EntityModel target)
    {
        if (!(target is DemoNPCModel nPC))
            return false;

        _dialogModel.CurrentNpcId = nPC.NpcId;
        _gameplayStateService.SetState<DialogMenuState>();

        return true;
    }
}