using System.Collections.Generic;

public class EntityRecipeCatalogManager :
    BaseCatalogManager<EntitiesRecipesCatalogConfig, EntityRecipeConfig, int>
{
    private readonly Dictionary<EEntityCategory, List<EntityRecipeConfig>> _recipesByCategory = new();

    public EntityRecipeCatalogManager(EntitiesRecipesCatalogConfig catalog) :
        base(catalog)
    {
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
}