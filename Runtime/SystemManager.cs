using System;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : SingletonBehaviour<SystemManager>
{
    private static class Cache<T> where T : Component
    {
        public static T Value;
    }

    private readonly Dictionary<Type, Component> _systemByType = new();

    public static T GetSystem<T>() where T : Component
    {
        if (!Instance)
        {
            Debug.LogError("no system manager instance found");
            return null;
        }

        // cache to bypass alloc
        T cachedValue = Cache<T>.Value;
        if (cachedValue)
        {
            return cachedValue;
        }

        // refresh cache
        if (!Instance._systemByType.TryGetValue(typeof(T), out Component spawned))
        {
            Debug.LogError($"someone require {typeof(T)} to work");
        }

        T newValue = spawned as T;
        Cache<T>.Value = newValue;
        return newValue;
    }

    public static void CreateSystem(GameObject prefab)
    {
        if (!Instance)
        {
            Debug.LogError("no system manager instance found");
            return;
        }

        GameObject go = Instantiate(prefab, Instance.transform);
        MonoBehaviour[] components = go.GetComponents<MonoBehaviour>();
        Component system = components[0];
        Type systemType = system.GetType();
        if (components.Length > 1)
        {
            Debug.LogWarning($"only index {systemType} on {prefab}", prefab);
        }

        Debug.Assert(
            !Instance._systemByType.ContainsKey(systemType),
            $"{systemType} should not has more than one instance",
            prefab
        );
        Instance._systemByType[systemType] = system;
    }
}