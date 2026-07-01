public class ChestModel : EntityModel<ChestData>, IUIshowable
{
    public ChestModel(ChestData data) : base(data)
    {
    }

    public bool UIAvailable => true;//or false from state
}