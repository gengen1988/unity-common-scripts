using System;
using UnityEngine;

public abstract class StatefulBehaviour<TContext, TState> : MonoBehaviour where TState : State<TContext>, new()
{
	private readonly StateMachine<TContext> _stateMachine;

	protected StatefulBehaviour()
	{
		_stateMachine = new StateMachine<TContext>();
		if (this is TContext context)
		{
			_stateMachine.Context = context;
		}
		else
		{
			Type type = GetType();
			Debug.LogError($"{type} should inherit StatefulBehaviour<{type}, StartState>");
		}

		_stateMachine.TransitionTo<TState>();
	}

	protected virtual void Update()
	{
		_stateMachine.Tick();
	}

	protected void TransitionTo<TStateNext>() where TStateNext : State<TContext>, new()
	{
		_stateMachine.TransitionTo<TStateNext>();
	}
}