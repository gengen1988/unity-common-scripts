using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class UnityUtil
{
    public static GameObject FindPlayer()
    {
        return GameObject.FindWithTag("Player");
    }

    public static Vector2 GetInputVector()
    {
        Vector2 vector = Vector2.zero;
#if ENABLE_INPUT_SYSTEM
        UnityEngine.InputSystem.Keyboard keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed)
            {
                vector.y += 1;
            }

            if (keyboard.sKey.isPressed)
            {
                vector.y -= 1;
            }

            if (keyboard.aKey.isPressed)
            {
                vector.x -= 1;
            }

            if (keyboard.dKey.isPressed)
            {
                vector.x += 1;
            }
        }
#else
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        vector = new Vector2(h, v);
#endif
        return vector;
    }

    /**
     * 取得鼠标在世界座标的位置。兼容 InputSystem
     */
    public static Vector2 GetMousePositionWorld()
    {
        Vector2 mousePosition;

#if ENABLE_INPUT_SYSTEM
        UnityEngine.InputSystem.Mouse currentMouse = UnityEngine.InputSystem.Mouse.current;
        if (currentMouse == null)
        {
            return Vector2.zero;
        }

        mousePosition = currentMouse.position.ReadValue();
#else
        mousePosition = Input.mousePosition;
#endif

        Camera mainCamera = Camera.main;
        if (!mainCamera)
        {
            return mousePosition;
        }

        if (!mainCamera.orthographic)
        {
            Debug.LogAssertion("MainCamera is not orthographic");
            return mousePosition;
        }

        return mainCamera.ScreenToWorldPoint(mousePosition);
    }

    public static Vector2 GetMousePositionDelta()
    {
        float x = Input.GetAxisRaw("Mouse X");
        float y = Input.GetAxisRaw("Mouse Y");
        return new Vector2(x, y);
    }

    /**
     * 取得 transform 在场景中的路径
     */
    public static string GetAbsolutePath(this Transform transform)
    {
        if (transform.parent == null)
        {
            return $"/{transform.name}";
        }

        return $"{GetAbsolutePath(transform.parent)}/{transform.name}";
    }

    /**
     * 确保一定有特定名称的 child
     */
    public static Transform EnsureChild(this Transform root, string childName)
    {
        Transform child;
        if (root)
        {
            child = root.Find(childName);
        }
        else
        {
            GameObject go = GameObject.Find(childName);
            child = go && !go.transform.parent ? go.transform : null;
        }

        if (!child)
        {
            GameObject go = new GameObject(childName);
            child = go.transform;
            if (root)
            {
                child.SetParent(root, false);
            }
        }

        return child;
    }

    public static T EnsureComponent<T>(this GameObject go) where T : Component
    {
        if (!go.TryGetComponent(out T result))
        {
#if UNITY_EDITOR
            // this method may provide more detailed exception, instead of fail silence,
            // such as instantiate an abstract class.
            result = ObjectFactory.AddComponent<T>(go);
#else
            result = go.AddComponent<T>();
#endif
        }

        return result;
    }

    public static T EnsureComponent<T>(this Component component) where T : Component
    {
        if (!component)
        {
            return null;
        }

        if (component is T result)
        {
            return result;
        }

        return EnsureComponent<T>(component.gameObject);
    }

//     public static void EnsureComponent<T>(
//         this GameObject root,
//         ref T component,
//         bool addIfNotExists = false) where T : Component
//     {
//         if (!root)
//         {
//             Debug.LogError("gameObject not exists");
//             return;
//         }
//
//         if (component && component.gameObject == root)
//         {
//             return;
//         }
//
//         if (root.TryGetComponent(out T found))
//         {
//             component = found;
//             return;
//         }
//
//         if (addIfNotExists)
//         {
// #if UNITY_EDITOR
//             // this method may provide more detailed exception instead of fail silence,
//             // such as instantiate an abstract class.
//             component = ObjectFactory.AddComponent<T>(root);
// #else
//             component = root.AddComponent<T>();
// #endif
//             return;
//         }
//
//         Debug.LogError($"{typeof(T)} not found on {root}", root);
//     }
//
//     public static void EnsureComponentInParent<T>(this GameObject searchFrom, ref T component) where T : Component
//     {
//         if (!searchFrom)
//         {
//             Debug.LogError("gameObject not exists");
//             return;
//         }
//
//         if (component && searchFrom.transform.IsChildOf(component.transform))
//         {
//             return;
//         }
//
//         T found = searchFrom.GetComponentInParent<T>();
//         if (found)
//         {
//             component = found;
//             return;
//         }
//
//         Debug.LogError($"{typeof(T)} not found above {searchFrom}", searchFrom);
//     }

    public static T[] GetAttachedComponents<T>(this Component manager, bool includeInactive = false)
    {
        if (!manager)
        {
            return Array.Empty<T>();
        }

        Type managerType = manager.GetType();
        return manager.GetComponentsInChildren<T>(includeInactive)
            .Cast<Component>()
            .Where(c => c != manager)
            .Where(c => manager == c.GetComponentInParent(managerType, true))
            .Cast<T>()
            .ToArray();
    }

    /**
     * 清理 transform 下的子物体
     */
    public static void DestroyChildren(this Transform root)
    {
        if (Application.isPlaying)
        {
            foreach (Transform child in root)
            {
                Object.Destroy(child.gameObject);
            }
        }
        else
        {
            // 在编辑阶段里只能用 DestroyImmediate，而且 DestroyImmediate 用 foreach 会漏删
            while (root.childCount > 0)
            {
                Transform target = root.GetChild(0);
                Object.DestroyImmediate(target.gameObject);
            }
        }
    }

    public static IEnumerable<Transform> Children(this Transform transform)
    {
        return transform.Cast<Transform>();
    }

    /**
     * move position in camera space ui
     */
    public static Vector3 TranslateWithCamera(Vector3 origin, Vector3 delta, Camera translateCamera)
    {
        Vector3 currentScreenPoint = translateCamera.WorldToScreenPoint(origin);
        Vector3 newWorldPoint = translateCamera.ScreenToWorldPoint(currentScreenPoint + delta);
        return newWorldPoint;
    }

    public static Vector3 CameraRemap(Vector3 fromWorldPosition, Camera from, Camera to)
    {
        Vector3 screenPoint = from.WorldToScreenPoint(fromWorldPosition);
        Vector3 remapped = to.ScreenToWorldPoint(screenPoint);
        return remapped;
    }

    /**
     * can be used to determine two RectTransform intersect
     */
    public static Rect GetScreenRect(RectTransform trans)
    {
        Vector3 origin = trans.position;
        Vector2 pivot = trans.pivot;

        // Calculate rect taking into account its scale
        Vector2 size = Vector2.Scale(trans.rect.size, trans.lossyScale);
        Rect rect = new Rect(origin.x, origin.y, size.x, size.y);

        // Adjust the x and y coordinates of the Rect based on the pivot point
        rect.x -= pivot.x * size.x;
        rect.y -= pivot.y * size.y;
        return rect;
    }

    public static bool Intersects(RectTransform trans1, RectTransform trans2)
    {
        Rect rect1 = GetScreenRect(trans1);
        Rect rect2 = GetScreenRect(trans2);
        return rect1.Overlaps(rect2);
    }

    public static bool Contains(RectTransform outer, RectTransform inner)
    {
        Rect outerRect = GetScreenRect(outer);
        Rect innerRect = GetScreenRect(inner);
        return outerRect.Contains(innerRect.min) && outerRect.Contains(innerRect.max);
    }

    // public static IEnumerable<T> WithAlive<T>(this IEnumerable<T> collection) where T : Object
    // {
    //     return collection.Where(entry =>
    //     {
    //         return entry switch
    //         {
    //             GameObject go => PoolWrapper.IsAlive(go),
    //             Component component => PoolWrapper.IsAlive(component),
    //             _ => entry
    //         };
    //     });
    // }

    // public static IEnumerable<Collider2D> FindSiblingNearby(
    //     Transform self,
    //     float radius,
    //     ContactFilter2D contactFilter,
    //     List<Collider2D> buffer)
    // {
    //     Vector2 searchOrigin = self.position;
    //     Physics2D.OverlapCircle(searchOrigin, radius, contactFilter, buffer);
    //     return buffer
    //         .Where(found => IsSibling(self, found.transform))
    //         .OrderBy(found => (found.ClosestPoint(searchOrigin) - searchOrigin).sqrMagnitude);
    // }
    //
    public static bool IsSibling(Transform trans1, Transform trans2)
    {
        if (!trans1 || !trans2)
        {
            return false;
        }

        return !trans1.IsChildOf(trans2) && !trans2.IsChildOf(trans1);
    }

    public static IEnumerable<Transform> TraverseTransform(Transform root, bool deep)
    {
        if (!root)
        {
            yield break;
        }

        foreach (Transform child in root)
        {
            yield return child;
            if (!deep)
            {
                continue;
            }

            foreach (Transform childOfChild in TraverseTransform(child, true))
            {
                yield return childOfChild;
            }
        }
    }
}