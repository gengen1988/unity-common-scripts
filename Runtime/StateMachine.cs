using System.Collections.Generic;

public class StateMachine<TContext>
{
    private readonly TContext _context;

    // private IState<TContext> _globalState;
    private IState<TContext> _currentState;
    private IState<TContext> _previousState;

    private readonly Dictionary<string, IState<TContext>> _transitionTable = new();

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
        _transitionTable.Clear();
        currentState?.OnExit(_context);
        nextState?.OnEnter(_context);
    }

    public void RevertBack()
    {
        TransitionTo(_previousState);
    }

    public void Refresh()
    {
        // _globalState?.OnRefresh(_context);
        _currentState?.OnRefresh(_context);
    }

    public void Send(string message)
    {
        if (!_transitionTable.TryGetValue(message, out var state))
        {
            return;
        }

        TransitionTo(state);
    }

    public void SetPreviousState(IState<TContext> state)
    {
        _previousState = state;
    }

    // public void SetGlobalState(IState<TContext> nextState)
    // {
    //     var currentState = _globalState;
    //     _globalState = nextState;
    //     currentState?.OnExit(_context);
    //     nextState?.OnEnter(_context);
    // }

    public void RegisterTransition(string message, IState<TContext> state)
    {
        _transitionTable.Add(message, state);
    }
}

public interface IState<in TContext>
{
    void OnEnter(TContext ctx);
    void OnExit(TContext ctx);
    void OnRefresh(TContext ctx);
}

public abstract class State<TContext, TState> : Singleton<TState>, IState<TContext>
    where TState : Singleton<TState>, new()
{
    public virtual void OnEnter(TContext ctx)
    {
    }

    public virtual void OnExit(TContext ctx)
    {
    }

    public virtual void OnRefresh(TContext ctx)
    {
    }
}