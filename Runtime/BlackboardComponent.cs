using System;
using UnityEngine;

public abstract class BlackboardComponent<TKey> : MonoBehaviour where TKey : struct, Enum
{
    private Blackboard<TKey> _store = new();

    public TValue GetValue<TValue>(TKey key)
    {
        return _store.GetValue<TValue>(key);
    }

    public void SetValue<TValue>(TKey key, TValue value)
    {
        _store.SetValue(key, value);
    }
}