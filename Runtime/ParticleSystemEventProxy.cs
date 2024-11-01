using System;
using UnityEngine;

[DisallowMultipleComponent]
public class ParticleSystemEventProxy : MonoBehaviour
{
    public event Action OnStopped;

    private void Awake()
    {
        ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        Transform self = transform;
        foreach (ParticleSystem ps in particles)
        {
            ParticleSystem.MainModule main = ps.main;
            main.stopAction = ps.transform == self ? ParticleSystemStopAction.Callback : ParticleSystemStopAction.None;
        }
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