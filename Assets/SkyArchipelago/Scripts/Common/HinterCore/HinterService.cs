using System;

public class HinterService
{
    private ISourceHint _currentSourceHint;

    private string _someHint;

    public Action UpdatedHint;

    public string Hint => _someHint;

    public void SetSourceHint(ISourceHint sourceHint)
    {
        if (_currentSourceHint != null)
        {
            _currentSourceHint.HintUpdated -= UpdateHint;
        }

        _currentSourceHint = sourceHint;
        _currentSourceHint.HintUpdated += UpdateHint;
        UpdateHint();
    }

    private void UpdateHint()
    {
        bool isDif = _someHint != _currentSourceHint.GetHint;
        _someHint = _currentSourceHint.GetHint;
        if (isDif)
            UpdatedHint?.Invoke();
    }
}