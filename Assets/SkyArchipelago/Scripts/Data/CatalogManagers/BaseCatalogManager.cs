using System.Collections.Generic;

public class BaseCatalogManager<T, T2, T3>
    where T : BaseCatalogConfig<T2, T3>
    where T2 : BaseCatalogElementConfig<T3>
    where T3 : struct
{
    private T _catalog;

    private Dictionary<T3, T2> _cacheConfigs = new();

    public BaseCatalogManager(
        T catalog)
    {
        _catalog = catalog;

        int count = _catalog.Elements.Count;
        for (int i = 0; i < count; i++)
            _cacheConfigs.Add(_catalog.Elements[i].KeyOfCatalog, _catalog.Elements[i]);
    }

    public bool TryGetConfigByKey(T3 key, out T2 configResult)
    {
        if (_cacheConfigs.TryGetValue(key, out configResult))
            return true;
        return false;
    }
}