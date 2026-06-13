using System;

public interface IInputProviderContainer
{
    Action InputProviderUpdated { get; set; }

    IInputProvider InputProvider { get; }
}