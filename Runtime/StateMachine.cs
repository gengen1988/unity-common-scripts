public class StateMachine<TContext>
{
    private readonly TContext _context;

    private IState<TContext> _previousState;
    private IState<TContext> _currentState;

    public StateMachine(TContext context)
    {
        _context = context;
    }

    public void TransitionTo(IState<TContext> nextState)
    {
        _currentState?.OnExit(_context);
        _previousState = _currentState;
        _currentState = nextState;
        nextState?.OnEnter(_context);
    }

    public void RevertBack()
    {
        TransitionTo(_previousState);
    }

    public void SendMessage(string message = null)
    {
        _currentState?.OnMessage(_context, message);
    }

    public IState<TContext> GetCurrentState()
    {
        return _currentState;
    }

    public void SetCurrentState(IState<TContext> state)
    {
        _currentState = state;
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
    public void OnMessage(TContext ctx, string message);
}

public abstract class State<TContext, TState> : Singleton<TState>, IState<TContext> where TState : class, new()
{
    public virtual void OnEnter(TContext ctx)
    {
    }

    public virtual void OnExit(TContext ctx)
    {
    }

    public virtual void OnMessage(TContext ctx, string message)
    {
    }
}