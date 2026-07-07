using System;
using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public Coroutine WaitAndCall(float seconds, Action call)
    {
        return StartCoroutine(WaitAndCallRout(seconds, call));
    }

    private IEnumerator WaitAndCallRout(float seconds, Action call)
    {
        yield return new WaitForSeconds(seconds);
        call?.Invoke();
    }
}