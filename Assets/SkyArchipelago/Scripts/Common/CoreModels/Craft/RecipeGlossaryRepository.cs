using System.Collections.Generic;

public class RecipeGlossaryRepository
{
    public List<IModelOfRecipeElement> CurrentModelRecipes = new();
    public List<ItemAvailable> CurrentStateItems = new();

    public IModelOfRecipeElement SelectedRecipe;
    public EEntityCategory SelectedEntityCategory;
}

public class RecipeStateOfWorld : IModelOfRecipeElement
{
    public BaseRecipeConfig CurrentRecipe;
    public int CountAvailable;

    public int RecipeId => CurrentRecipe.UidKeyOfCatalog;
    public string Title => CurrentRecipe.GetTitle;
    int IModelOfRecipeElement.CountAvailable => CountAvailable;
    public ERecipeType RecipeType => ERecipeType.Item;

    public RecipeStateOfWorld(BaseRecipeConfig recipeConfig)
    {
        CurrentRecipe = recipeConfig;
    }
}

public class ItemAvailable : IModelOfCostElement
{
    public ItemConfig ItemConfig;
    public int AvailableCount;
    public int NeedCounts;

    public string Title => ItemConfig.KeyName;
    public int CountHave => AvailableCount;
    public int CountNeed => NeedCounts;

    public ItemAvailable(ItemConfig itemConfig)
    {
        ItemConfig = itemConfig;
    }
}