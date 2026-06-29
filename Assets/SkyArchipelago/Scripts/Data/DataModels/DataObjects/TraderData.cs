[System.Serializable]
public class TraderData : BaseData, IHealthContainer, IProtectedContainer
{
    public int Health;
    public int Protect;

    public int GetHealth{ get => Health; set => Health = value; }
    public int GetProtect { get => Protect; set => Protect = value; }
}