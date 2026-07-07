public interface IHarvestable
{
    bool AvailableHarvestBy(ItemModel item);
    EItemType GetHarvestableItemType();
    int GetHarvestableCount();
}