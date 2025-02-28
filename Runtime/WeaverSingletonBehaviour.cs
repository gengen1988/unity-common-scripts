using UnityEngine;
using Weaver;

public abstract class WeaverSingletonBehaviour<T> : MonoBehaviour where T : WeaverSingletonBehaviour<T>
{
    // add this to your T:
    // [AssetReference] private static readonly T SingletonPrefab;

    private static T _instance;

    public static T Instance
    {
        get
        {
            // two cases:
            // 1. fresh start
            // 2. re-enter play mode when not reload domain
            if (!_instance)
            {
                var prefab = GetPrefab();
                if (!prefab)
                {
                    Debug.LogAssertion($"please add static SingletonPrefab field to {typeof(T)}");
                    return null;
                }

                _instance = Instantiate(prefab);
                DontDestroyOnLoad(_instance);
                Debug.Log($"Singleton {typeof(T)} Instantiated", _instance);
            }

            return _instance;
        }
    }

    private static T GetPrefab()
    {
        var prefabField = typeof(T).GetField("SingletonPrefab", ReflectionUtilities.StaticBindings);
        var prefab = prefabField?.GetValue(null) as T;
        return prefab;
    }

    public void Touch()
    {
        // just a placeholder method to ensure prefab instantiated 
    }
}