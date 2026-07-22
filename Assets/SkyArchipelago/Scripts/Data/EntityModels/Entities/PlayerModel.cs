using System.Collections.Generic;

public class PlayerModel : EntityModel<PlayerData>, IHaveContainer
{
    public string PlayerId => GetData.PlayerName;
    public override bool IsPhysical => true;
    public override float MoveSpeed => 5f;
    public ItemModel CurrentItem => null;
    public EContainerType MainContainer => EContainerType.BasePlayer;
    public int IdEntityOwner => Id;

    public PlayerModel(PlayerData data) : base(data)
    {
    }

    public int GetIdContainerByEType(EContainerType eType)
    {
        switch (eType)
        {
            case EContainerType.PlayerDragByUI:
                return GetData.ContainerIdOfDrag;
            default:
                return GetData.ContainerIdInventory;
        }
    }

    public bool SetIdContainerByEType(EContainerType eType, int newId)
    {
        switch (eType)
        {
            case EContainerType.PlayerDragByUI:
                GetData.ContainerIdOfDrag = newId;
                break;
            default:
                GetData.ContainerIdInventory = newId;
                break;
        }
        return true;
    }

    public int GetAllContainersId(List<int> ids)
    {
        ids.Add(GetData.ContainerIdInventory);
        ids.Add(GetData.ContainerIdOfDrag);
        return 2;
    }
}