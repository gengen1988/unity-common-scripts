using System;
using UnityEngine;

public abstract class StatefulBehaviour<TContext> : MonoBehaviour where TContext : StatefulBehaviour<TContext>
{
    private event Action RelayDisposer;

    private StateMachine<TContext> _stateMachine;

    protected StateMachine<TContext> StateMachine => _stateMachine ??= new StateMachine<TContext>(this as TContext);

    public void Tick(float deltaTime)
    {
        StateMachine.Tick(deltaTime);
    }

    protected void TransitionTo(IState<TContext> state)
    {
        StateMachine.TransitionTo(state);
    }

    protected void Relay<TEvent>() where TEvent : GameEvent
    {
        Action<TEvent> callback = evt => StateMachine.SendEvent(evt);
        GameEventBus.Subscribe(callback);
        RelayDisposer += () => GameEventBus.Unsubscribe(callback);
    }

    protected void CancelRelay()
    {
        RelayDisposer?.Invoke();
        RelayDisposer = null;
    }
}