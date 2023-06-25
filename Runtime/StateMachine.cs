public class StateMachine<TContext>
{
	public TContext Context;

	private State<TContext> _currentState;

	public void Tick()
	{
		_currentState.Tick();
	}

	public void TransitionTo<TState>() where TState : State<TContext>, new()
	{
		_currentState?.Exit();
		TState state = new TState();
		state.StateMachine = this;
		_currentState = state;
		state.Enter();
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

	public virtual void Enter()
	{
	}

	public virtual void Exit()
	{
	}

	public virtual void Tick()
	{
	}
}