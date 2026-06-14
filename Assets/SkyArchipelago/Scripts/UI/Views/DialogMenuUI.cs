using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DialogMenuUI : UIWindowBase
{
    [SerializeField] private TextMeshProUGUI _targetDialog;
    [SerializeField] private Button _buttonClose;

    [Inject] private DialogModel _dialogModel;

    private void Awake()
    {
        _buttonClose.onClick.AddListener(OnClickClose);
    }

    public override void Show()
    {
        base.Show();
        _targetDialog.text = _dialogModel.CurrentNpcId;
    }

    private void OnClickClose()
    {
        _dialogModel.OnTryClosed?.Invoke();
    }


    private void OnDestroy()
    {
        _buttonClose.onClick.RemoveAllListeners();
    }
}