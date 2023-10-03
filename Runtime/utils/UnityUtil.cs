using UnityEngine;

public static class UnityUtil
{
    public static GameObject FindPlayer()
    {
        return GameObject.FindWithTag("Player");
    }

    /**
     * 取得鼠标在世界座标的位置。兼容 InputSystem
     */
    public static Vector2 GetMouseWorldPosition()
    {
        Vector2 mousePosition;
#if ENABLE_INPUT_SYSTEM
		mousePosition = Mouse.current.position.ReadValue();
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
        Transform child = root.Find(childName);
        if (!child)
        {
            GameObject go = new GameObject(childName);
            child = go.transform;
            child.SetParent(root, false);
        }

        return child;
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
            // 在编辑阶段里只能用 DestroyImmediate，而且 DestroyImmediate 用 foreach 会导致漏删
            while (root.childCount > 0)
            {
                Transform target = root.GetChild(0);
                Object.DestroyImmediate(target.gameObject);
            }
        }
    }
}