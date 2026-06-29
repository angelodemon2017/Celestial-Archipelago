public class ShovelModel : ItemModel
{
    public override CtxFlag GetTag => CtxFlag.Harvesting | CtxFlag.Damaging;
}