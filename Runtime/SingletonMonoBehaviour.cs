using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
    private static T _instance;

    public static T Instance => _instance;

    private void Awake()
    {
        Debug.Assert(Instance == null || Instance == this, $"{typeof(T).Name} has more than one instances", this);
        _instance = this as T;
        OnAwake();
    }

    protected virtual void OnAwake()
    {
    }
}