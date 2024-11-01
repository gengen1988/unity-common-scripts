using UnityEngine;

public abstract class StatefulBehaviour<T> : MonoBehaviour where T : StatefulBehaviour<T>
{
    private StateMachine<T> _stateMachine;
    public StateMachine<T> StateMachine => _stateMachine ??= new StateMachine<T>(this as T);
}