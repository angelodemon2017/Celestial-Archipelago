public struct ContainerOfEntityRequest
{
    public int IdEntity;
    public int IdContainer;

    public ContainerOfEntityRequest(int idEnt, int idCont)
    {
        IdEntity = idEnt;
        IdContainer = idCont;
    }
}