public interface IHaveContainer
{
    int ContainerId { get; set; }
    EContainerType GetContainerType { get; }
}