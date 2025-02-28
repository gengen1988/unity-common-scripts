using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public interface IComponentManager<T>
{
    private static readonly List<IComponentManager<T>> Managers = new();

    public static void NotifyEnabled(T component)
    {
        foreach (var manager in Managers)
        {
            manager.OnComponentEnabled(component);
        }
    }

    public static void NotifyDisabled(T component)
    {
        foreach (var manager in Managers)
        {
            manager.OnComponentDisabled(component);
        }
    }

    public static void RegisterManager(IComponentManager<T> manager)
    {
        var existing = FindObjectsByType(FindObjectsInactive.Exclude);
        foreach (var found in existing)
        {
            manager.OnComponentEnabled(found);
        }

        Managers.Add(manager);
    }

    public static void DeregisterManager(IComponentManager<T> manager)
    {
        Managers.Remove(manager);

        var all = FindObjectsByType(FindObjectsInactive.Include);
        foreach (var found in all)
        {
            manager.OnComponentDisabled(found);
        }
    }

    private static IEnumerable<T> FindObjectsByType(FindObjectsInactive findObjectsInactive)
    {
        var genericType = typeof(T);
        var componentType = typeof(MonoBehaviour);

        if (componentType.IsAssignableFrom(genericType))
        {
            // generic type is a MonoBehaviour
            // find objects by type directly
            // can't use generic method because T may be an interface
            var results = Object.FindObjectsByType(genericType, findObjectsInactive, FindObjectsSortMode.None);
            foreach (var result in results)
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
            var candidates = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => componentType.IsAssignableFrom(type)) // the type must be a subclass of MonoBehaviour
                .Where(type => genericType.IsAssignableFrom(type));

            var typeFound = false;
            foreach (var type in candidates)
            {
                var results = Object.FindObjectsByType(type, findObjectsInactive, FindObjectsSortMode.None);
                foreach (var result in results)
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

    void OnComponentEnabled(T component);
    void OnComponentDisabled(T component);
}