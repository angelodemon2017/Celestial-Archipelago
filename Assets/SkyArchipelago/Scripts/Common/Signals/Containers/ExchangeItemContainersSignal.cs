public struct ExchangeItemContainersSignal
{
    public int ContainerIdFrom;
    public int ContainerIdTo;
    public int FromIdSlot;
    public int ToIdSlot;

    public ExchangeItemContainersSignal(int from, int to, int slotId, int toIdSlot)
    {
        ContainerIdFrom = from;
        ContainerIdTo = to;
        FromIdSlot = slotId;
        ToIdSlot = toIdSlot;
    }
}