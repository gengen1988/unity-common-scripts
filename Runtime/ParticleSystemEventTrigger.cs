using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

/**
 * proxy OnParticleSystemStopped callback to event
 * useful when pooling vfx with deep hierarchy
 */
[DisallowMultipleComponent]
public class ParticleSystemEventTrigger : MonoBehaviour
{
    public UnityEvent<ParticleSystem> OnStopped = new UnityEvent<ParticleSystem>();

    private ParticleSystem _particleSystem;

    private void Awake()
    {
        Assert.IsTrue(TryGetComponent(out _particleSystem));
        ParticleSystem.MainModule main = _particleSystem.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    private void OnParticleSystemStopped()
    {
        OnStopped.Invoke(_particleSystem);
    }
}