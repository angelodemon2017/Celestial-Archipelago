public struct MoveItemBetweenContainersSignal
{
    public int ContainerIdFrom;
    public int ContainerIdTo;
    public int FromIdSlot;

    public MoveItemBetweenContainersSignal(int from, int to, int slotId)
    {
        ContainerIdFrom = from;
        ContainerIdTo = to;
        FromIdSlot = slotId;
    }
}