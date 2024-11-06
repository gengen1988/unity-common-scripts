using UnityEngine;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
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
        GameEventBus.Subscribe<OnFrameworkInit>(HandleFrameworkInit);
        UnityAwake();
        DontDestroyOnLoad(this);
    }

    private void OnDestroy()
    {
        GameEventBus.Unsubscribe<OnFrameworkInit>(HandleFrameworkInit);
        UnityOnDestroy();
    }

    protected virtual void UnityAwake()
    {
    }

    protected virtual void UnityOnDestroy()
    {
    }

    protected virtual void HandleFrameworkInit(OnFrameworkInit evt)
    {
    }
}