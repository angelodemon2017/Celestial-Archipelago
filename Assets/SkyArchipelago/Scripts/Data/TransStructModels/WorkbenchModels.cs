using System.Collections.Generic;

public struct ModelOfCostElement : IModelOfCostElement
{
    private string _title;
    private int _countHave;
    private int _countNeed;

    public string Title => _title;
    public int CountHave => _countHave;
    public int CountNeed => _countNeed;

    public ModelOfCostElement(
        string title,
        int have,
        int need)
    {
        _title = title;
        _countHave = have;
        _countNeed = need;
    }
}

public struct ModelOfRecipeElement : IModelOfRecipeElement
{
    private int _recipeId;
    private string _title;
    private int _countAvailable;

    public int RecipeId => _recipeId;
    public string Title => _title;
    public int CountAvailable => _countAvailable;

    public ModelOfRecipeElement(
        int recipeId,
        string title,
        int countAvailable)
    {
        _recipeId = recipeId;
        _title = title;
        _countAvailable = countAvailable;
    }
}

public struct ModelShowRecipe
{
    private int _prevIdRecipe;
    private int _nextIdRecipe;
    private string _title;
    private int _countProduction;
    private int _craftAvailable;
    private List<IModelOfCostElement> _elements;

    public int PrevRecipeId => _prevIdRecipe;
    public int NexRecipeId => _nextIdRecipe;
    public string Title => _title;
    public int CountProduction => _countProduction;
    public int CraftAvailable => _craftAvailable;
    public List<IModelOfCostElement> Elements => _elements;

    public ModelShowRecipe(
        int prevIdRecipe,
        int nextdRecipe,
        string title,
        int countOut,
        int craftAvailable,
        List<IModelOfCostElement> elements)
    {
        _prevIdRecipe = prevIdRecipe;
        _nextIdRecipe = nextdRecipe;
        _title = title;
        _countProduction = countOut;
        _craftAvailable = craftAvailable;
        _elements = elements;
    }
}