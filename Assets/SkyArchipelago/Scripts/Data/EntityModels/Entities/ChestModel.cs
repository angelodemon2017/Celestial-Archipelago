public class ChestModel : EntityModel<ChestData>, IUIshowable, IHaveContainer
{
    public int ContainerId
    {
        get => GetData.ContainerId;
        set => GetData.ContainerId = value;
    }
    public bool UIAvailable => true;//or false from state

    public EContainerType GetContainerType => EContainerType.WoodChest;

    public ChestModel(ChestData data) : base(data)
    {
    }
}