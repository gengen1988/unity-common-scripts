using System;
using UnityEngine;

public abstract class StatefulBehaviour<TContext, TStartState> : MonoBehaviour
	where TStartState : State<TContext>, new()
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
			Debug.LogError($"{type} should inherit StatefulBehaviour<{type}, TStartState>", this);
		}

		_stateMachine.TransitionTo<TStartState>();
	}

	protected virtual void Update()
	{
		_stateMachine.Tick();
	}
}