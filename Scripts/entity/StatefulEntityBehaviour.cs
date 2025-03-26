using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(EntityProxy))]
public abstract class StatefulEntityBehaviour<TContext> : MonoBehaviour, IEntityAttach, IEntityDetach, IEntityFrame
    where TContext : StatefulEntityBehaviour<TContext>
{
    private StateMachine<TContext> _fsm;

    [ShowInInspector, ReadOnly] // for debug
    private string CurrentState => _fsm?.CurrentState?.ToString();
    protected StateMachine<TContext> FSM => _fsm ??= new StateMachine<TContext>(this as TContext);

    protected abstract IState<TContext> InitialState { get; }

    public void OnEntityAttach(GameEntity entity)
    {
        _fsm.TransitionTo(InitialState);
    }

    public void OnEntityDetach(GameEntity entity)
    {
        _fsm.TransitionTo(null);
    }

    public void OnEntityFrame(GameEntity entity, float deltaTime)
    {
        _fsm.Tick(deltaTime);
    }
}