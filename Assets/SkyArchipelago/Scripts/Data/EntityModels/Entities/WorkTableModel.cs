public class WorkTableModel : EntityModel<WorkTableData>, IUIshowable, ICraftable
{
    public override bool IsInteractable => true;
    public override float MaxInteractionDistance => 4f;
    public override string InteractionPrompt => $"Верстак {base.InteractionPrompt}";
    public bool UIAvailable => true;//or false from state
    public bool IsActive => true;

    private int _craftIdProcess = -1;
    public int CraftIdProcess
    {
        get => _craftIdProcess;
        set 
        {
            if(_craftIdProcess != value)
                HaveChange = true;
            _craftIdProcess = value;
        }
    }

    public WorkTableModel(WorkTableData data) : base(data)
    {

    }
}