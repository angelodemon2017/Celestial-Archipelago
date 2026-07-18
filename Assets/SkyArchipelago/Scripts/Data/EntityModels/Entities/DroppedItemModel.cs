public class DroppedItemModel : EntityModel<DroppedItemData>
{
    public int IdOwnedDrop => GetData.IdOwner;
    public EItemType eItemType => GetData.TypeItem;
    public int Count => GetData.Count;

    public override bool IsInteractable => true;
    public override float MaxInteractionDistance => 3f;
    public override string InteractionPrompt => $"Pick UP {eItemType}";

    public DroppedItemModel(DroppedItemData data) : base(data)
    {
    }
}