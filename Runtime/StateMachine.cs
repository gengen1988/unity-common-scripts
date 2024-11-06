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

    public void Tick(float deltaTime)
    {
        _currentState?.Tick(_context, deltaTime);
    }

    public void SendEvent<T>(T gameEvent) where T : GameEvent
    {
        _currentState?.OnEvent(_context, gameEvent);
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
    void OnEnter(TContext ctx);
    void OnExit(TContext ctx);
    void OnEvent(TContext ctx, GameEvent evt);
    void Tick(TContext ctx, float deltaTime);
}

public abstract class State<TContext, TState> : Singleton<TState>, IState<TContext> where TState : Singleton<TState>, new()
{
    public virtual void OnEnter(TContext ctx)
    {
    }

    public virtual void OnExit(TContext ctx)
    {
    }

    public virtual void OnEvent(TContext ctx, GameEvent evt)
    {
    }

    public virtual void Tick(TContext ctx, float deltaTime)
    {
    }
}