using UnityEngine;

public static class StateMachine
{
    public static StateMachine<TContext> CreateInstance<TContext>(TContext context)
    {
        return new StateMachine<TContext>(context);
    }
}

public class StateMachine<TContext>
{
    private readonly TContext _context;

    private IState<TContext> _previousState;
    private IState<TContext> _currentState;

    public StateMachine(TContext context)
    {
        _context = context;
    }

    public void Tick(float deltaTime)
    {
        Debug.Assert(_currentState != null, "current state is null");
        _currentState?.OnTick(_context, deltaTime);
    }

    public void TransitionTo(IState<TContext> nextState)
    {
        _currentState?.OnExit(_context);
        _previousState = _currentState;
        _currentState = nextState;
        nextState?.OnEnter(_context);
    }

    public void RevertToPreviousState()
    {
        TransitionTo(_previousState);
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

    public void SendMessage(string message)
    {
        IState<TContext> nextState = _currentState?.OnMessage(message);
        if (nextState != null)
        {
            TransitionTo(nextState);
        }
    }
}

public interface IState<in TContext>
{
    public void OnEnter(TContext context);
    public void OnExit(TContext context);
    public void OnTick(TContext context, float deltaTime);
    public IState<TContext> OnMessage(string message);
}

public abstract class State<TState, TContext> : Singleton<TState>, IState<TContext> where TState : class, new()
{
    public virtual void OnEnter(TContext context)
    {
    }

    public virtual void OnExit(TContext context)
    {
    }

    public virtual void OnTick(TContext context, float deltaTime)
    {
    }

    public virtual IState<TContext> OnMessage(string message)
    {
        return null;
    }
}