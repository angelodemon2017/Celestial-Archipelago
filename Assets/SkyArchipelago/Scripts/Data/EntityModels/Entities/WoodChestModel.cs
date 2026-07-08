public class WoodChestModel : EntityModel<WoodChestData>, IUIshowable, IHaveContainer
{
    public int ContainerId
    {
        get => GetData.ContainerId;
        set => GetData.ContainerId = value;
    }
    public override bool IsInteractable => true;
    public override float MaxInteractionDistance => 4f;
    public override string InteractionPrompt => $"Сундук {base.InteractionPrompt}";
    public bool UIAvailable => true;//or false from state

    public EContainerType GetContainerType => EContainerType.WoodChest;

    public WoodChestModel(WoodChestData data) : base(data)
    {
    }
}