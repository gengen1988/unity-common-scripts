using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameEntityBridge))]
public class Feedback : StatefulEntityBehaviour<Feedback>
{
    public const string MSG_STOP = "stop";

    // event action are more flexible when other refuse to register on awake
    public event Action OnPlay;
    public event Action OnStop;
    public event Action OnClear;

    [SerializeField] private float MaxDespawningTime = 1f;

    private float _elapsedTime;
    private readonly HashSet<object> _blocks = new();

    protected override IState<Feedback> InitialState => Playing.Instance;

    private void OnDestroy()
    {
        OnPlay = null;
        OnStop = null;
        OnClear = null;
    }

    public void AcquireBlock(object obj)
    {
        _blocks.Add(obj);
    }

    public void ReleaseBlock(object obj)
    {
        _blocks.Remove(obj);
    }

    private class Playing : State<Feedback, Playing>
    {
        public override void OnEnter(Feedback ctx)
        {
            // Debug.Log("feedback play");
            ctx.RegisterTransition(MSG_STOP, Stopping.Instance);
            ctx._blocks.Clear();
            ctx.OnPlay?.Invoke();
        }

        public override void OnRefresh(Feedback ctx)
        {
            // block
            if (ctx._blocks.Count == 0)
            {
                ctx.StateMachine.Send(MSG_STOP);
            }
        }
    }

    private class Stopping : State<Feedback, Stopping>
    {
        public override void OnEnter(Feedback ctx)
        {
            // Debug.Log("feedback stop");
            ctx._elapsedTime = 0;
            ctx.OnStop?.Invoke();
        }

        public override void OnRefresh(Feedback ctx)
        {
            // force clear when timeout
            if (ctx._elapsedTime >= ctx.MaxDespawningTime)
            {
                ctx._blocks.Clear();
            }

            // no longer blocked
            if (ctx._blocks.Count == 0)
            {
                ctx.OnClear?.Invoke();
                PoolUtil.Despawn(ctx);
                return;
            }

            ctx._elapsedTime += ctx.LocalDeltaTime;
        }
    }
}

public static class FeedbackUtil
{
    public static Feedback Play(this Feedback prefab, Vector3 position, Quaternion rotation)
    {
        return PoolUtil.Spawn(prefab, position, rotation);
    }

    public static Feedback Play(this Feedback prefab)
    {
        return PoolUtil.Spawn(prefab, Vector3.zero, Quaternion.identity);
    }

    public static void Stop(this Feedback feedback)
    {
        feedback?.StateMachine.Send(Feedback.MSG_STOP);
    }
}