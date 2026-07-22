using System.Collections.Generic;

public interface IHaveContainer
{
    int IdEntityOwner { get; }
    EContainerType MainContainer { get; }
    int GetIdContainerByEType(EContainerType eType);
    bool SetIdContainerByEType(EContainerType eType, int newId);
    int GetAllContainersId(List<int> ids);
}