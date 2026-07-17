public struct ContainerUpdatedSignal
{
    public int IdContainer;
    public ContainerModel Container;

    public ContainerUpdatedSignal(int id, ContainerModel container)
    {
        IdContainer = id;
        Container = container;
    }
}