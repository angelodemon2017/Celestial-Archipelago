public struct EntityDeleteRequestSignal
{
    public int IdEntity;
    public int IdEntityOwner;

    public EntityDeleteRequestSignal(int id, int idOwner)
    {
        IdEntity = id;
        IdEntityOwner = idOwner;
    }
}