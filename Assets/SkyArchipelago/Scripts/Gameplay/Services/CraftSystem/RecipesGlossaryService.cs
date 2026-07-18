using System;
using System.Collections.Generic;
using Zenject;

public class RecipesGlossaryService : IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly ContainersService _containersService;
    private readonly EntitiesCatalogManager _entitiesCatalogManager;
    private readonly CraftProcessRepository _craftProcessRepository;
    private readonly CraftingOperationService _craftingOperationService;
    private readonly RecipeGlossaryRepository _recipeGlossaryRepository;

    private Dictionary<int, RecipeStateOfWorld> _cacheRecipies = new();
    private Dictionary<EItemType, ItemAvailable> _cacheItemsAvailabl = new();

    private ICraftable _targetEntity = null;
    private ContainerModel _focusContainer = null;

    public int CurrentIdContainer => _focusContainer?.Id ?? -1;

    public RecipesGlossaryService(
        SignalBus signalBus,
        ContainersService containersService,
        EntitiesCatalogManager entitiesCatalogManager,
        CraftProcessRepository craftProcessRepository,
        CraftingOperationService craftingOperationService,
        RecipeGlossaryRepository recipeGlossaryRepository)
    {
        _signalBus = signalBus;
        _containersService = containersService;
        _entitiesCatalogManager = entitiesCatalogManager;
        _craftProcessRepository = craftProcessRepository;
        _craftingOperationService = craftingOperationService;
        _recipeGlossaryRepository = recipeGlossaryRepository;
    }

    public void SetFocusContainer(EntityModel entityModel, IHaveContainer entityWithContainer)
    {
        if (entityModel is ICraftable craftable)
            _targetEntity = craftable;
        _focusContainer = _containersService.GetContainerModel(entityWithContainer);
        UpdateCurrentRecipes();
    }

    private void SelectRecipe(SelectRecipe recipe)
    {
        UpdateCurrentRecipes();
    }

    public void UpdateCurrentRecipes()
    {
        if (_targetEntity == null)
            return;

        if (!(_entitiesCatalogManager.TryGetModule(_targetEntity.EntType, CtxFlag.HaveRecipe, out var module) &&
            module is RecipesModuleConfig recipesModule))
            return;

        _recipeGlossaryRepository.CurrentRecipes.Clear();
        for (int i = 0; i < recipesModule.AvailableRecipes.Count; i++)
        {
            var recipe = recipesModule.AvailableRecipes[i];
            if (!_cacheRecipies.ContainsKey(recipe.KeyOfCatalog))
                _cacheRecipies[recipe.KeyOfCatalog] = new RecipeStateOfWorld(recipe);
            var stateRecipe = _cacheRecipies[recipe.KeyOfCatalog];
            RecalcRecipe(stateRecipe);
            _recipeGlossaryRepository.CurrentRecipes.Add(stateRecipe);
        }
    }

    private void RecalcRecipe(RecipeStateOfWorld recipeState)
    {
        if (_focusContainer == null)
            return;

        recipeState.CountAvailable = _craftingOperationService.GetAvailableProductionByRecipe(recipeState.RecipeConfig, _focusContainer._dataModel);
        UpdateCurrentCosts();
    }

    private void UpdateCurrentCosts()
    {
        if (!_craftProcessRepository.TryGetCraftById(_targetEntity.CraftIdProcess, out var craft))
            return;

        if (!_cacheRecipies.TryGetValue(craft.ConfigModel.Uid, out var rsw))
            return;

        var recipe = rsw.RecipeConfig;
        _recipeGlossaryRepository.CurrentStateItems.Clear();
        for (int i = 0; i < recipe._inputs.Count; i++)
        {
            var input = recipe._inputs[i];
            if (!_cacheItemsAvailabl.ContainsKey(input.Config.TypeItem))
                _cacheItemsAvailabl[input.Config.TypeItem] = new ItemAvailable(input.Config);
            var ia = _cacheItemsAvailabl[input.Config.TypeItem];
            ia.NeedCounts = input.Amount;
            ia.AvailableCount = _focusContainer._dataModel.GetItemCountByType(input.Config.TypeItem);
            _recipeGlossaryRepository.CurrentStateItems.Add(ia);
        }
    }

    public void UnFocuse()
    {
        _targetEntity = null;
        _focusContainer = null;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<SelectRecipe>(SelectRecipe);
    }

    public void Dispose()
    {
        _signalBus?.Unsubscribe<SelectRecipe>(SelectRecipe);
    }
}