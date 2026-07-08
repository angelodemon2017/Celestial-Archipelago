public class DemoNPCModel : EntityModel<DemoNPCData>, IUIshowable
{
    public string NpcId => GetData.NpcId;
    public override bool IsInteractable => true;
    public override float MaxInteractionDistance => 4f;
    public override string InteractionPrompt => $"{NpcId} {base.InteractionPrompt}";
    public bool UIAvailable => true;

    public DemoNPCModel(DemoNPCData data) : base(data)
    {
    }

    public int GetHarvestableCount()
    {
        return 1;
    }

    public bool AvailableHarvestBy(ItemModel item)
    {
        return true;
    }
}