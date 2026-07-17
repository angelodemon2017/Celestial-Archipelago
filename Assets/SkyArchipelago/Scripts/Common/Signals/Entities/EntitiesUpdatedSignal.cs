public struct EntitiesUpdatedSignal
{
    public int IdEntity;
    public EntityModel Entity;

    public EntitiesUpdatedSignal(int id, EntityModel entity)
    {
        IdEntity = id;
        Entity = entity;
    }
}