public interface IBurnable : IEntity
{
    int BurnIdProcess { get; set; }
    bool IsNeedBurn { get; }
}