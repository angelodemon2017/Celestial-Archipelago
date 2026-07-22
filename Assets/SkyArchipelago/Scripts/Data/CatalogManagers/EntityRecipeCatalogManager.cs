using System.Collections.Generic;

public class EntityRecipeCatalogManager :
    BaseCatalogManager<EntitiesRecipesCatalogConfig, EntityRecipeConfig, int>
{
    private readonly Dictionary<EEntityCategory, List<EntityRecipeConfig>> _recipesByCategory = new();
    private readonly Dictionary<int, EntityRecipeConfig> _recipesByEntity = new();

    public EntityRecipeCatalogManager(EntitiesRecipesCatalogConfig catalog) :
        base(catalog)
    {
        int count = _catalog.Elements.Count;
        for (int i = 0; i < count; i++)
        {
            var recipe = _catalog.Elements[i];
            _recipesByEntity[recipe.EntityConfig.Uid] = recipe;
        }
    }

    public List<EntityRecipeConfig> GetEntityRecipesByCategory(EEntityCategory eEntityCategory)
    {
        if (eEntityCategory == EEntityCategory.None)
            return _catalog.Elements;

        if (_recipesByCategory.Count == 0)
            for (int i = 0; i < _catalog.Elements.Count; i++)
            {
                var elem = _catalog.Elements[i];
                if (!_recipesByCategory.ContainsKey(elem.eEntityCategory))
                    _recipesByCategory[elem.eEntityCategory] = new();
                _recipesByCategory[elem.eEntityCategory].Add(elem);
            }
        return _recipesByCategory[eEntityCategory];
    }

    public bool TryGetRecipeByEntityUId(int idConfig, out EntityRecipeConfig entityRecipe)
    {
        return _recipesByEntity.TryGetValue(idConfig, out entityRecipe);
    }
}