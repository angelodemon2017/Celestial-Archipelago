using System;
using System.Collections.Generic;

public class RecipeGlossaryRepository
{
    public List<RecipeStateOfWorld> CurrentRecipes = new();
    public List<ItemAvailable> CurrentStateItems = new();

    public Action<int> ChangedSelectRecipe;
}

public class RecipeStateOfWorld : IModelOfRecipeElement
{
    public RecipeConfig RecipeConfig;
    public int CountAvailable;

    public int RecipeId => RecipeConfig.KeyOfCatalog;
    public string Title => RecipeConfig._outputs[0].Config.KeyName;
    int IModelOfRecipeElement.CountAvailable => CountAvailable;

    public RecipeStateOfWorld(RecipeConfig recipeConfig)
    {
        RecipeConfig = recipeConfig;
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