using System.Collections.Generic;
using UnityEngine;

public class HitDetector : MonoBehaviour
{
    [SerializeField] private List<Collider> colliders;

    private void OnValidate()
    {
        gameObject.layer = 8;
        colliders.Clear();
        var cols = GetComponents<Collider>();
        foreach (var col in cols)
        {
            col.isTrigger = true;
            colliders.Add(col);
        }
    }
}