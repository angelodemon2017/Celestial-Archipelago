public class FurnaceModel : EntityModel<FurnaceData>,
    IUIshowable, IHaveContainer, ICraftable, IBurnable
{
    public override bool IsInteractable => true;
    public override float MaxInteractionDistance => 4f;
    public override string InteractionPrompt => $"Печка {base.InteractionPrompt}";
    public bool UIAvailable => true;//or false from state
    public bool IsNeedBurn => CraftIdProcess >= 0;//circle 
    public bool IsActive => CraftIdProcess >= 0 && BurnIdProcess >= 0;
    public EContainerType MainContainer => EContainerType.SourceInput;
    public int CraftIdProcess
    {
        get => GetData.CraftIdProcess;
        set
        {
            if (GetData.CraftIdProcess != value)
                HaveChange = true;
            GetData.CraftIdProcess = value;
        }
    }
    public int BurnIdProcess
    {
        get => GetData.BurnIdProcess;
        set
        {
            if (GetData.BurnIdProcess != value)
                HaveChange = true;
            GetData.BurnIdProcess = value;
        }
    }

    public FurnaceModel(FurnaceData data) : base(data)
    {

    }

    public int GetIdContainerByEType(EContainerType eType)
    {
        switch (eType)
        {
            case EContainerType.SourceInput:
                return GetData.ContainerInputId;
            case EContainerType.ProductionOutput:
                return GetData.ContainerOutputId;
            case EContainerType.BurningFuel:
                return GetData.ContainerFuelId;
            default:
                return -1;
        }
    }

    public bool SetIdContainerByEType(EContainerType eType, int newId)
    {
        switch (eType)
        {
            case EContainerType.SourceInput:
                GetData.ContainerInputId = newId;
                return true;
            case EContainerType.ProductionOutput:
                GetData.ContainerOutputId = newId;
                return true;
            case EContainerType.BurningFuel:
                GetData.ContainerFuelId = newId;
                return true;
        }
        return false;
    }
}