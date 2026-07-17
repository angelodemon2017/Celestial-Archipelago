public interface IHaveContainer
{
    int Id { get; }
    EContainerType MainContainer { get; }
    int GetIdContainerByEType(EContainerType eType);
    bool SetIdContainerByEType(EContainerType eType, int newId);
}