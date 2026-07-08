using System.IO;

[System.Serializable]
public class PlayerData : EntityData
{
    public string PlayerName = "Player";
    public int ContainerId = -1;

    public PlayerData(string playerName)
    {
        EntityType = EEntityType.Player;
        PlayerName = playerName;
    }

    public override EntityModel CreateModel()
    {
        return new PlayerModel(this);
    }

    public override void SaveToBinary(BinaryWriter writer)
    {
        base.SaveToBinary(writer);
        writer.Write(ContainerId);
        writer.Write(PlayerName ?? string.Empty);
    }

    public override void LoadFromBinary(BinaryReader binaryReader)
    {
        base.LoadFromBinary(binaryReader);
        ContainerId = binaryReader.ReadInt32();
        PlayerName = binaryReader.ReadString();
    }
}