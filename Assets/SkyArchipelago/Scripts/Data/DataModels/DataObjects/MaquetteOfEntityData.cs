public class MaquetteOfEntityData : EntityData
{
    public int EntityId;
    public int RecipeId;

    public MaquetteOfEntityData() => EntityType = EEntityType.MaquetteEntity;

    public override void InitConfig(ModelConfig config)
    {
        base.InitConfig(config);
        AvailableFlags |= CtxFlag.Maquette;
    }

    public override EntityModel CreateModel()
    {
        return new MaquetteOfEntityModel(this);
    }
}