public class ShowUIHandler : BaseInteractHandler
{
//    private readonly DialogModel _dialogModel;

    public ShowUIHandler()
    {

    }

    public override int Priority => 1;

    public override bool CanHandle(ItemModel item, EntityModel target)
    {
        if (target is IUIshowable uIshowable)
            return uIshowable.UIAvailable;

        return false;
    }

    public override bool TryExecute(EntityModel source, ItemModel item, EntityModel target)
    {
        //_dialogModel.SetMetaFromTarget
        //push signal about open 

        return true;
    }
}