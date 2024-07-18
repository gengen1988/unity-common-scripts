using System;
using System.Collections;
using UnityEngine;

public class SystemManager : SingletonMonoBehaviour<SystemManager>
{
    private static class SystemCache<T>
    {
        public static T Value;
    }

    private readonly Hashtable _systemByType = new();

    public static T GetSystem<T>() where T : Component
    {
        if (!Instance)
        {
            Debug.LogError("no system manager instance found");
            return null;
        }

        // cache to bypass alloc
        T cachedValue = SystemCache<T>.Value;
        if (cachedValue)
        {
            return cachedValue;
        }

        // update cache
        T newValue = Instance.FindSystemByType<T>();
        SystemCache<T>.Value = newValue;
        return newValue;
    }

    public static void CreateSystem(GameObject prefab)
    {
        if (!Instance)
        {
            Debug.LogError("no system manager instance found");
            return;
        }

        Instance.SpawnSystem(prefab);
    }

    private T FindSystemByType<T>() where T : Component
    {
        Type systemType = typeof(T);
        T result = (T)_systemByType[systemType];
        if (!result)
        {
            Debug.LogError($"some one require {nameof(T)} to work", this);
            return null;
        }

        return result;
    }

    private void SpawnSystem(GameObject prefab)
    {
        GameObject go = Instantiate(prefab, transform);
        ISystem[] components = go.GetComponents<ISystem>();
        Debug.Assert(components.Length == 1, "please consider put systems into individual prefabs", prefab);
        ISystem system = components[0];
        Type systemType = system.GetType();
        Debug.Assert(!_systemByType.ContainsKey(systemType), "system should not has more than one instance", prefab);
        _systemByType[systemType] = system;
    }
}