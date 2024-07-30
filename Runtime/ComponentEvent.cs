using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public static class ComponentEvent<T> where T : class
{
    private static event Action<T> OnComponentEnabled;
    private static event Action<T> OnComponentDisabled;

    public static void NotifyEnabled(T component)
    {
        OnComponentEnabled?.Invoke(component);
    }

    public static void NotifyDisabled(T component)
    {
        OnComponentDisabled?.Invoke(component);
    }

    public static Action Register(IComponentLifespanHandler<T> handler)
    {
        IEnumerable<T> existing = FindObjectsByType(FindObjectsInactive.Exclude);
        foreach (T found in existing)
        {
            handler.OnComponentEnabled(found);
        }

        OnComponentEnabled += handler.OnComponentEnabled;
        OnComponentDisabled += handler.OnComponentDisabled;

        return () =>
        {
            OnComponentEnabled -= handler.OnComponentEnabled;
            OnComponentDisabled -= handler.OnComponentDisabled;

            IEnumerable<T> all = FindObjectsByType(FindObjectsInactive.Include);
            foreach (T found in all)
            {
                handler.OnComponentDisabled(found);
            }
        };
    }

    private static IEnumerable<T> FindObjectsByType(FindObjectsInactive findObjectsInactive)
    {
        Type genericType = typeof(T);
        Type componentType = typeof(Component);
        if (componentType.IsAssignableFrom(genericType))
        {
            // generic type is a component
            // find objects by type directly
            Object[] results = Object.FindObjectsByType(genericType, findObjectsInactive, FindObjectsSortMode.None);
            foreach (Object result in results)
            {
                if (result is T component)
                {
                    yield return component;
                }
            }
        }
        else if (genericType.IsInterface)
        {
            // generic type is an interface
            // find all component types that inherit the interface
            // then find objects by type for each component type
            IEnumerable<Type> objectTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => componentType.IsAssignableFrom(type)) // sub classes of component
                .Where(type => genericType.IsAssignableFrom(type));

            bool typeFound = false;
            foreach (Type objectType in objectTypes)
            {
                Object[] results = Object.FindObjectsByType(objectType, findObjectsInactive, FindObjectsSortMode.None);
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
        }
        else
        {
            throw new InvalidOperationException($"The type {genericType} is neither a Component nor an interface.");
        }
    }
}

public interface IComponentLifespanHandler<in T>
{
    void OnComponentEnabled(T subject);
    void OnComponentDisabled(T subject);
}