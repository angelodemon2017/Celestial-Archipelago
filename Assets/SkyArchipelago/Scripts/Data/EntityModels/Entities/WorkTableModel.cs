public class WorkTableModel : EntityModel<WorkTableData>, IUIshowable
{
    public override bool IsInteractable => false;
    public override float MaxInteractionDistance => 4f;
    public override string InteractionPrompt => $"Верстак {base.InteractionPrompt}";
    public bool UIAvailable => false;//or false from state

    public WorkTableModel(WorkTableData data) : base(data)
    {
    }
}