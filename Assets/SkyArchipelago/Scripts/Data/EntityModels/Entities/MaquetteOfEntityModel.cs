public class MaquetteOfEntityModel : EntityModel<MaquetteOfEntityData>
{
    public int GetIdRecipe => GetData.RecipeId;
    public int GetIdEntity => GetData.EntityId;
    public override float MaxInteractionDistance => 4f;
    public override string InteractionPrompt => $"Заполнить";
    public override bool IsInteractable => true;

    public MaquetteOfEntityModel(MaquetteOfEntityData data) : base(data)
    {
    }
}