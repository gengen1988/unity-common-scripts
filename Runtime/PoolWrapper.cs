using Lean.Pool;
using UnityEngine;
using Object = UnityEngine.Object;

public static class PoolWrapper
{
    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        bool before = prefab.activeSelf;
        prefab.SetActive(false);
        GameObject gameObject = LeanPool.Spawn(prefab, position, rotation, parent);
        AttachStamp(gameObject);
        gameObject.SetActive(before);
        prefab.SetActive(before);
        return gameObject;
    }

    public static void Despawn(GameObject gameObject, float delay = 0f)
    {
        if (LeanPool.Links.ContainsKey(gameObject))
        {
            // Debug.Log($"detach stamp: {gameObject}");
            DetachStamp(gameObject);
            // Debug.Log($"despawn: {gameObject}");
            LeanPool.Despawn(gameObject, delay);
        }
        else
        {
            Object.Destroy(gameObject, delay);
        }
    }

    public static bool IsAlive(GameObject gameObject, int spawnStamp)
    {
        if (LeanPool.Links.ContainsKey(gameObject))
        {
            if (spawnStamp < 0)
            {
                return false;
            }

            int currentStamp = GetStamp(gameObject);
            if (currentStamp < 0)
            {
                return false;
            }

            // Debug.Log($"{gameObject} is alive: current - {currentStamp}, spawn - {spawnStamp}");
            return currentStamp == spawnStamp;
        }
        else
        {
            return gameObject && gameObject.activeInHierarchy;
        }
    }

    public static bool IsAlive(Component component, int spawnStamp)
    {
        if (!component)
        {
            return false;
        }

        return IsAlive(component.gameObject, spawnStamp);
    }

    /**
     * stamp is like:
     * [10546] Projectile_NormalShot (Clone)
     */
    public static int GetStamp(GameObject gameObject)
    {
        if (!gameObject)
        {
            return -1;
        }

        if (!gameObject.TryGetComponent(out PoolAddition data))
        {
            return -1;
        }

        return data.Stamp;
    }

    public static int GetStamp(Component component)
    {
        if (!component)
        {
            return -1;
        }

        return GetStamp(component.gameObject);
    }

    private static void AttachStamp(GameObject gameObject)
    {
        int stamp = Random.Range(10000, 100000);
        PoolAddition data = gameObject.GetOrAddComponent<PoolAddition>();
        data.Stamp = stamp;
    }

    private static void DetachStamp(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out PoolAddition data))
        {
            data.Stamp = -1;
        }
    }
}