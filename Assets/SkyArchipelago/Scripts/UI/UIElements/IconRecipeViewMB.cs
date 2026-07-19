using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class IconRecipeViewMB : MonoBehaviour, IPoolable<IModelOfRecipeElement>
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _textSlotId;
    [SerializeField] private TextMeshProUGUI _textTitle;
    [SerializeField] private TextMeshProUGUI _textAvailableCount;
    [SerializeField] private Button _selfButton;
    [SerializeField] private Image _selectFrame;

    private IModelOfRecipeElement _modelOfRecipe;
    private int _recipeId;

    public int GetId => _modelOfRecipe.RecipeId;

    public Action<int> OnSelfButton;
    public Action<IModelOfRecipeElement> OnRecipeClick;

    public void Init(IModelOfRecipeElement modelOfRecipe)
    {
        _modelOfRecipe = modelOfRecipe;
        _recipeId = _modelOfRecipe.RecipeId;

        UpdateView();
    }

    private void UpdateView()
    {
        _textSlotId.text = _recipeId.ToTxt();//TODO replace to slotId or remove
        _textTitle.text = _modelOfRecipe.Title;
        _textAvailableCount.text = _modelOfRecipe.CountAvailable.ToTxt();
    }

    public void SetSelect(bool isTrue)
    {
        _selectFrame.enabled = isTrue;
    }

    private void OnEnable()
    {
        _selfButton.onClick.AddListener(OnSelfClick);
    }

    private void OnSelfClick()
    {
        OnSelfButton?.Invoke(_recipeId);
        OnRecipeClick?.Invoke(_modelOfRecipe);
    }

    private void OnDisable()
    {
        _selfButton.onClick.RemoveAllListeners();
    }

    public void OnSpawned(IModelOfRecipeElement p1)
    {
        Init(p1);
    }

    public void OnDespawned()
    {
    }
}