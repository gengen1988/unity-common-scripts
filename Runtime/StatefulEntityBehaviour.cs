using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(GameEntityBridge))]
public abstract class StatefulEntityBehaviour<TContext> : MonoBehaviour, IEntityAttach, IEntityDetach, IEntityFrame
    where TContext : StatefulEntityBehaviour<TContext>
{
    private GameEntityBridge _bridge;
    private StateMachine<TContext> _stateMachine;

    [ShowInInspector, ReadOnly] // for debug
    private string CurrentState => _stateMachine?.CurrentState?.ToString();
    public StateMachine<TContext> StateMachine => _stateMachine ??= new StateMachine<TContext>(this as TContext);

    protected abstract IState<TContext> InitialState { get; }

    public float LocalDeltaTime => _bridge.Clock.LocalDeltaTime;
    public float UnscaledDeltaTime => _bridge.Clock.UnscaledDeltaTime;

    public void OnEntityAttach(GameEntity entity)
    {
        _bridge = entity.GetBridge();
        StateMachine.TransitionTo(InitialState);
    }

    public void OnEntityDetach(GameEntity entity)
    {
        StateMachine.TransitionTo(null);
    }

    public void OnEntityFrame(GameEntity entity)
    {
        StateMachine.Refresh();
    }

    protected void TransitionTo(IState<TContext> state)
    {
        StateMachine.TransitionTo(state);
    }

    protected void RegisterTransition(string msg, IState<TContext> state)
    {
        StateMachine.RegisterTransition(msg, state);
    }

    protected void Send(string msg)
    {
        StateMachine.Send(msg);
    }
}