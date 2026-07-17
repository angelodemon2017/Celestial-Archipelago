using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SimpleWorkbenchViewMB : BaseViewOfModelEntity<WorkTableModel>
{
    [SerializeField] private TextMeshProUGUI _textTitleWorkbench;
    [SerializeField] private Transform _contentRecipesParent;
    [SerializeField] private Image _iconRecipe;
    [SerializeField] private TextMeshProUGUI _textRecipeName;
    [SerializeField] private TextMeshProUGUI _textProgress;
    [SerializeField] private Transform _contentCostParent;
    [SerializeField] private Button _actionUnitButton;

    [SerializeField] private TextMeshProUGUI _textTitlePlayerContainer;
    [SerializeField] private Transform _playerContainerParent;

    [Inject] private SignalBus _signalBus;
    [Inject] private UIMBFactory<IModelOfRecipeElement, IconRecipeViewMB> _iconViewFactory;
    [Inject] private UIMBFactory<IModelOfCostElement, CostElementMB> _costElementsFactory;
    [Inject] private RecipeGlossaryRepository _recipeGlossaryRepository;
    [Inject] private CraftProcessRepository _craftProcessRepository;

    private ItemsListViewMB _playerInventoryView;
    private int _recipeCounts;
    private bool _availableCurrentRecipe;
    private List<IconRecipeViewMB> _iconRecipes = new();
    private List<CostElementMB> _iconCosts = new();

    protected override void UpdateViewWithSettedModel()
    {
        base.UpdateViewWithSettedModel();
        _textTitleWorkbench.text = _model.ModelName;
        _textTitlePlayerContainer.text = PlayerContainer.TitleContainer;
        if (!_playerInventoryView)
            _playerInventoryView = GetILVMB(PLM, EContainerType.BasePlayer, _playerContainerParent);

        if(_craftProcessRepository.TryGetCraftById(_model.CraftIdProcess, out var craft))
            UpdateRecipesList(craft.ConfigModel.Uid);
        else
            UpdateRecipesList(-1);
    }

    private void UpdateRecipesList(int idSelectRecipe)
    {
        CleanRecipesList();
        _recipeCounts = _recipeGlossaryRepository.CurrentRecipes.Count;
        for (int i = 0; i < _recipeCounts; i++)
        {
            var rsw = _recipeGlossaryRepository.CurrentRecipes[i];
            var isSelectedRecipe = idSelectRecipe == rsw.RecipeId;
            var newIcon = _iconViewFactory.Create(rsw, _contentRecipesParent);
            newIcon.OnSelfButton += ClickedOnSlot;
            newIcon.SetSelect(isSelectedRecipe);
            _iconRecipes.Add(newIcon);
            if (isSelectedRecipe)
                UpdateSelectRecipe(rsw);
        }
    }

    private void UpdateSelectRecipe(RecipeStateOfWorld rsw)
    {
        _textRecipeName.text = rsw.Title;
        //_iconRecipe.sprite = icon;
        CleanCostIcons();
        var count = _recipeGlossaryRepository.CurrentStateItems.Count;
        for (int i = 0; i < count; i++)
        {
            var ia = _recipeGlossaryRepository.CurrentStateItems[i];
            var newElement = _costElementsFactory.Create(ia, _contentCostParent);
            _iconCosts.Add(newElement);
        }
        OnUpdatedProgress();
        _availableCurrentRecipe = rsw.CountAvailable > 0;
        _actionUnitButton.interactable = _availableCurrentRecipe;
    }

    private void ClickedOnSlot(int id)
    {//method for model of subState
        _signalBus.Fire(new SelectRecipe(PLM.Id, _model.Id, id));
    }

    private void OnClickActionUnit()
    {
        MainAction();
    }

    public override void MainAction()
    {//method for model of subState
        if(_availableCurrentRecipe)
            _signalBus?.Fire(new HandleCraft(PLM.Id, _model.Id));
    }

    private void OnUpdatedProgress()
    {
        var cur = _craftProcessRepository.TryGetCraftById(_model.CraftIdProcess, out var craft) ?
            craft.Process : 0;
        _textProgress.text = $"{cur.ToTxt()}/{craft.ConfigModel.ActionUnitsRequired.ToTxt()}";
    }

    private void OnEnable()
    {
        _actionUnitButton.onClick.AddListener(OnClickActionUnit);
        _recipeGlossaryRepository.ChangedSelectRecipe += UpdateRecipesList;
    }

    private void OnDisable()
    {
        _actionUnitButton.onClick.RemoveAllListeners();
        _recipeGlossaryRepository.ChangedSelectRecipe -= UpdateRecipesList;
    }

    private void CleanRecipesList()
    {
        for (int i = 0; i < _recipeCounts; i++)
        {
            _iconRecipes[i].OnSelfButton -= ClickedOnSlot;
            _iconViewFactory.Despawn(_iconRecipes[i]);
        }
        _iconRecipes.Clear();
        _recipeCounts = 0;
    }

    private void CleanCostIcons()
    {
        for (int i = 0; i < _iconCosts.Count; i++)
        {
            _costElementsFactory.Despawn(_iconCosts[i]);
        }
        _iconCosts.Clear();
    }

    public override void OnDespawned()
    {
        CleanRecipesList();
        CleanCostIcons();
        base.OnDespawned();
        if (_playerInventoryView)
        {
            _factoryContainer.Despawn(_playerInventoryView);
            _playerInventoryView = null;
        }
        gameObject.SetActive(false);
    }
}