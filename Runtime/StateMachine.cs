public class StateMachine<TContext>
{
    public readonly TContext Context;

    private State<TContext> _currentState;

    public StateMachine(TContext context)
    {
        Context = context;
    }

    public void Tick(float deltaTime)
    {
        _currentState.OnTick(deltaTime);
    }

    public void TransitionTo<TState>() where TState : State<TContext>, new()
    {
        _currentState?.OnExit();
        TState state = new TState();
        state.StateMachine = this;
        _currentState = state;
        state.OnEnter();
    }

    public State<TContext> GetCurrentState()
    {
        return _currentState;
    }
}

public abstract class State<TContext>
{
    public StateMachine<TContext> StateMachine;

    protected TContext Context => StateMachine.Context;

    protected void TransitionTo<TState>() where TState : State<TContext>, new()
    {
        StateMachine.TransitionTo<TState>();
    }

    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
    }

    public virtual void OnTick(float deltaTime)
    {
    }
}