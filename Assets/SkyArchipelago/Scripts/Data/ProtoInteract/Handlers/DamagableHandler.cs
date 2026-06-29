public class DamagableHandler : BaseInteractHandler
{
    public override int Priority => 50;
    public int DefEmptyDamage => 1;
    public int DefToolDamage => 3;//or from some config

    public override bool CanHandle(ItemModel item, EntityModel target)
    {
        return (item.GetTag & target.AvailableFlag)
            .HasFlag(CtxFlag.Damaging);
    }

    public override bool TryExecute(EntityModel source, ItemModel item, EntityModel target)
    {
        if (!(target is IDamagable damagable &&
            damagable.IsAvailableGetDamage()))
            return false;

        int startDamage = DefEmptyDamage;
        int startSpendDurability = item is SwordModel ? 1 : 5;

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
        if (entity._dataModel is IProtectedContainer protect)
        {
            damage -= protect.GetProtect;
            if (damage <= 0)
                damage = 0;
        }
        if (entity._dataModel is IHealthContainer healthContainer)
        {
            if(healthContainer.GetHealth < damage)
                damage = healthContainer.GetHealth;
            healthContainer.GetHealth -= damage;
            result = damage;
        }
        else
        {
            result = 0;
            return false;
        }

        return true;
    }
}