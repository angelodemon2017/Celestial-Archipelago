using System.Collections.Generic;

public class WoodChestModel : EntityModel<WoodChestData>, IUIshowable, IHaveContainer
{
    public override bool IsInteractable => true;
    public override float MaxInteractionDistance => 4f;
    public override string InteractionPrompt => $"Сундук {base.InteractionPrompt}";
    public bool UIAvailable => true;//or false from state
    public EContainerType MainContainer => EContainerType.WoodChest;
    public int IdEntityOwner => Id;

    public WoodChestModel(WoodChestData data) : base(data)
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