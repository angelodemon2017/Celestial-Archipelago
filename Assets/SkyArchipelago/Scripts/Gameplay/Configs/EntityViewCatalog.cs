using UnityEngine;

[CreateAssetMenu(fileName = "EntityViewCatalog", menuName = "Entity/EntityViewCatalog")]
public class EntityViewCatalog : BaseCatalogConfig<RootViewHandler, EEntityType>
{
    public EntityViewMB entityViewPrefab;

    protected override void OnValidate()
    {
        for (int i = 0; i < _elements.Count; i++)
        {
            _elements[i].modelConfig.Uid = i;
        }
    }
}

[System.Serializable]
public struct RootViewHandler : BaseCatalogElementConfig<EEntityType>
{
    public EntityRootHandlerMB entityRootHandlerPrefab;
    public ModelConfig modelConfig;

    public EEntityType UidKeyOfCatalog
    {
        get => modelConfig.eEntityType;
        set => modelConfig.eEntityType = value;
    }
}