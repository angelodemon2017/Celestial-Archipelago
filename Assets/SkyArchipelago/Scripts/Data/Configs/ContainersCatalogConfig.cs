using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Containers/ContainersCatalog Config")]
public class ContainersCatalogConfig : BaseDataConfig
{
    [SerializeField] private List<ContainerConfig> containers = new ();

    private Dictionary<EContainerType, ContainerConfig> _cacheContainers = new();

    public ContainerConfig GetContainerConfig(EContainerType containerType)
    {
        if (!_cacheContainers.ContainsKey(containerType))
        {
            _cacheContainers.Clear();
            containers.ForEach(c => _cacheContainers.Add(c.containerType, c));
        }

        return _cacheContainers[containerType];
    }
}