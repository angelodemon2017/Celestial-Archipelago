using System.Collections.Generic;

public class EntitiesCatalogManager :
    BaseCatalogManager<EntityViewCatalog, RootViewHandler, int>
{
    private Dictionary<int, Dictionary<CtxFlag, ModuleConfig>> _cacheModules = new();

    public EntitiesCatalogManager(EntityViewCatalog catalog) : base(catalog)
    {
    }

    public bool TryGetModule(int id, CtxFlag ctxFlag, out ModuleConfig module)
    {
        if (!_cacheModules.ContainsKey(id))
        {
            _cacheModules[id] = new();
            if (TryGetConfigByKey(id, out var config))
                for (int i = 0; i < config.modelConfig.ModuleConfigs.Count; i++)
                {
                    var mod = config.modelConfig.ModuleConfigs[i];
                    _cacheModules[id][mod.KeyFlag] = mod;
                }
        }
        return _cacheModules[id].TryGetValue(ctxFlag, out module);
    }
}