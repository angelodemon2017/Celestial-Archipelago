public class TraderModel : EntityModel<TraderData>, IDamagable, IUIshowable
{
    public bool UIAvailable => true;

    public bool IsAvailableGetDamage()
    {
        return true;//or false from death state or other reason
    }
}