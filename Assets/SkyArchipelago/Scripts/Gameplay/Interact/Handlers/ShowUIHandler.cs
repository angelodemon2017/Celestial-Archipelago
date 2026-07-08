public class ShowUIHandler : BaseInteractHandler
{
    private readonly DialogModel _dialogModel;
    private readonly GameplayStateService _gameplayStateService;
    private readonly MenuEntityWithInventoryModel _menuEntityWithInventoryModel;

    public ShowUIHandler(
        DialogModel dialogModel,
        GameplayStateService gameplayStateService,
        MenuEntityWithInventoryModel menuEntityWithInventoryModel)
    {
        _dialogModel = dialogModel;
        _gameplayStateService = gameplayStateService;
        _menuEntityWithInventoryModel = menuEntityWithInventoryModel;
    }

    public override int Priority => 1;

    public override bool CanHandle(ItemModel item, EntityModel target)
    {
        return target.AvailableTags.HasFlag(CtxFlag.UIHave);
    }

    public override bool TryExecute(EntityModel source, ItemModel item, EntityModel target)
    {
        if (target is DemoNPCModel nPC)
        {
            _dialogModel.CurrentNpcId = nPC.NpcId;
            _gameplayStateService.SetState<DialogMenuState>();
        }

        if (target is IHaveContainer wcm &&
            source is IHaveContainer sourceCont)
        {
            _menuEntityWithInventoryModel.SetTargetEntity(wcm, sourceCont);
            _gameplayStateService.SetState<MenuOfEntityWithInventoryState>();
        }

        return true;
    }
}