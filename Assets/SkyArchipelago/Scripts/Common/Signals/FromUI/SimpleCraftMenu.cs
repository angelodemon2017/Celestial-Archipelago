public struct SelectRecipe
{
    public int ReasonEntityId;
    public int TargetEntityId;
    public int RecipeID;

    public SelectRecipe(int reasonEntityId, int targetEntityId, int recipeId)
    {
        ReasonEntityId = reasonEntityId;
        TargetEntityId = targetEntityId;
        RecipeID = recipeId;
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

