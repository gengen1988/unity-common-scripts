using UnityEngine;

// two types:
// 1. trail (continuous)
//   - block playing until request stop
//   - block stopping until particle stopped
// 2. explosion (one-shot)
//   - do not block playing (enter stopping immediate after play)
//   - block stopping until particle stopped
[RequireComponent(typeof(Feedback))]
public class FeedbackShuriken : MonoBehaviour, IFeedbackComponent
{
    private ParticleSystem _shuriken;

    private void Awake()
    {
        _shuriken = GetComponentInChildren<ParticleSystem>();
        Debug.Assert(_shuriken.main.stopAction != ParticleSystemStopAction.Destroy, "shuriken should not destroy itself", _shuriken);
        if (!_shuriken)
        {
            Debug.LogWarning($"No particle system found in {gameObject}, please remove {GetType()}", this);
            Destroy(this);
        }
    }

    public bool IsStopped => _shuriken.isStopped;

    public void Play()
    {
        _shuriken.Play(true);
    }

    public void Stop()
    {
        _shuriken.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}