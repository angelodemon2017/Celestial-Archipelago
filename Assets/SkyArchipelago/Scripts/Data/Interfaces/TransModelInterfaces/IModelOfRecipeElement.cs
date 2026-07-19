public interface IModelOfRecipeElement
{
    ERecipeType RecipeType { get; }
    int RecipeId { get; }
    string Title { get; }
    int CountAvailable { get; }
}