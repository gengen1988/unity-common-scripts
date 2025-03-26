using System;
using UnityEngine;

[CreateAssetMenu]
public class CueChannel : ScriptableObject
{
    [SerializeField] private bool isOneshot = true;

    [Multiline]
    [SerializeField] private string notes;

    private readonly EventBus<GameplayCuePlay> _playEvent = new();
    private readonly EventBus<GameplayCueStop> _stopEvent = new();
    private readonly EventBus<GameplayCueUpdate> _updateEvent = new();

    public bool IsOneshot => isOneshot;

    public Guid Play()
    {
        var cueId = Guid.NewGuid();
        var cue = new GameplayCuePlay
        {
            Channel = this,
            CueId = cueId,
            Position = Vector3.zero,
            Rotation = Quaternion.identity,
        };

        _playEvent.Raise(cue);
        return cueId;
    }

    public Guid Play(Vector3 position)
    {
        var cueId = Guid.NewGuid();
        var cue = new GameplayCuePlay
        {
            Channel = this,
            CueId = cueId,
            Position = position,
            Rotation = Quaternion.identity,
        };

        _playEvent.Raise(cue);
        return cueId;
    }

    public Guid Play(Vector3 position, Quaternion rotation)
    {
        var cueId = Guid.NewGuid();
        var cue = new GameplayCuePlay
        {
            Channel = this,
            CueId = cueId,
            Position = position,
            Rotation = rotation,
        };

        _playEvent.Raise(cue);
        return cueId;
    }

    public Guid Play(Transform follow)
    {
        var cueId = Guid.NewGuid();
        var cue = new GameplayCuePlay
        {
            Channel = this,
            CueId = cueId,
            Follow = follow,
        };

        _playEvent.Raise(cue);
        return cueId;
    }

    public void Stop(Guid cueId)
    {
        var cue = new GameplayCueStop
        {
            Channel = this,
            CueId = cueId,
        };

        _stopEvent.Raise(cue);
    }

    public void SetParameter(Guid cueId, string parameterName, string parameterValue)
    {
        var cue = new GameplayCueUpdate
        {
            Channel = this,
            CueId = cueId,
            ParameterName = parameterName,
            ParameterValue = parameterValue
        };

        _updateEvent.Raise(cue);
    }

    public void RegisterBinding(EventBinding<GameplayCuePlay> binding)
    {
        _playEvent.Register(binding);
    }

    public void RegisterBinding(EventBinding<GameplayCueStop> binding)
    {
        _stopEvent.Register(binding);
    }

    public void RegisterBinding(EventBinding<GameplayCueUpdate> binding)
    {
        _updateEvent.Register(binding);
    }

    public void DeregisterBinding(EventBinding<GameplayCuePlay> binding)
    {
        _playEvent.Deregister(binding);
    }

    public void DeregisterBinding(EventBinding<GameplayCueStop> binding)
    {
        _stopEvent.Deregister(binding);
    }

    public void DeregisterBinding(EventBinding<GameplayCueUpdate> binding)
    {
        _updateEvent.Deregister(binding);
    }
}

public static class CueChannelUtil
{
    public static Guid PlayIfNotNull(this CueChannel channel)
    {
        if (!channel)
        {
            return Guid.Empty;
        }

        return channel.Play();
    }

    public static Guid PlayIfNotNull(this CueChannel channel, Vector3 position)
    {
        if (!channel)
        {
            return Guid.Empty;
        }

        return channel.Play(position);
    }

    public static Guid PlayIfNotNull(this CueChannel channel, Vector3 position, Quaternion rotation)
    {
        if (!channel)
        {
            return Guid.Empty;
        }

        return channel.Play(position, rotation);
    }

    public static Guid PlayIfNotNull(this CueChannel channel, Transform transform)
    {
        if (!channel)
        {
            return Guid.Empty;
        }

        return channel.Play(transform);
    }

    public static void StopIfNotNull(this CueChannel channel, Guid cueId)
    {
        if (!channel)
        {
            return;
        }

        channel.Stop(cueId);
    }
}

public struct GameplayCuePlay : IEvent
{
    public CueChannel Channel;
    public Guid CueId;
    public Vector3 Position;
    public Quaternion Rotation;
    public Transform Follow;
}

public struct GameplayCueStop : IEvent
{
    public CueChannel Channel;
    public Guid CueId;
}

public struct GameplayCueUpdate : IEvent
{
    public CueChannel Channel;
    public Guid CueId;
    public string ParameterName;
    public string ParameterValue;
}