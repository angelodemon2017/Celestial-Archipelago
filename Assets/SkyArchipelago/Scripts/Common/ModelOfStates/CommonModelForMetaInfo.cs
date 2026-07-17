using System;

public class CommonModelForMetaInfo
{
    public bool _showKeyHints = false;

    public Action<bool> ToggleKeyHints;

    public void ToggleHints()
    {
        _showKeyHints = !_showKeyHints;
        ToggleKeyHints?.Invoke(_showKeyHints);
    }
}