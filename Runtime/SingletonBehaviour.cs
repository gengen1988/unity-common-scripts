using UnityEngine;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
    private static T _instance;

    public static T Instance => _instance;

    private void Awake()
    {
        Debug.Assert(_instance == null || _instance == this, $"{typeof(T)} has more than one instances", this);
        _instance = this as T;
        OnAwake();
    }

    protected virtual void OnAwake()
    {
    }
}