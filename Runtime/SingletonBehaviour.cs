using UnityEngine;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
    private static T _instance;
    private static bool _isQuitting;

    public static T Instance
    {
        get
        {
            if (_instance == null && !_isQuitting)
            {
                Debug.LogAssertion($"{typeof(T)} has not been instantiated.");
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Debug.LogAssertion($"{typeof(T)} has more than one instances", this);
        }

        _instance = this as T;
        UnityAwake();
    }

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    protected virtual void UnityAwake()
    {
    }
}