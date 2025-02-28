using System;
using UnityEngine;

[DisallowMultipleComponent]
public class ParticleSystemEventProxy : MonoBehaviour
{
    public event Action OnStopped;

    private void Awake()
    {
        TryGetComponent(out ParticleSystem shuriken);
        var mainModule = shuriken.main;
        mainModule.stopAction = ParticleSystemStopAction.Callback;
    }

    private void OnDestroy()
    {
        OnStopped = null;
    }

    private void OnParticleSystemStopped()
    {
        OnStopped?.Invoke();
    }
}