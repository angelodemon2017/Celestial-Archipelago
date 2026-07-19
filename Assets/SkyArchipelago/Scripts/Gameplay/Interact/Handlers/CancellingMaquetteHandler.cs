using Zenject;

public class CancellingMaquetteHandler : BaseInteractHandler
{
    private readonly SignalBus _signalBus;
    private readonly MaquetteReleaseService _maquetteReleaseService;

    public CancellingMaquetteHandler(
        SignalBus signalBus,
        MaquetteReleaseService maquetteReleaseService)
    {
        _signalBus = signalBus;
        _maquetteReleaseService = maquetteReleaseService;
    }

    public override int Priority => 25;

    public override bool CanHandle(ItemModel item, EntityModel target)
    {
        return (target.AvailableTags & CtxFlag.Maquette) == CtxFlag.Maquette;
    }

    public override bool TryExecute(EntityModel source, ItemModel item, EntityModel target)
    {
        if(!(target is MaquetteOfEntityModel moem))
            return false;

        _signalBus.Fire(new EntityDeleteRequestSignal(moem.Id));
        return true;
    }
}