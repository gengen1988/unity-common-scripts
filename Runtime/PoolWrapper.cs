using Lean.Pool;
using UnityEngine;

public static class PoolWrapper
{
    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        return LeanPool.Spawn(prefab, position, rotation, parent);
    }

    public static void Despawn(GameObject gameObject, float delay = 0f)
    {
        if (LeanPool.Links.ContainsKey(gameObject))
        {
            LeanPool.Despawn(gameObject, delay);
        }
        else
        {
            Object.Destroy(gameObject, delay);
        }
    }

    public static bool IsAlive(GameObject gameObject)
    {
        return gameObject && gameObject.activeInHierarchy;
    }

    public static bool IsAlive(Component component)
    {
        return component && component.gameObject.activeInHierarchy;
    }
}