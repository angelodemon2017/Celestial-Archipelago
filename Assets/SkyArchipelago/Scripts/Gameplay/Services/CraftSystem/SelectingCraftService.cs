using System;
using Zenject;

public class SelectingCraftService : IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly RecipeCatalogManager _recipeCatalogManager;
    private readonly EntityRuntimeService _entityRuntimeService;
    private readonly ContainersService _containersService;
    private readonly CraftProcessRepository _craftProcessRepository;
    private readonly CraftingOperationService _craftingOperationService;
    private readonly EntitiesCatalogManager _entitiesCatalogManager;

    public SelectingCraftService(
        SignalBus signalBus,
        ContainersService containersService,
        EntityRuntimeService entityRuntimeService,
        RecipeCatalogManager recipeCatalogManager,
        CraftProcessRepository craftProcessRepository,
        CraftingOperationService craftingOperationService,
        EntitiesCatalogManager entitiesCatalogManager)
    {
        _signalBus = signalBus;
        _containersService = containersService;
        _entityRuntimeService = entityRuntimeService;
        _recipeCatalogManager = recipeCatalogManager;
        _craftProcessRepository = craftProcessRepository;
        _craftingOperationService = craftingOperationService;
        _entitiesCatalogManager = entitiesCatalogManager;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<SelectRecipe>(OnHandle);
        _signalBus.Subscribe<HandleCraft>(TakeCraftByEntity);
        _signalBus.Subscribe<ContainerOfEntityRequest>(OnHandle);
    }

    public void Dispose()
    {
        _signalBus?.Unsubscribe<SelectRecipe>(OnHandle);
        _signalBus?.Unsubscribe<HandleCraft>(TakeCraftByEntity);
        _signalBus?.Unsubscribe<ContainerOfEntityRequest>(OnHandle);
    }

    private void OnHandle(SelectRecipe selectRecipe)
    {
        if (!(_entityRuntimeService.TryGetEntityById(selectRecipe.TargetEntityId, out var target) &&
            target is ICraftable craftable))
            return;

        TryUnselectCraftProcess(craftable);

        if (!_entityRuntimeService.TryGetEntityById(selectRecipe.ReasonEntityId, out var reasonEntity))
            return;

        if (!_recipeCatalogManager.TryGetConfigByKey(selectRecipe.RecipeID, out var recipe))
            return;

        var craft = _craftProcessRepository.StartCraft(reasonEntity, craftable, recipe);
        craftable.CraftIdProcess = craft.Id;
    }

    private void OnHandle(ContainerOfEntityRequest containerOfEntityRequest)
    {
        if (_entityRuntimeService.TryGetEntityById(containerOfEntityRequest.IdEntity, out var entity))
        {
            if (entity is IHaveContainer haveContainer &&
                entity is ICraftable craftable)
                TryAutoSelectingCraft(craftable, haveContainer);
        }
    }

    public void TryAutoSelectingCraft(ICraftable craftable, IHaveContainer entitySourceContainer)
    {
        if (!(_entitiesCatalogManager.TryGetModule(craftable.ConfigId, CtxFlag.HaveRecipe, out var module) &&
            module is RecipesModuleConfig recipesModuleConfig))
            return;

        TryUnselectCraftProcess(craftable);

        var inputContainer = _containersService.GetContainerModel(entitySourceContainer, EContainerType.SourceInput);
        for (int i = 0; i < recipesModuleConfig.AvailableRecipes.Count; i++)
        {
            var recipe = recipesModuleConfig.AvailableRecipes[i];
            if (!recipe.IsStaticRecipe &&
                _craftingOperationService.GetAvailableProductionsByRecipe(recipe, inputContainer._dataModel) > 0)
            {
                var craft = _craftProcessRepository.StartCraft(craftable, craftable, recipe);
                craftable.CraftIdProcess = craft.Id;
                return;
            }
        }
    }

    /// <summary>
    /// thinking about place for method
    /// </summary>
    private void TakeCraftByEntity(HandleCraft handleCraft)
    {
        if (!(_entityRuntimeService.TryGetEntityById(handleCraft.TargetEntityId, out var target) &&
            target is ICraftable craftable && craftable.IsActive && craftable.CraftIdProcess >= 0))
            return;

        if (!_craftProcessRepository.TryGetCraftById(craftable.CraftIdProcess, out var craft))
            return;

        _craftingOperationService.TakeUA(craft, 100);
    }

    public void TryUnselectCraftProcess(ICraftable craftable)
    {
        if (craftable.CraftIdProcess < 0)
            return;

        _craftProcessRepository.RemoveCraft(craftable.CraftIdProcess);
        craftable.CraftIdProcess = -1;
    }
}