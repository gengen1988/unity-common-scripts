using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityProxy))]
public class FeedbackManager : MonoBehaviour, IEntityFrame
{
    [SerializeField] private FeedbackLibrary library;
    [SerializeField] private bool debugLog;

    private EventBinding<GameplayCuePlay> _playBinding;
    private EventBinding<GameplayCueStop> _stopBinding;
    private EventBinding<GameplayCueUpdate> _updateBinding;

    private readonly Dictionary<CueChannel, Feedback> _prefabByChannel = new();
    private readonly Dictionary<Guid, Feedback> _feedbackByCueId = new();
    private readonly LinkedList<Feedback> _feedbacks = new();

    private void Awake()
    {
        _playBinding = new EventBinding<GameplayCuePlay>(HandleGameplayCuePlay);
        _stopBinding = new EventBinding<GameplayCueStop>(HandleGameplayCueStop);
        _updateBinding = new EventBinding<GameplayCueUpdate>(HandleGameplayCueUpdate);
    }

    private void OnDestroy()
    {
        _playBinding.Dispose();
        _stopBinding.Dispose();
        _updateBinding.Dispose();
    }

    private void OnEnable()
    {
        _prefabByChannel.Clear();
        foreach (var entry in library.Entries)
        {
            var channel = entry.Channel;
            channel.RegisterBinding(_playBinding);
            channel.RegisterBinding(_stopBinding);
            channel.RegisterBinding(_updateBinding);

            // index prefab
            Debug.Assert(entry.Prefab, "prefab not set", library);
            _prefabByChannel[channel] = entry.Prefab;
        }
    }

    private void OnDisable()
    {
        foreach (var entry in library.Entries)
        {
            entry.Channel.DeregisterBinding(_playBinding);
            entry.Channel.DeregisterBinding(_stopBinding);
            entry.Channel.DeregisterBinding(_updateBinding);
        }
    }

    public void OnEntityFrame(GameEntity entity, float deltaTime)
    {
        var node = _feedbacks.First;
        while (node != null)
        {
            var feedback = node.Value;
            var next = node.Next;

            feedback.Tick(deltaTime);

            if (feedback.IsStopped())
            {
                _feedbacks.Remove(node);
                _feedbackByCueId.Remove(feedback.CueId);
                PoolUtil.Despawn(feedback.gameObject);
            }

            node = next;
        }
    }

    private void LateUpdate()
    {
        foreach (var feedback in _feedbacks)
        {
            feedback.Follow();
        }
    }

    private void HandleGameplayCuePlay(GameplayCuePlay evt)
    {
        var channel = evt.Channel;
        var cueId = evt.CueId;
        var position = evt.Position;
        var rotation = evt.Rotation;
        if (evt.Follow)
        {
            position = evt.Follow.position;
            rotation = evt.Follow.rotation;
        }

        var prefab = _prefabByChannel[channel];
        var feedback = PoolUtil.Spawn(prefab, position, rotation);
        _feedbackByCueId[cueId] = feedback;
        _feedbacks.AddFirst(feedback);
        feedback.SetFollow(evt.Follow);
        feedback.CueId = cueId;
        if (channel.IsOneshot)
        {
            feedback.PlayOneshot();
        }
        else
        {
            feedback.Play();
        }

        if (debugLog)
        {
            Debug.Log($"[{Time.frameCount}] play {feedback}");
        }
    }

    private void HandleGameplayCueStop(GameplayCueStop evt)
    {
        var cueId = evt.CueId;
        var feedback = _feedbackByCueId.GetValueOrDefault(cueId);
        if (feedback)
        {
            feedback.Stop();
        }
    }

    private void HandleGameplayCueUpdate(GameplayCueUpdate evt)
    {
        var cueId = evt.CueId;
        var feedback = _feedbackByCueId.GetValueOrDefault(cueId);
        if (feedback)
        {
            feedback.SetParameter(evt.ParameterName, evt.ParameterValue);
        }
    }
}