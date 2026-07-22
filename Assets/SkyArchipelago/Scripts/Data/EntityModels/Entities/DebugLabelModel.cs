public class DebugLabelModel : EntityModel<DebugLabelData>
{
    public override float MaxInteractionDistance => 4f;
    public override string InteractionPrompt => $"Прочитать Debug записку";
    public override bool IsInteractable => true;
    public string DebugText => GetData.Config.ContentName;

    public DebugLabelModel(DebugLabelData data) : base(data)
    {
    }
}