public class ShovelModel : ItemModel
{
    public virtual int HarvestPower => 1;

    public override string FullItemName => base.FullItemName;
    public override CtxFlag GetTag => CtxFlag.Harvesting | CtxFlag.Damaging;
    public override ERarityItem RarityItem => ERarityItem.Uncommon;
}