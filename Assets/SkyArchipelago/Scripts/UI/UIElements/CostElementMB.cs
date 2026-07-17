using TMPro;
using UnityEngine;
using Zenject;

public class CostElementMB : MonoBehaviour, IPoolable<IModelOfCostElement>
{
    [SerializeField] private TextMeshProUGUI _textTitle;

    private IModelOfCostElement _modelOfCostElement;

    public void OnSpawned(IModelOfCostElement p1)
    {
        _modelOfCostElement = p1;
        UpdateView();
    }

    private void UpdateView()
    {
        if(_modelOfCostElement.CountHave > 0)
            _textTitle.text = $"{_modelOfCostElement.Title} {_modelOfCostElement.CountHave.ToTxt()}/{_modelOfCostElement.CountNeed.ToTxt()}";
        else
            _textTitle.text = $"{_modelOfCostElement.Title} {_modelOfCostElement.CountNeed.ToTxt()}";
    }

    public void OnDespawned()
    {

    }
}