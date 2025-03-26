public class StateMachine<TContext>
{
    private readonly TContext _context;

    private IState<TContext> _currentState;
    private IState<TContext> _previousState;

    public IState<TContext> CurrentState
    {
        get => _currentState;
        set => _currentState = value;
    }

    public StateMachine(TContext context)
    {
        _context = context;
    }

    public void TransitionTo(IState<TContext> nextState)
    {
        var currentState = _currentState;
        _currentState = nextState;
        _previousState = currentState;
        currentState?.OnExit(_context);
        nextState?.OnEnter(_context);
    }

    public void RevertBack()
    {
        TransitionTo(_previousState);
    }

    public void Tick(float deltaTime)
    {
        _currentState?.OnRefresh(_context, deltaTime);
    }

    public void Send(string message)
    {
        _currentState?.OnMessage(_context, message);
    }

    public void SetPreviousState(IState<TContext> state)
    {
        _previousState = state;
    }
}

public interface IState<in TContext>
{
    public void OnEnter(TContext ctx);
    public void OnExit(TContext ctx);
    public void OnRefresh(TContext ctx, float deltaTime);
    public void OnMessage(TContext ctx, string message);
}

// do not use field in instance because it is singleton.
// use context instead
public abstract class State<TContext, TState> : Singleton<TState>, IState<TContext>
    where TState : Singleton<TState>, new()
{
    public virtual void OnEnter(TContext ctx)
    {
    }

    public virtual void OnExit(TContext ctx)
    {
    }

    public virtual void OnRefresh(TContext ctx, float deltaTime)
    {
    }

    public virtual void OnMessage(TContext ctx, string message)
    {
    }
}