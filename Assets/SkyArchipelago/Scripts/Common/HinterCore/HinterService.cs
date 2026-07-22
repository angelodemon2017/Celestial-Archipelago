using System;
using System.Collections.Generic;

public class HinterService
{
    private ISourceHint _currentSourceHint;
    private List<string> _currentHints = new();
//    private string _someHint;

//    public Action UpdatedHint;
    public Action UpdateListHints;

//    public string Hint => _someHint;
    public List<string> GetHints => _currentHints;

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

    public void SetListHints(List<string> hints)
    {
        _currentHints = hints;
        UpdateListHints?.Invoke();
    }

    private void UpdateHint()
    {
/*        bool isDif = _someHint != _currentSourceHint.GetHint;
        _someHint = _currentSourceHint.GetHint;
        if (isDif)
            UpdatedHint?.Invoke();/**/
    }
}