using System;
using System.Linq;
using UnityEngine;

public enum FeedbackState
{
    Playing,
    Stopping,
    Stopped,
}

public class Feedback : MonoBehaviour
{
    [SerializeField] private float maxTime = 3f;

    private float _elapsedTime;
    private FeedbackState _state;
    private GameEntity _followEntity;
    private Transform _followTrans;
    private IFeedbackComponent[] _feedbackComponents;

    public Guid CueId { get; set; } // for feedback manager cleanup

    private void Awake()
    {
        _feedbackComponents = GetComponents<IFeedbackComponent>();
    }

    /// <summary>
    /// Plays the feedback continuously. The Playing state must be managed externally (e.g., via Stop() or other systems).
    /// </summary>
    public void Play()
    {
        _elapsedTime = 0;
        _state = FeedbackState.Playing;
        foreach (var component in _feedbackComponents)
        {
            component.Play();
        }
    }

    public void PlayOneshot()
    {
        _elapsedTime = 0;
        _state = FeedbackState.Stopping;
        foreach (var component in _feedbackComponents)
        {
            component.Play();
        }
    }

    public void Stop()
    {
        if (_state != FeedbackState.Playing)
        {
            Debug.LogAssertion($"[{this}] only continuous feedback can be stop", this);
            return;
        }

        _elapsedTime = 0;
        _state = FeedbackState.Stopping;
        foreach (var component in _feedbackComponents)
        {
            component.Stop();
        }
    }

    public void SetFollow(Transform followTrans)
    {
        if (followTrans)
        {
            _followEntity = followTrans.GetEntity(true);
            _followTrans = followTrans;
        }
        else
        {
            _followEntity = null;
            _followTrans = null;
        }
    }

    public void SetParameter(string parameterName, string parameterValue)
    {
        throw new NotImplementedException();
    }

    public void Follow()
    {
        if (_followEntity)
        {
            transform.position = _followTrans.position;
        }
    }

    public void Tick(float deltaTime)
    {
        // ignore playing state (controlled by other logic)

        if (_state == FeedbackState.Stopping)
        {
            _elapsedTime += deltaTime;
            if (maxTime > 0 && _elapsedTime >= maxTime)
            {
                _state = FeedbackState.Stopped;
                return;
            }

            if (_feedbackComponents.All(comp => comp.IsStopped))
            {
                _state = FeedbackState.Stopped;
                return;
            }
        }
    }

    public bool IsStopped()
    {
        return _state == FeedbackState.Stopped;
    }
}

public interface IFeedbackComponent
{
    public bool IsStopped { get; }
    public void Play();
    public void Stop();
}