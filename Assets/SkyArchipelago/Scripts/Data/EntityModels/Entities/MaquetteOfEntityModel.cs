using System.Collections.Generic;

[System.Serializable]//delete Serializable
public class MaquetteOfEntityModel : EntityModel<MaquetteOfEntityData>, IHaveContainer
{
    public int GetIdRecipe => GetData.RecipeId;
    public int GetIdEntity => GetData.EntityId;
    public override float MaxInteractionDistance => 4f;
    public override string InteractionPrompt => $"Заполнить";
    public override bool IsInteractable => true;

    public int IdEntityOwner => Id;

    public EContainerType MainContainer => EContainerType.MaquetteSource;

    public MaquetteOfEntityModel(MaquetteOfEntityData data) : base(data)
    {
    }

    public int GetIdContainerByEType(EContainerType eType)
    {
        return GetData.ContainerId;
    }

    public bool SetIdContainerByEType(EContainerType eType, int newId)
    {
        GetData.ContainerId = newId;
        return true;
    }

    public int GetAllContainersId(List<int> ids)
    {
        ids.Add(GetData.ContainerId);
        return 1;
    }
}