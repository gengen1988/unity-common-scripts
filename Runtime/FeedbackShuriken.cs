using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class FeedbackShuriken : MonoBehaviour
{
    [SerializeField] private float ClearTimeAfterStop = 1f;

    private Guid _blockId;
    private ParticleSystem _shuriken;
    private Feedback _feedback;

    private void Awake()
    {
        _shuriken = GetComponentInChildren<ParticleSystem>();
        ParticleSystemEventProxy proxy = _shuriken.EnsureComponent<ParticleSystemEventProxy>();
        TryGetComponent(out _feedback);
        _feedback.OnPlay += HandlePlay;
        _feedback.OnStop += HandleStop;
        proxy.OnStopped += HandleParticleSystemStopped;
    }

    private void HandlePlay(Feedback feedback)
    {
        CancelInvoke();
        _blockId = feedback.AcquireBlocker();
    }

    private void HandleStop(Feedback feedback)
    {
        _shuriken.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        Invoke(nameof(Clear), ClearTimeAfterStop);
    }

    private void HandleParticleSystemStopped()
    {
        _feedback.ReleaseBlocker(_blockId);
    }

    private void Clear()
    {
        _shuriken.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}