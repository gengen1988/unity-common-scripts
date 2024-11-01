using Lean.Pool;
using UnityEngine;
using Object = UnityEngine.Object;

public static class PoolUtil
{
    public const int DEFAULT_STAMP = 0;

    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!prefab)
        {
            return null;
        }

        // note that if someone want stamp on enable, the prefab should spawn in inactivated state
        var gameObject = LeanPool.Spawn(prefab, position, rotation, parent);
        var stamp = Random.Range(10000, 100000); // never be default
        var poolData = gameObject.EnsureComponent<PoolData>();
        poolData.Stamp = stamp;
        return gameObject;
    }

    public static GameObject Spawn(GameObject prefab, Transform parent = null)
    {
        var position = parent ? parent.position : Vector3.zero;
        var rotation = parent ? parent.rotation : Quaternion.identity;
        return Spawn(prefab, position, rotation, parent);
    }

    public static void Despawn(GameObject gameObject, float delay = 0f)
    {
        // not exists
        if (!gameObject)
        {
            return;
        }

        // an object in scene and spawned by pool 
        if (LeanPool.Links.ContainsKey(gameObject))
        {
            // Debug.Log("case 1: spawned in scene");
            LeanPool.Despawn(gameObject, delay);
            return;
        }

        // instantiate without pool
        if (gameObject.activeInHierarchy)
        {
            // Debug.Log("case 2: instantiated in scene and active");
            Object.Destroy(gameObject, delay);
            return;
        }

        Debug.LogWarning($"despawn an inactivated {gameObject}, please check if repeat despawn", gameObject);
    }

    public static bool IsAlive(GameObject gameObject, int spawnStamp)
    {
        if (!gameObject)
        {
            return false;
        }

        if (!gameObject.activeInHierarchy)
        {
            return false;
        }

        if (LeanPool.Links.ContainsKey(gameObject))
        {
            return spawnStamp == GetStamp(gameObject);
        }

        return true;
    }

    public static bool IsAlive(Component component, int spawnStamp)
    {
        if (!component)
        {
            return false;
        }

        return IsAlive(component.gameObject, spawnStamp);
    }

    public static int GetStamp(GameObject gameObject)
    {
        if (!gameObject)
        {
            return DEFAULT_STAMP;
        }

        if (!gameObject.TryGetComponent(out PoolData poolData))
        {
            return DEFAULT_STAMP;
        }

        return poolData.Stamp;
    }

    public static int GetStamp(Component component)
    {
        if (!component)
        {
            return DEFAULT_STAMP;
        }

        return GetStamp(component.gameObject);
    }
}