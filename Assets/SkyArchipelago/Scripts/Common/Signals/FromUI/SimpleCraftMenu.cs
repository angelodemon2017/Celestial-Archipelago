public struct SelectRecipe
{
    public ERecipeType RecipeType;
    public int ReasonEntityId;
    public int TargetEntityId;
    public int RecipeID;

    public SelectRecipe(ERecipeType recipeType, int reasonEntityId, int targetEntityId, int recipeId)
    {
        RecipeType = recipeType;
        ReasonEntityId = reasonEntityId;
        TargetEntityId = targetEntityId;
        RecipeID = recipeId;
    }
}

public struct SelectRecipeEntity
{
    public int RecipeID;

    public SelectRecipeEntity(int recipeId)
    {
        RecipeID = recipeId;
    }
}

public struct SelectEntityCategory
{
    public EEntityCategory SelectedCategory;

    public SelectEntityCategory(EEntityCategory selectedCategory)
    {
        SelectedCategory = selectedCategory;
    }
}

public struct StartPlaceEntity
{
    public int RecipeId;

    public StartPlaceEntity(int recipeId)
    {
        RecipeId = recipeId;
    }
}

public struct HandleCraft
{
    public int ReasonEntityId;
    public int TargetEntityId;

    public HandleCraft(int reasonEntityId, int targetEntityId)
    {
        ReasonEntityId = reasonEntityId;
        TargetEntityId = targetEntityId;
    }
}