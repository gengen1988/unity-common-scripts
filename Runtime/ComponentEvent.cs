using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public interface IComponentManager<T>
{
    private static event Action<T> OnEnabled;
    private static event Action<T> OnDisabled;

    public static void NotifyEnabled(T component)
    {
        OnEnabled?.Invoke(component);
    }

    public static void NotifyDisabled(T component)
    {
        OnDisabled?.Invoke(component);
    }

    public static void RegisterManager(IComponentManager<T> manager)
    {
        IEnumerable<T> existing = FindObjectsByType(FindObjectsInactive.Exclude);
        foreach (T found in existing)
        {
            manager.OnComponentEnabled(found);
        }

        OnEnabled += manager.OnComponentEnabled;
        OnDisabled += manager.OnComponentDisabled;
    }

    public static void DeregisterManager(IComponentManager<T> manager)
    {
        OnEnabled -= manager.OnComponentEnabled;
        OnDisabled -= manager.OnComponentDisabled;

        IEnumerable<T> all = FindObjectsByType(FindObjectsInactive.Include);
        foreach (T found in all)
        {
            manager.OnComponentDisabled(found);
        }
    }

    private static IEnumerable<T> FindObjectsByType(FindObjectsInactive findObjectsInactive)
    {
        Type genericType = typeof(T);
        Type behaviourType = typeof(MonoBehaviour);

        if (behaviourType.IsAssignableFrom(genericType))
        {
            // generic type is a MonoBehaviour
            // find objects by type directly
            // can't use generic method because T may be an interface
            Object[] results = Object.FindObjectsByType(genericType, findObjectsInactive, FindObjectsSortMode.None);
            foreach (Object result in results)
            {
                if (result is T component)
                {
                    yield return component;
                }
            }

            yield break;
        }

        if (genericType.IsInterface)
        {
            // generic type is an interface
            // find all component types that inherit the interface
            // then find objects by type for each component type
            IEnumerable<Type> candidates = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => behaviourType.IsAssignableFrom(type)) // the type must be a sub class of MonoBehaviour
                .Where(type => genericType.IsAssignableFrom(type));

            bool typeFound = false;
            foreach (Type type in candidates)
            {
                Object[] results = Object.FindObjectsByType(type, findObjectsInactive, FindObjectsSortMode.None);
                foreach (Object result in results)
                {
                    if (result is T component)
                    {
                        yield return component;
                    }
                }

                typeFound = true;
            }

            if (!typeFound)
            {
                Debug.LogWarning($"no component inherit {genericType}");
            }

            yield break;
        }

        throw new InvalidOperationException($"The type {genericType} is neither a Component nor an interface.");
    }

    public void OnComponentEnabled(T component);
    public void OnComponentDisabled(T component);
}