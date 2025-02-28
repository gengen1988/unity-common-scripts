using Sirenix.OdinInspector;
using UnityEngine;

public class FeedbackShuriken : MonoBehaviour
{
    // [SerializeField] private SortingLayer ParticleLayer;

    private Feedback _feedback;
    private ParticleSystem _shuriken;

    private void Awake()
    {
        _shuriken = GetComponentInChildren<ParticleSystem>();
        if (!_shuriken)
        {
            Debug.LogWarning($"No particle system found in {gameObject}, please remove {GetType()}", this);
            Destroy(this);
            return;
        }

        var proxy = _shuriken.EnsureComponent<ParticleSystemEventProxy>(true);
        proxy.OnStopped += HandleParticleSystemStopped;

        _feedback = this.EnsureComponent<Feedback>();
        _feedback.OnPlay += HandlePlay;
        _feedback.OnStop += HandleStop;
        _feedback.OnClear += HandleClear;
    }

    private void HandlePlay()
    {
        _feedback.AcquireBlock(this);
        _shuriken.Play(true);
    }

    private void HandleStop()
    {
        _shuriken.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void HandleClear()
    {
        _shuriken.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void HandleParticleSystemStopped()
    {
        _feedback.ReleaseBlock(this);
    }

    [Button]
    private void EnsureShurikenLayer()
    {
        var particleSystems = GetComponentsInChildren<ParticleSystem>();
        foreach (var system in particleSystems)
        {
            // system.main.
        }
    }
}