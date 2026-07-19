using System;
using UnityEngine;

public class TriggerDetector : MonoBehaviour
{
    public Action<GameObject> TriggerEntered;
    public Action<GameObject> TriggerExited;

    private void OnTriggerEnter(Collider other)
    {
        TriggerEntered?.Invoke(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        TriggerExited?.Invoke(other.gameObject);
    }
}