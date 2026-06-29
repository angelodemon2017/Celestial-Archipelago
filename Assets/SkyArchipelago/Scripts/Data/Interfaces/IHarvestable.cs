public interface IHarvestable
{
    bool TryReleaseHarvest(EntityModel source, ItemModel item);
}