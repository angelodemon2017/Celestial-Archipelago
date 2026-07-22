public class MaquetteOfEntityData : EntityData
{
    public int EntityId;
    public int RecipeId;
    public int ContainerId = -1;

    public MaquetteOfEntityData() => EntityType = EEntityType.MaquetteEntity;

    public override void InitConfig(ModelConfig config)
    {
        base.InitConfig(config);
        AvailableFlags |= CtxFlag.Maquette | CtxFlag.Disassemble | CtxFlag.HaveContainers;
    }

    public override EntityModel CreateModel()
    {
        return new MaquetteOfEntityModel(this);
    }

    public override void ResetData()
    {
        base.ResetData();
        ContainerId = -1;
        EntityId = -1;
    }
}