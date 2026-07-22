using UnityEngine;

public class AnchorMB : MonoBehaviour
{
    [SerializeField] private EntityRootHandlerMB _mainRoot;
    [SerializeField] private Transform _targetPosition;
    [SerializeField] private EEntityType FilterType;
    [SerializeField] private Collider _collider;

    public EEntityType Filter => FilterType;
    public Vector3 TargetPosition => _targetPosition.position;
    public Quaternion TargetRotation => _targetPosition.rotation;
    public GameObject GOOfCollider => _collider.gameObject;

    public void Set(bool isOn)
    {
        _collider.enabled = isOn;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(_targetPosition.position, 0.5f);
    }
}