public class SwordModel : ItemModel, IDamageContainer
{
    private CtxFlag _constFlag = CtxFlag.Damaging;

    public override CtxFlag GetTag => _constFlag;

    public int GetDamage { get => 10; set { } }
}