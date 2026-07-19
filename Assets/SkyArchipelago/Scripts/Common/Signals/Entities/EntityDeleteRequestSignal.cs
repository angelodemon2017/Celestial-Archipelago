public struct EntityDeleteRequestSignal
{
    public int IdEntity;
    public int IdEntityOwner;

    public EntityDeleteRequestSignal(int id, int idOwner = -1)
    {
        IdEntity = id;
        IdEntityOwner = idOwner;
    }
}