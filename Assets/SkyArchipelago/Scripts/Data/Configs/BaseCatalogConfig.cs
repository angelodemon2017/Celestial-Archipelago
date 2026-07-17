using System.Collections.Generic;
using UnityEngine;

public class BaseCatalogConfig<T, T2> : BaseDataConfig
    where T : BaseCatalogElementConfig<T2>
    where T2 : struct
{
    [SerializeField] protected List<T> _elements = new();

    public List<T> Elements => _elements;

    protected virtual void OnValidate()
    {
        
    }
}