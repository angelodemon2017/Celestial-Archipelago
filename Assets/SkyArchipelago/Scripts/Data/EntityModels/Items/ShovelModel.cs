public class ShovelModel : ItemModel
{
    public virtual int HarvestPower => 1;
    public virtual int BaseDamage => 1;

    public override CtxFlag GetTag => CtxFlag.Harvesting | CtxFlag.Damaging;
}