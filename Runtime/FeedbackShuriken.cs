using System;
using UnityEngine;

public class FeedbackShuriken : MonoBehaviour
{
    private Guid _blockId;
    private ParticleSystem _shuriken;
    private bool _particleStopped;
    private bool _killInvoked;
    private Feedback _feedback;

    private void Awake()
    {
        // feedback logic
        TryGetComponent(out _feedback);
        _feedback.OnPlay += HandlePlay;
        _feedback.OnStop += HandleStop;
        _feedback.OnClear += HandleClear;

        // shuriken logic
        _shuriken = GetComponentInChildren<ParticleSystem>();
        ParticleSystemEventProxy proxy = _shuriken.EnsureComponent<ParticleSystemEventProxy>();
        proxy.OnStopped += HandleParticleSystemStopped;
    }

    private void HandlePlay(Feedback feedback)
    {
        _blockId = feedback.AcquireBlock();
    }

    private void HandleStop(Feedback feedback)
    {
        _shuriken.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void HandleClear(Feedback feedback)
    {
        _shuriken.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void HandleParticleSystemStopped()
    {
        _feedback.ReleaseBlock(_blockId);
        _feedback.Stop();
    }
}