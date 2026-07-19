using System;
using System.Collections.Generic;
using Zenject;

public class RecipesGlossaryService : IInitializable, IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly FPSCommonModel _fpsCommonModel;
    private readonly ContainersService _containersService;
    private readonly EntitiesCatalogManager _entitiesCatalogManager;
    private readonly CraftProcessRepository _craftProcessRepository;
    private readonly CraftingOperationService _craftingOperationService;
    private readonly RecipeGlossaryRepository _recipeGlossaryRepository;
    private readonly EntityRecipeCatalogManager _entityRecipeCatalogManager;

    private Dictionary<int, RecipeStateOfWorld> _cacheItemRecipies = new();
    private Dictionary<int, RecipeStateOfWorld> _cacheEntityRecipies = new();
    private Dictionary<EItemType, ItemAvailable> _cacheItemsAvailabl = new();

    private ICraftable _targetEntity = null;
    private ContainerModel _focusContainer = null;

    public int CurrentIdContainer => _focusContainer?.Id ?? -1;

    public RecipesGlossaryService(
        SignalBus signalBus,
        FPSCommonModel fpsCommonModel,
        ContainersService containersService,
        EntitiesCatalogManager entitiesCatalogManager,
        CraftProcessRepository craftProcessRepository,
        CraftingOperationService craftingOperationService,
        RecipeGlossaryRepository recipeGlossaryRepository,
        EntityRecipeCatalogManager entityRecipeCatalogManager)
    {
        _signalBus = signalBus;
        _fpsCommonModel = fpsCommonModel;
        _containersService = containersService;
        _entitiesCatalogManager = entitiesCatalogManager;
        _craftProcessRepository = craftProcessRepository;
        _craftingOperationService = craftingOperationService;
        _recipeGlossaryRepository = recipeGlossaryRepository;
        _entityRecipeCatalogManager = entityRecipeCatalogManager;
    }

    public void SetFocusContainer(EntityModel entityModel, IHaveContainer entityWithContainer)
    {
        if (entityModel is ICraftable craftable)
            _targetEntity = craftable;
        _focusContainer = _containersService.GetContainerModel(entityWithContainer);
        UpdateCurrentItemRecipes();
    }

    private void SelectRecipe(SelectRecipe recipe)
    {
        UpdateCurrentItemRecipes();
    }

    public void UpdateCurrentItemRecipes()
    {
        if (_targetEntity == null)
            return;

        if (!(_entitiesCatalogManager.TryGetModule(_targetEntity.EntType, CtxFlag.HaveRecipe, out var module) &&
            module is RecipesModuleConfig recipesModule))
            return;

        UpdateModelRecipes(_cacheItemRecipies, recipesModule.AvailableRecipes, _focusContainer._dataModel);
        UpdateCostsForCurrentItemRecipe();
    }

    private void UpdateCostsForCurrentItemRecipe()
    {
        if (!_craftProcessRepository.TryGetCraftById(_targetEntity.CraftIdProcess, out var craft))
            return;

        if (!_cacheItemRecipies.TryGetValue(craft.ConfigModel.Uid, out var rsw))
            return;

        UpdateStateItems(rsw, _focusContainer._dataModel);
    }

    private void UpdateStateItems(RecipeStateOfWorld recipeState, ContainerData container)
    {
        _recipeGlossaryRepository.CurrentStateItems.Clear();
        for (int i = 0; i < recipeState.CurrentRecipe._inputs.Count; i++)
        {
            var input = recipeState.CurrentRecipe._inputs[i];
            if (!_cacheItemsAvailabl.ContainsKey(input.Config.TypeItem))
                _cacheItemsAvailabl[input.Config.TypeItem] = new ItemAvailable(input.Config);
            var ia = _cacheItemsAvailabl[input.Config.TypeItem];
            ia.NeedCounts = input.Amount;
            ia.AvailableCount = container.GetItemCountByType(input.Config.TypeItem);
            _recipeGlossaryRepository.CurrentStateItems.Add(ia);
        }
        _recipeGlossaryRepository.SelectedRecipe = recipeState;
    }

    private void OnSelectEntityCategory(SelectEntityCategory selectEntityCategory)
    {
        _recipeGlossaryRepository.SelectedEntityCategory = selectEntityCategory.SelectedCategory;
        UpdateCurrentEntityRecipes();
    }

    private void SelectRecipe(SelectRecipeEntity selectRecipe)
    {
        SelectEntityRecipe(selectRecipe.RecipeID);
    }

    private void SelectEntityRecipe(int recipeId)
    {
        if (_cacheEntityRecipies.TryGetValue(recipeId, out var rsw))
        {
            UpdateStateItems(rsw, _fpsCommonModel.ContainerModel._dataModel);
        }
    }

    private void UpdateCurrentEntityRecipes()
    {
        var listByCategory = _entityRecipeCatalogManager.GetEntityRecipesByCategory(_recipeGlossaryRepository.SelectedEntityCategory);
        UpdateModelRecipes(_cacheEntityRecipies, listByCategory, _fpsCommonModel.ContainerModel._dataModel);
        if(listByCategory.Count > 0)
            SelectEntityRecipe(listByCategory[0].UidKeyOfCatalog);
    }

    private void UpdateModelRecipes<T>(Dictionary<int, RecipeStateOfWorld> cache,
        List<T> recipes, ContainerData container)
        where T : BaseRecipeConfig
    {
        _recipeGlossaryRepository.CurrentModelRecipes.Clear();
        int count = recipes.Count;
        for (int i = 0; i < count; i++)
        {
            var recipe = recipes[i];
            if (!cache.ContainsKey(recipe.UidKeyOfCatalog))
                cache[recipe.UidKeyOfCatalog] = new RecipeStateOfWorld(recipe);
            var stateRecipe = cache[recipe.UidKeyOfCatalog];
            stateRecipe.CountAvailable = _craftingOperationService.GetAvailableProductionByRecipe(stateRecipe.CurrentRecipe, container);
            _recipeGlossaryRepository.CurrentModelRecipes.Add(stateRecipe);
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
        _signalBus.Subscribe<SelectRecipeEntity>(SelectRecipe);
        _signalBus.Subscribe<SelectEntityCategory>(OnSelectEntityCategory);
    }

    public void Dispose()
    {
        _signalBus?.Unsubscribe<SelectRecipe>(SelectRecipe);
        _signalBus?.Unsubscribe<SelectRecipeEntity>(SelectRecipe);
        _signalBus?.Unsubscribe<SelectEntityCategory>(OnSelectEntityCategory);
    }
}