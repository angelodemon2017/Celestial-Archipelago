public class RecipeCatalogManager :
    BaseCatalogManager<RecipesCatalogConfig, RecipeConfig, int>
{
    public RecipeCatalogManager(RecipesCatalogConfig catalog) :
        base(catalog)
    {
    }
}