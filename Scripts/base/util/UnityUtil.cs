using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

public static class UnityUtil
{
    public static GameObject FindPlayer()
    {
        return GameObject.FindWithTag("Player");
    }

    public static Vector2 GetInputVector()
    {
        var vector = Vector2.zero;
#if ENABLE_INPUT_SYSTEM
        var keyboard = Keyboard.current;
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
        var currentMouse = Mouse.current;
        if (currentMouse == null)
        {
            return Vector2.zero;
        }

        mousePosition = currentMouse.position.ReadValue();
#else
        mousePosition = Input.mousePosition;
#endif

        var mainCamera = Camera.main;
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
        var x = Input.GetAxisRaw("Mouse X");
        var y = Input.GetAxisRaw("Mouse Y");
        return new Vector2(x, y);
    }

    /**
     * 取得 transform 在场景中的路径
     */
    public static string GetPath(this Transform transform)
    {
        if (!transform)
        {
            return "";
        }

        return $"{GetPath(transform.parent)}/{transform.name}";
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
            var go = GameObject.Find(childName);
            child = go && !go.transform.parent ? go.transform : null;
        }

        if (!child)
        {
            var go = new GameObject(childName);
            child = go.transform;
            if (root)
            {
                child.SetParent(root, false);
            }
        }

        return child;
    }

    /**
     * 清理 transform 下的子物体
     */
    public static void DestroyChildren(this Transform src)
    {
        if (Application.isPlaying)
        {
            foreach (Transform child in src)
            {
                Object.Destroy(child.gameObject);
            }
        }
        else
        {
            // 在编辑阶段里只能用 DestroyImmediate
            // 而且 DestroyImmediate 用 foreach 会漏删
            while (src.childCount > 0)
            {
                var child = src.GetChild(0);
                Object.DestroyImmediate(child.gameObject);
            }
        }
    }

    public static IEnumerable<Transform> Children(this Transform src)
    {
        return src.Cast<Transform>();
    }

    /**
     * move position in camera space ui
     */
    public static Vector3 TranslateWithCamera(Vector3 origin, Vector3 delta, Camera translateCamera)
    {
        var currentScreenPoint = translateCamera.WorldToScreenPoint(origin);
        var newWorldPoint = translateCamera.ScreenToWorldPoint(currentScreenPoint + delta);
        return newWorldPoint;
    }

    public static Vector3 CameraRemap(Vector3 fromWorldPosition, Camera from, Camera to)
    {
        var screenPoint = from.WorldToScreenPoint(fromWorldPosition);
        var remapped = to.ScreenToWorldPoint(screenPoint);
        return remapped;
    }

    /**
     * can be used to determine two RectTransform intersect
     */
    public static Rect GetScreenRect(RectTransform trans)
    {
        var origin = trans.position;
        var pivot = trans.pivot;

        // Calculate rect taking into account its scale
        var size = Vector2.Scale(trans.rect.size, trans.lossyScale);
        var rect = new Rect(origin.x, origin.y, size.x, size.y);

        // Adjust the x and y coordinates of the Rect based on the pivot point
        rect.x -= pivot.x * size.x;
        rect.y -= pivot.y * size.y;
        return rect;
    }

    public static bool Intersects(RectTransform trans1, RectTransform trans2)
    {
        var rect1 = GetScreenRect(trans1);
        var rect2 = GetScreenRect(trans2);
        return rect1.Overlaps(rect2);
    }

    public static bool Contains(RectTransform outer, RectTransform inner)
    {
        var outerRect = GetScreenRect(outer);
        var innerRect = GetScreenRect(inner);
        return outerRect.Contains(innerRect.min) && outerRect.Contains(innerRect.max);
    }

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

    public static bool IsSibling(Transform trans1, Transform trans2)
    {
        if (!trans1 || !trans2)
        {
            return false;
        }

        return !trans1.IsChildOf(trans2) && !trans2.IsChildOf(trans1);
    }

    public static IEnumerable<Transform> TraverseTransform(Transform src, bool deep)
    {
        if (!src)
        {
            yield break;
        }

        foreach (Transform child in src)
        {
            yield return child;
            if (!deep)
            {
                continue;
            }

            foreach (var childOfChild in TraverseTransform(child, true))
            {
                yield return childOfChild;
            }
        }
    }

    public static Transform GetAttachedTransform(this Collider2D collider)
    {
        if (!collider)
        {
            return null;
        }

        return collider.attachedRigidbody ? collider.attachedRigidbody.transform : collider.transform;
    }
}