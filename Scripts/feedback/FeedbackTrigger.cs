using UnityEngine;
using UnityEngine.Events;

public class FeedbackTrigger : MonoBehaviour, IFeedbackComponent
{
    [SerializeField] private UnityEvent OnPlay;
    [SerializeField] private UnityEvent OnStop;

    public bool IsStopped { get; set; }

    public void Play()
    {
        OnPlay.Invoke();
    }

    public void Stop()
    {
        OnStop.Invoke();
    }
}