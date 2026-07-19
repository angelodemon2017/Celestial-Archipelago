using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class RecipesListViewMB : MonoBehaviour, IPoolable<List<IModelOfRecipeElement>>
{
    [SerializeField] private Transform _contentParent;

    [Inject] private UIMBFactory<IModelOfRecipeElement, IconRecipeViewMB> _iconViewFactory;
    [Inject] private RecipeGlossaryRepository _recipeGlossaryRepository;

    private List<IconRecipeViewMB> _iconRecipes = new();

    public Action<IModelOfRecipeElement> OnClick;

    public void OnSpawned(List<IModelOfRecipeElement> p1)
    {
        UpdateViews(p1);
    }

    public void UpdateViews(List<IModelOfRecipeElement> p1)
    {
        CleanIcons();
        int count = p1.Count;
        for (int i = 0; i < count; i++)
        {
            var imore = p1[i];
            var newIcon = _iconViewFactory.Create(imore, _contentParent);
            newIcon.OnRecipeClick += OnClickRecipe;
            newIcon.SetSelect(imore.RecipeId == _recipeGlossaryRepository.SelectedRecipe.RecipeId);
            _iconRecipes.Add(newIcon);
        }
    }

    private void OnClickRecipe(IModelOfRecipeElement more)
    {
        OnClick?.Invoke(more);
    }

    private void CleanIcons()
    {
        var count = _iconRecipes.Count;
        for (int i = 0; i < count; i++)
        {
            var icon = _iconRecipes[i];
            icon.OnRecipeClick -= OnClickRecipe;
            _iconViewFactory.Despawn(icon);
        }
        _iconRecipes.Clear();
    }

    public void OnDespawned()
    {
        CleanIcons();
        gameObject.SetActive(false);
    }
}