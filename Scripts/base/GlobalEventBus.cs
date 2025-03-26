using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public interface IGlobalEvent : IEvent
{
}

public static class GlobalEventBus<TEvent> where TEvent : IGlobalEvent
{
    private static readonly EventBus<TEvent> Instance = new();

    public static void Register(IEventBinding<TEvent> binding)
    {
        Instance.Register(binding);
    }

    public static void Deregister(IEventBinding<TEvent> binding)
    {
        Instance.Deregister(binding);
    }

    public static void Raise(TEvent evt)
    {
        Instance.Raise(evt);
    }

    private static void Clear()
    {
        Instance.Clear();
    }
}

/// <summary>
/// Utility class that manages the lifecycle of all event buses in the application.
/// Handles initialization, discovery of event types, and cleanup of event buses.
/// </summary>
public static class GlobalEventBusUtil
{
    private static IReadOnlyList<Type> EventTypes { get; set; }
    private static IReadOnlyList<Type> EventBusTypes { get; set; }

#if UNITY_EDITOR
    private static PlayModeStateChange PlayModeState { get; set; }

    /// <summary>
    /// Initializes the event system in the editor
    /// </summary>
    [InitializeOnLoadMethod]
    private static void InitializeEditor()
    {
        EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
        EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
    }

    /// <summary>
    /// Handles cleanup when exiting play mode in the editor
    /// </summary>
    private static void HandlePlayModeStateChanged(PlayModeStateChange state)
    {
        PlayModeState = state;
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            ClearAllBuses();
        }
    }
#endif

    /// <summary>
    /// Initializes the event system at runtime before any scene is loaded
    /// Discovers all event types and creates their corresponding buses
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        EventTypes = GetDerivedTypes(typeof(IGlobalEvent));
        EventBusTypes = InitializeAllBuses();
    }

    /// <summary>
    /// Creates all event buses for discovered event types
    /// </summary>
    /// <returns>List of created event bus types</returns>
    private static List<Type> InitializeAllBuses()
    {
        var eventBusTypes = new List<Type>();
        var genericBusType = typeof(GlobalEventBus<>);
        foreach (var eventType in EventTypes)
        {
            var busType = genericBusType.MakeGenericType(eventType);
            eventBusTypes.Add(busType);
            Debug.Log($"initialize {busType}");
        }

        return eventBusTypes;
    }

    /// <summary>
    /// Clears all event buses to prevent memory leaks and stale event handlers
    /// </summary>
    private static void ClearAllBuses()
    {
        foreach (var busType in EventBusTypes)
        {
            var clearMethod = busType.GetMethod("Clear", BindingFlags.Static | BindingFlags.NonPublic);
            clearMethod?.Invoke(null, null);
            Debug.Log($"{busType} cleared");
        }
    }

    /// <summary>
    /// Discovers all types that implement the specified base type
    /// Used to find all event types in the application
    /// </summary>
    /// <param name="baseType">The base type to search for (IEvent)</param>
    /// <returns>List of discovered types</returns>
    private static List<Type> GetDerivedTypes(Type baseType)
    {
        var derivedTypes = new List<Type>();
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            if (!assembly.FullName.StartsWith("Assembly-CSharp"))
            {
                continue;
            }

            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (baseType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                {
                    derivedTypes.Add(type);
                }
            }
        }

        return derivedTypes;
    }
}