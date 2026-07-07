using UnityEngine;

public class DebugLabelModel : EntityModel<DebugLabelData>, IHarvestable
{
    public override float MaxInteractionDistance => 4f;
    public override string InteractionPrompt => $"Прочитать Debug записку";
    public override bool IsInteractable => true;
    public string DebugText => GetData.Config.ContentName;

    public DebugLabelModel(DebugLabelData data) : base(data)
    {
    }

    public EItemType GetHarvestableItemType()
    {
        return Random.Range(1, 100) > 50 ? EItemType.Wood : EItemType.Rock;
    }

    public int GetHarvestableCount()
    {
        return Random.Range(1, 10);
    }

    public bool AvailableHarvestBy(ItemModel item)
    {
//        return item.GetTag.HasFlag(CtxFlag.Harvesting);
        return true;
    }
}