using System;

[Flags]
public enum ContainerAvailabilityFlag : int
{
    None = 0,
    CanHandleDrag = 1 << 0,
    CanHandleDrop = 2 << 0,
    CanAutoDrag = 4 << 0,
    CanAutoDrop = 8 << 0,
    HandleActions = CanHandleDrag | CanHandleDrop,
    AutoActions = CanAutoDrag | CanAutoDrop,
    DragActions = CanAutoDrag | CanHandleDrag,
    DropActions = CanAutoDrop | CanHandleDrop,
    ProductionContainer = CanAutoDrop | CanHandleDrag,
    FullAvailability = int.MaxValue
}