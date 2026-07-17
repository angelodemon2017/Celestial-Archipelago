public class WoodChestModel : EntityModel<WoodChestData>, IUIshowable, IHaveContainer
{
    public override bool IsInteractable => true;
    public override float MaxInteractionDistance => 4f;
    public override string InteractionPrompt => $"Сундук {base.InteractionPrompt}";
    public bool UIAvailable => true;//or false from state

    public EContainerType MainContainer => EContainerType.WoodChest;

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
}