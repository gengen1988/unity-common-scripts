using UnityEngine;

// beware: not for domain reloading!!!

/// <summary>
/// Inherit from this base class to create a singleton.
/// e.g. public class MyClassName : MonoBehaviourSingleton<MyClassName> {}
/// </summary>
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    // Check to see if we're about to be destroyed.
    static bool _shuttingDown;
    static readonly object _lock = new object();
    static T _instance;

    /// <summary>
    /// Access singleton instance through this propriety.
    /// </summary>
    public static T instance
    {
        get
        {
            if (_shuttingDown)
            {
                Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed. Returning null.");
                return null;
            }

            lock (_lock)
            {
                if (_instance) return _instance;
                
                // Search for existing instance.
                _instance = FindObjectOfType<T>();
                if (_instance) return _instance;
                
                // Create new instance if one doesn't already exist.
                // Need to create a new GameObject to attach the singleton to.
                var singletonObject = new GameObject($"{typeof(T)} (Singleton)");
                _instance = singletonObject.AddComponent<T>();

                // Make instance persistent.
                DontDestroyOnLoad(singletonObject);
                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (_instance)
        {
            Destroy(gameObject);
            return;
        }
        _instance = GetComponent<T>();
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnApplicationQuit()
    {
        _shuttingDown = true;
    }
    
    protected virtual void OnDestroy()
    {
        _shuttingDown = true;
    }
}
