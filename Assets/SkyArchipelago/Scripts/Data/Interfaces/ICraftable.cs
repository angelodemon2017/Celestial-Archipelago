public interface ICraftable : IEntity
{
    bool IsActive { get; }
    int CraftIdProcess { get; set; }
    virtual int UAPower => 1;
}