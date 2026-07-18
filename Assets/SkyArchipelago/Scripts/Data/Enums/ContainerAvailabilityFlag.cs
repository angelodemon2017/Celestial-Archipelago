using System;

[Flags]
public enum ContainerAvailabilityFlag : int
{
    None = 0,
    CanHandleDrag = 1 << 0,
    CanHandleDrop = 1 << 1,
    CanAutoDrag = 1 << 2,
    CanAutoDrop = 1 << 3,
    HandleActions = CanHandleDrag | CanHandleDrop,
    AutoActions = CanAutoDrag | CanAutoDrop,
    DragActions = CanAutoDrag | CanHandleDrag,
    DropActions = CanAutoDrop | CanHandleDrop,
    ProductionContainer = CanAutoDrop | CanHandleDrag,
    FullAvailability = int.MaxValue
}