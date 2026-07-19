using System.Collections.Generic;
using UnityEngine;

public class BasePlacebleEntityRootHandlerMB : EntityRootHandlerMB
{
    public TriggerDetector TriggerDetector;
    [SerializeField] private List<MeshRenderer> _meshRenderers;
    [SerializeField] private List<Collider> _colliders;

    private Dictionary<MeshRenderer, Material> normalMaterials = new();

    protected override void Awake()
    {
        base.Awake();
        for(int i=0;i< _meshRenderers.Count;i++)
            normalMaterials[_meshRenderers[i]] = _meshRenderers[i].material;
    }

    public void EnableGhostView(Material mat)
    {
        for (int i = 0; i < _meshRenderers.Count; i++)
            _meshRenderers[i].material = mat;
        for (int i = 0; i < _colliders.Count; i++)
            _colliders[i].enabled = false;
    }

    public void DisableGhost()
    {
        for (int i = 0; i < _meshRenderers.Count; i++)
            _meshRenderers[i].material = normalMaterials[_meshRenderers[i]];
        for (int i = 0; i < _colliders.Count; i++)
            _colliders[i].enabled = true;
    }

    public override void OnDespawned()
    {
        base.OnDespawned();
    }
}