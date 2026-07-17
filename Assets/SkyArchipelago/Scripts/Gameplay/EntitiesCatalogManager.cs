using System.Collections.Generic;

public class EntitiesCatalogManager :
    BaseCatalogManager<EntityViewCatalog, RootViewHandler, EEntityType>
{
    private Dictionary<EEntityType, Dictionary<CtxFlag, ModuleConfig>> _cacheModules = new();

    public EntitiesCatalogManager(EntityViewCatalog catalog) :
        base(catalog)
    {
    }

    public bool TryGetModule(EEntityType entityType, CtxFlag ctxFlag, out ModuleConfig module)
    {
        if (!_cacheModules.ContainsKey(entityType))
        {
            _cacheModules[entityType] = new();
            if (TryGetConfigByKey(entityType, out var config))
                for (int i = 0; i < config.modelConfig.ModuleConfigs.Count; i++)
                {
                    var mod = config.modelConfig.ModuleConfigs[i];
                    _cacheModules[entityType][mod.KeyFlag] = mod;
                }
        }
        return _cacheModules[entityType].TryGetValue(ctxFlag, out module);
    }
}