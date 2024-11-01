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

    protected override void AfterAwake()
    {
        // register systems on this gameObject
        RegisterGameObject(gameObject);
    }

    public static void RegisterGameObject(GameObject go)
    {
        if (!Instance)
        {
            Debug.LogError("not ready");
            return;
        }

        Dictionary<Type, Component> index = Instance._systemByType;
        MonoBehaviour[] behaviours = go.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour behaviour in behaviours)
        {
            Type systemType = behaviour.GetType();
            if (index.ContainsKey(systemType))
            {
                Debug.LogError($"{systemType} should not has more than one instance", go);
            }
            else
            {
                index[systemType] = behaviour;
            }
        }
    }

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
}