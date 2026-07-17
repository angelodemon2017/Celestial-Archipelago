public struct MoveAmountBetweenSlotsSignal
{
    public int ContainerIdFrom { get; }
    public int ContainerIdTo { get; }
    public int FromSlotId { get; }
    public int ToSlotId { get; }
    public int Amount { get; }

    public MoveAmountBetweenSlotsSignal(int from, int to, int fromSlot, int toSlot, int amount)
    {
        ContainerIdFrom = from;
        ContainerIdTo = to;
        FromSlotId = fromSlot;
        ToSlotId = toSlot;
        Amount = amount;
    }
}