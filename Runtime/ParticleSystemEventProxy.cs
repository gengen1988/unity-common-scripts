using System;
using UnityEngine;

[DisallowMultipleComponent]
public class ParticleSystemEventProxy : MonoBehaviour
{
    public event Action OnStopped;

    private ParticleSystem _particleSystem;

    private void Awake()
    {
        TryGetComponent(out _particleSystem);
    }

    private void OnDestroy()
    {
        OnStopped = null;
    }

    private void OnEnable()
    {
        ParticleSystem.MainModule main = _particleSystem.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    private void OnParticleSystemStopped()
    {
        OnStopped?.Invoke();
    }
}