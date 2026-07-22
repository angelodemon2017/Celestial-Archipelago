using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class BasePlacebleEntityRootHandlerMB : EntityRootHandlerMB
{
    public TriggerDetector TriggerDetector;
    [SerializeField] private List<MeshRenderer> _meshRenderers;
    [SerializeField] private List<Collider> _colliders;
    [SerializeField] private List<AnchorMB> _anchors;

    private AnchorsRegistry _anchorsRegistry;

    private Dictionary<MeshRenderer, Material> normalMaterials = new();

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < _meshRenderers.Count; i++)
            normalMaterials[_meshRenderers[i]] = _meshRenderers[i].material;
    }

    [Inject]
    private void Construct(
        AnchorsRegistry anchorsRegistry)
    {
        _anchorsRegistry = anchorsRegistry;
        int count = _anchors.Count;
        for (int i = 0; i < count; i++)
            _anchorsRegistry.Register(_anchors[i].GOOfCollider, _anchors[i]);
    }

    public void EnableGhostView(Material mat)
    {
        for (int i = 0; i < _meshRenderers.Count; i++)
            _meshRenderers[i].material = mat;
        for (int i = 0; i < _colliders.Count; i++)
            _colliders[i].enabled = false;
        for (int i = 0; i < _anchors.Count; i++)
            _anchors[i].Set(false);
    }

    public void DisableGhost()
    {
        for (int i = 0; i < _meshRenderers.Count; i++)
            _meshRenderers[i].material = normalMaterials[_meshRenderers[i]];
        for (int i = 0; i < _colliders.Count; i++)
            _colliders[i].enabled = true;
        for (int i = 0; i < _anchors.Count; i++)
            _anchors[i].Set(true);
    }

    public override void OnDespawned()
    {
        base.OnDespawned();
        int count = _anchors.Count;
        for (int i = 0; i < count; i++)
            _anchorsRegistry.Unregister(_anchors[i].GOOfCollider);
    }
}