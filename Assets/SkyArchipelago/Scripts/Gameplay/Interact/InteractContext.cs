public struct InteractContext
{
    public EntityModel Source;
    public ItemModel Item;
    public EModeInteract ModeInteract;
    public EntityModel Target;

    public InteractContext(
        EntityModel sourc = null,
        ItemModel item = null,
        EModeInteract mode = EModeInteract.None,
        EntityModel target = null)
    {
        Source = sourc;
        Item = item;
        ModeInteract = mode;
        Target = target;
    }
}