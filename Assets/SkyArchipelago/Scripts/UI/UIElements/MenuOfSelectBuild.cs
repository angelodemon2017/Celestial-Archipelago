using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MenuOfSelectBuild : MonoBehaviour, IPoolable<int>
{
    [SerializeField] private List<Button> _tabButtons;
    [SerializeField] private Transform _parentRecipeList;
    [SerializeField] private TextMeshProUGUI _textTitle;
    [SerializeField] private Image _iconRecipe;
    [SerializeField] private Transform _parentCosts;
    [SerializeField] private Button _startPlaceButton;

    [Inject] private SignalBus _signalBus;
    [Inject] private UIMBFactory<List<IModelOfRecipeElement>, RecipesListViewMB> _recipeListFactory;
    [Inject] private UIMBFactory<IModelOfCostElement, CostElementMB> _costElementsFactory;
    [Inject] private RecipeGlossaryRepository _recipeGlossaryRepository;
    [Inject] private EntityRecipeCatalogManager _recipeCatalogManager;

    private int _tabCount;
    private int _lastTab = 0;
    private RecipesListViewMB _recipesListViewMB;
    private List<CostElementMB> _iconCosts = new();

    private void Awake()
    {
        _tabCount = _tabButtons.Count;
        _tabButtons[0].onClick.AddListener(() => OnClickTab(0));
        _tabButtons[1].onClick.AddListener(() => OnClickTab(1));
        _tabButtons[2].onClick.AddListener(() => OnClickTab(2));
        _startPlaceButton.onClick.AddListener(OnClickStartBuild);
    }

    public void OnSpawned(int dummyParametr)
    {
        OnClickTab(_lastTab);
    }

    private void OnClickTab(int id)
    {
        _tabButtons[_lastTab].interactable = true;
        _tabButtons[id].interactable = false;
        _lastTab = id;
        OpenCategory((EEntityCategory)(id + 1));
    }

    private void OpenCategory(EEntityCategory entityCategory)
    {
        _signalBus.Fire(new SelectEntityCategory(entityCategory));
        UpdateRecipeList();
        UpdateSelectRecipe();
    }

    private void UpdateRecipeList()
    {
        TryDespawnCurrentList();
        var list = _recipeGlossaryRepository.CurrentModelRecipes;
        _recipesListViewMB = _recipeListFactory.Create(list, _parentRecipeList);
        _recipesListViewMB.OnClick += OnSelectRecipe;
    }

    private void OnSelectRecipe(IModelOfRecipeElement modelOfRecipeElement)
    {
        _signalBus.Fire(new SelectRecipeEntity(modelOfRecipeElement.RecipeId));
        UpdateSelectRecipe();
    }

    private void UpdateSelectRecipe()
    {
        var mre = _recipeGlossaryRepository.SelectedRecipe;
        _textTitle.text = mre.Title;
        //set icon
        CleanCostIcons();
        var count = _recipeGlossaryRepository.CurrentStateItems.Count;
        for (int i = 0; i < count; i++)
        {
            var ia = _recipeGlossaryRepository.CurrentStateItems[i];
            var newElement = _costElementsFactory.Create(ia, _parentCosts);
            _iconCosts.Add(newElement);
        }

        if (_recipeCatalogManager.TryGetConfigByKey(mre.RecipeId, out var recipe))
            _startPlaceButton.interactable = !recipe.IsDeleteInputsOnPlacement || mre.CountAvailable > 0;
        else
            _startPlaceButton.interactable = mre.CountAvailable > 0;
    }

    private void OnClickStartBuild()
    {
        _signalBus.Fire(new StartPlaceEntity(_recipeGlossaryRepository.SelectedRecipe.RecipeId));
    }

    private void TryDespawnCurrentList()
    {
        if (_recipesListViewMB)
        {
            _recipesListViewMB.OnClick -= OnSelectRecipe;
            _recipeListFactory.Despawn(_recipesListViewMB);
            _recipesListViewMB = null;
        }
    }

    private void CleanCostIcons()
    {
        for (int i = 0; i < _iconCosts.Count; i++)
        {
            _costElementsFactory.Despawn(_iconCosts[i]);
        }
        _iconCosts.Clear();
    }

    public void OnDespawned()
    {
        TryDespawnCurrentList();
        CleanCostIcons();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < _tabCount; i++)
            _tabButtons[i].onClick.RemoveAllListeners();
        _startPlaceButton.onClick.RemoveAllListeners();
    }
}