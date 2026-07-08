public class DamagableHandler : BaseInteractHandler
{
    public override int Priority => 50;
    public int DefEmptyDamage => 1;
    public int DefToolDamage => 3;//or from some config

    public override bool CanHandle(ItemModel item, EntityModel target)
    {
        if(item == null || target == null)
            return false;

        return (item.ItemTags & target.AvailableTags)
            .HasFlag(CtxFlag.Damaging);
    }

    public override bool TryExecute(EntityModel source, ItemModel item, EntityModel target)
    {
        if (!(target is IDamagable damagable &&
            damagable.IsAvailableGetDamage()))
            return false;

        int startDamage = DefEmptyDamage;
        int startSpendDurability = item is IDamageContainer ? 1 : 5;

        if (item is IDamageContainer damageContainer)
        {//or make item interface for GetDamage
            startDamage = damageContainer.GetDamage;
        }

        if (TryReleaseDamage(target, startDamage, out int totalDamage))
        {
            if (item is IDurabilable durabilable)
            {
                durabilable.TrySpendDurability(startSpendDurability);
            }
            return true;
        }

        return false;
    }

    bool TryReleaseDamage(EntityModel entity, int damage, out int result)
    {
        result = 0;

        //calcDamage

        return true;
    }
}