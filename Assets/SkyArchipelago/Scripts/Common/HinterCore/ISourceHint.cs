using System;

public interface ISourceHint
{
    string GetHint { get; }

    Action HintUpdated { get; set; }
}