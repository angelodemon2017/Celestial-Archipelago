using System.IO;

[System.Serializable]
public class PlayerData : EntityData
{
    public string PlayerName = "Player";
    public int ContainerIdInventory = -1;
    public int ContainerIdOfDrag = -1;

    public PlayerData()
    {
        EntityType = EEntityType.Player;
    }

    public override EntityModel CreateModel()
    {
        return new PlayerModel(this);
    }

    public override void SaveToBinary(BinaryWriter writer)
    {
        base.SaveToBinary(writer);
        writer.Write(ContainerIdInventory);
        writer.Write(PlayerName ?? string.Empty);
    }

    public override void LoadFromBinary(BinaryReader binaryReader)
    {
        base.LoadFromBinary(binaryReader);
        ContainerIdInventory = binaryReader.ReadInt32();
        PlayerName = binaryReader.ReadString();
    }
}