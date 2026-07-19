using UnityEngine;

public class FullingMaquetteHandler : BaseInteractHandler
{
    private readonly MaquetteReleaseService _maquetteReleaseService;
    private readonly FPSCommonModel _fpsCommonModel;

    public FullingMaquetteHandler(
        FPSCommonModel fpsCommonModel,
        MaquetteReleaseService maquetteReleaseService)
    {
        _fpsCommonModel = fpsCommonModel;
        _maquetteReleaseService = maquetteReleaseService;
    }

    public override int Priority => 25;

    public override bool CanHandle(ItemModel item, EntityModel target)
    {
        return (target.AvailableTags & CtxFlag.Maquette) == CtxFlag.Maquette;
    }

    public override bool TryExecute(EntityModel source, ItemModel item, EntityModel target)
    {
        if (_maquetteReleaseService.TryReleaseRecipeEntity(target.Id, _fpsCommonModel.ContainerModel.Id))
        {
            //TODO change logic on fulled ContainerOfEntity
        }

        return true;
    }
}