using System.Collections.Generic;
using System.Linq;
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
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        return new Vector2(h, v);
    }

    /**
     * 取得鼠标在世界座标的位置。兼容 InputSystem
     */
    public static Vector2 GetMouseWorldPosition()
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

        return mainCamera.ScreenToWorldPoint(mousePosition);
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
     * 确保一定有该名称的 child
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

    public static void EnsureComponent<T>(
        this GameObject root,
        ref T component,
        bool addIfNotExists = false) where T : Component
    {
        if (!root)
        {
            Debug.LogError("gameObject not exists");
            return;
        }

        if (component && component.gameObject == root)
        {
            return;
        }

        if (root.TryGetComponent(out T found))
        {
            component = found;
            return;
        }

        if (addIfNotExists)
        {
            component = root.AddComponent<T>();
            return;
        }

        Debug.LogError($"{typeof(T)} not found on {root}", root);
    }

    public static void EnsureComponentInParent<T>(this GameObject searchFrom, ref T component) where T : Component
    {
        if (!searchFrom)
        {
            Debug.LogError("gameObject not exists");
            return;
        }

        if (component && searchFrom.transform.IsChildOf(component.transform))
        {
            return;
        }

        T found = searchFrom.GetComponentInParent<T>();
        if (found)
        {
            component = found;
            return;
        }

        Debug.LogError($"{typeof(T)} not found above {searchFrom}", searchFrom);
    }

    /**
     * 清理 transform 下的子物体
     */
    public static void DestroyChildren(this Transform root)
    {
        if (Application.isPlaying)
        {
            foreach (Transform child in root.Children())
            {
                Object.Destroy(child.gameObject);
            }
        }
        else
        {
            // 在编辑阶段里只能用 DestroyImmediate，而且 DestroyImmediate 用 foreach 会导致漏删
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

    public static IEnumerable<T> WithExists<T>(this IEnumerable<T> collection) where T : Object
    {
        return collection.Where(entry => entry);
    }

    public static IEnumerable<Collider2D> FindSiblingNearby(
        Transform self,
        float radius,
        ContactFilter2D contactFilter,
        List<Collider2D> buffer)
    {
        Vector2 searchOrigin = self.position;
        Physics2D.OverlapCircle(searchOrigin, radius, contactFilter, buffer);
        return buffer
            .Where(found => IsSibling(self, found.transform))
            .OrderBy(col => (col.ClosestPoint(searchOrigin) - searchOrigin).sqrMagnitude);
    }

    public static bool IsSibling(Transform trans1, Transform trans2)
    {
        if (!trans1 || !trans2)
        {
            return false;
        }

        return !trans1.IsChildOf(trans2) && !trans2.IsChildOf(trans1);
    }
}