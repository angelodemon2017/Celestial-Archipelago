using UnityEngine;

[CreateAssetMenu(fileName = "EntityViewCatalog", menuName = "Entity/EntityViewCatalog")]
public class EntityViewCatalog : BaseCatalogConfig<RootViewHandler, int>
{
    public EntityViewMB entityViewPrefab;

    protected override void OnValidate()
    {
        for (int i = 0; i < _elements.Count; i++)
        {
            var element = _elements[i];
            element.JustName = _elements[i].modelConfig.name;
            element.modelConfig.Uid = i;
        }
    }
}

[System.Serializable]
public class RootViewHandler : BaseCatalogElementConfig<int>
{
    public string JustName;
    public EntityRootHandlerMB entityRootHandlerPrefab;
    public ModelConfig modelConfig;

    public int UidKeyOfCatalog
    {
        get => modelConfig.Uid;
        set => modelConfig.Uid = value;
    }
}