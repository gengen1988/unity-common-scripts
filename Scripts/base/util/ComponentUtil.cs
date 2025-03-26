using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class ComponentUtil
{
    [MenuItem("GameObject / Tools / Sort Components")]
    private static void SortComponents(MenuCommand menuCommand)
    {
        throw new NotImplementedException();
        var gameObject = menuCommand.context as GameObject;
        if (!gameObject)
        {
            return;
        }

        var components = gameObject.GetComponents<Component>();
        // components.Select()
    }

    public static T OptionalComponent<T>(this Component src) where T : Component
    {
        if (src == null)
        {
            return null;
        }

        if (src.TryGetComponent(out T result))
        {
            return result;
        }

        return null;
    }

    public static T OptionalComponent<T>(this GameObject src) where T : Component
    {
        if (src == null)
        {
            return null;
        }

        if (!src.TryGetComponent(out T result))
        {
            return result;
        }

        return null;
    }

    public static T EnsureComponent<T>(this Component src, bool addAtRuntime = false) where T : Component
    {
        if (!src)
        {
            Debug.LogWarning("src is null");
            return null;
        }

        if (src is T result)
        {
            return result;
        }

        if (src.TryGetComponent(out T found))
        {
            return found;
        }

        if (addAtRuntime)
        {
            return AddComponentSafe<T>(src.gameObject);
        }

        Debug.LogAssertion($"please add a {typeof(T)} to {src}'s prefab", src);
        return AddComponentSafe<T>(src.gameObject);
    }

    public static T EnsureComponent<T>(this GameObject src, bool addAtRuntime = false) where T : Component
    {
        if (!src)
        {
            Debug.LogWarning("src is null");
            return null;
        }

        if (src.TryGetComponent(out T found))
        {
            return found;
        }

        if (addAtRuntime)
        {
            return AddComponentSafe<T>(src);
        }

        Debug.LogAssertion($"please add a {typeof(T)} to {src}'s prefab", src);
        return AddComponentSafe<T>(src);
    }

    private static T AddComponentSafe<T>(GameObject src) where T : Component
    {
#if UNITY_EDITOR
        // this method may provide more detailed exception
        // such as instantiate an abstract class.
        return ObjectFactory.AddComponent<T>(src);
#else
        // this method will fail silence
        return src.AddComponent<T>();
#endif
    }

    public static T[] GetDirectComponents<T>(this Component manager, bool includeInactive = false)
    {
        if (!manager)
        {
            return Array.Empty<T>();
        }

        var managerType = manager.GetType();
        return manager.GetComponentsInChildren<T>(includeInactive)
            .Cast<Component>()
            .Where(c => c != manager)
            .Where(c => manager == c.GetComponentInParent(managerType, true))
            .Cast<T>()
            .ToArray();
    }
}