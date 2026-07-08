public struct MoveItemBetweenContainersSignal
{
    public int ContainerIdFrom;
    public int ContainerIdTo;
    public byte FromIdSlot;

    public MoveItemBetweenContainersSignal(int from, int to, byte slotId)
    {
        ContainerIdFrom = from;
        ContainerIdTo = to;
        FromIdSlot = slotId;
    }
}