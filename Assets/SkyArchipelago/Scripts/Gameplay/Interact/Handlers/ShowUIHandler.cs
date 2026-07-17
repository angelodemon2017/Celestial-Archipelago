public class ShowUIHandler : BaseInteractHandler
{
    private readonly DialogModel _dialogModel;
    private readonly GameplayStateService _gameplayStateService;
    private readonly MenuOfEntityModel _menuOfEntityModel;
    private readonly RecipesGlossaryService _recipesGlossaryService;

    public ShowUIHandler(
        DialogModel dialogModel,
        GameplayStateService gameplayStateService,
        MenuOfEntityModel menuOfEntityModel,
        RecipesGlossaryService recipesGlossaryService)
    {
        _dialogModel = dialogModel;
        _gameplayStateService = gameplayStateService;
        _menuOfEntityModel = menuOfEntityModel;
        _recipesGlossaryService = recipesGlossaryService;
    }

    public override int Priority => 1;

    public override bool CanHandle(ItemModel item, EntityModel target)
    {
        return (target.AvailableTags & CtxFlag.UIHave) == CtxFlag.UIHave;
    }

    public override bool TryExecute(EntityModel source, ItemModel item, EntityModel target)
    {
        if (target is DemoNPCModel nPC)
        {
            _dialogModel.CurrentNpcId = nPC.NpcId;
            _gameplayStateService.SetState<DialogMenuState>();
            return true;
        }
        //TODO Обобщить target!

        if (target is IUIshowable)
        {
            if ((target.AvailableTags & CtxFlag.HaveRecipe) == CtxFlag.HaveRecipe)
            {
                if(target is IHaveContainer haveContainer)
                    _recipesGlossaryService.SetFocusContainer(target, haveContainer);
                else if(source is IHaveContainer sourceContainer)
                    _recipesGlossaryService.SetFocusContainer(target, sourceContainer);
            }
            _menuOfEntityModel.SetEM(target);

            _gameplayStateService.SetState<MenuOfEntityState>();
            return true;
        }

        return false;
    }
}