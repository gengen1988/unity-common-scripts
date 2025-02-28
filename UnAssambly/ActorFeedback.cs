using UnityEngine;

public class ActorFeedback : MonoBehaviour
{
    [SerializeField] private Feedback FeedbackOnSpawn;
    [SerializeField] private Feedback FeedbackOnKill;
    [SerializeField] private Feedback FeedbackOvertime;

    private Actor _actor;
    private Feedback _overtimeFeedback;

    private void Awake()
    {
        TryGetComponent(out _actor);
        _actor.OnBorn += HandleBorn;
        _actor.OnReady += HandleReady;
        _actor.OnKill += HandleKill;
    }

    private void LateUpdate()
    {
        if (_overtimeFeedback)
        {
            _overtimeFeedback.transform.position = transform.position;
        }
    }

    private void HandleBorn()
    {
        var position = transform.position;
        var rotation = transform.rotation;
        FeedbackOnSpawn.Play(position, rotation);
    }

    private void HandleReady()
    {
        var position = transform.position;
        var rotation = transform.rotation;
        _overtimeFeedback = FeedbackOvertime.Play(position, rotation);
    }

    private void HandleKill()
    {
        var position = transform.position;
        var rotation = transform.rotation;
        _overtimeFeedback.Stop();
        _overtimeFeedback = null;
        FeedbackOnKill.Play(position, rotation);
    }
}