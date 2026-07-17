using Zenject;

public class MenuOfEntityState : StateWithWindow<MenuOfEntityView>
{
    private readonly SignalBus _signalBus;
    private readonly EntityUIviewFactory _entityUIviewFactory;
    private readonly GameplayStateService _gameplayStateService;
    private readonly MenuOfEntityModel _menuOfEntity;
    private readonly RecipesGlossaryService _recipesGlossaryService;

    private BaseViewOfModelEntity _viewOfModelEntity;

    public MenuOfEntityState(
        SignalBus signalBus,
        EntityUIviewFactory entityUIviewFactory,
        GameplayStateService gameplayStateService,
        MenuOfEntityModel menuOfEntityModel,
        RecipesGlossaryService recipesGlossaryService,
        UIViewCoordinator uIViewCoordinator) :
        base(uIViewCoordinator)
    {
        _signalBus = signalBus;
        _entityUIviewFactory = entityUIviewFactory;
        _gameplayStateService = gameplayStateService;
        _menuOfEntity = menuOfEntityModel;
        _recipesGlossaryService = recipesGlossaryService;
    }

    public override void StateOn()
    {
        base.StateOn();
        _signalBus.Subscribe<EntitiesUpdatedSignal>(OnHandle);
        _viewOfModelEntity = _entityUIviewFactory.Spawn(_menuOfEntity.EntMod, _viewOfState.ParentOfView);
        _menuOfEntity.OnTryClosed += GoToMainGameplay;
    }

    public override void ProcessTryClose(bool isClosing)
    {
        if (isClosing)
            GoToMainGameplay();
    }

    public override void ProcessJump(bool jumpPressed)
    {
        if (jumpPressed)
            _viewOfModelEntity.MainAction();
    }

    private void GoToMainGameplay()
    {
        _gameplayStateService.SetState<MainFPSState>();
    }

    private void OnHandle(EntitiesUpdatedSignal entitiesUpdated)
    {
        if ((entitiesUpdated.Entity.AvailableTags & CtxFlag.UIHave) == CtxFlag.UIHave &&
            _menuOfEntity.EntMod.Id == entitiesUpdated.IdEntity)
            _viewOfModelEntity.UpdateView(entitiesUpdated.Entity);
    }

    public override void StateOff()
    {
        base.StateOff();
        _entityUIviewFactory.Despawn(_viewOfModelEntity);
        _recipesGlossaryService.UnFocuse();
        _signalBus.Unsubscribe<EntitiesUpdatedSignal>(OnHandle);
        _menuOfEntity.OnTryClosed -= GoToMainGameplay;
        _menuOfEntity.CleanByExitState();
    }
}