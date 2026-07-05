public class PlayerModel : EntityModel<PlayerData>, IHaveContainer
{
    public string PlayerId => GetData.PlayerName;
    public override bool IsPhysical => true;
    public override float MoveSpeed => 5f;
    public ItemModel CurrentItem => null;

    public int ContainerId
    {
        get => GetData.ContainerId;
        set => GetData.ContainerId = value;
    }

    public EContainerType GetContainerType => EContainerType.BasePlayer;

    public PlayerModel(PlayerData data) : base(data)
    {
    }
}