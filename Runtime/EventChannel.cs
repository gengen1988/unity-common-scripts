using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Event Channel / notifier")]
public class EventChannel : ScriptableObject
{
    event Action channel;

    public void On(Action action)
    {
        channel += action;
    }

    public void Off(Action action)
    {
        channel -= action;
    }

    public void Emit()
    {
        channel?.Invoke();
    }
}

public class EventChannel<T1> : ScriptableObject
{
    event Action<T1> channel;

    public void On(Action<T1> action)
    {
        channel += action;
    }

    public void Off(Action<T1> action)
    {
        channel -= action;
    }

    public void Emit(T1 arg)
    {
        channel?.Invoke(arg);
    }
}

public class EventChannel<T1, T2> : ScriptableObject
{
    event Action<T1, T2> channel;

    public void On(Action<T1, T2> action)
    {
        channel += action;
    }

    public void Off(Action<T1, T2> action)
    {
        channel -= action;
    }

    public void Emit(T1 arg1, T2 arg2)
    {
        channel?.Invoke(arg1, arg2);
    }
}

public class EventChannel<T1, T2, T3> : ScriptableObject
{
    event Action<T1, T2, T3> channel;

    public void On(Action<T1, T2, T3> action)
    {
        channel += action;
    }

    public void Off(Action<T1, T2, T3> action)
    {
        channel -= action;
    }

    public void Emit(T1 arg1, T2 arg2, T3 arg3)
    {
        channel?.Invoke(arg1, arg2, arg3);
    }
}