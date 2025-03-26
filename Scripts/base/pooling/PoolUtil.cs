using UnityEngine;

public static class PoolUtil
{
    private static IPoolingSystem _activePool;

    public static void SetActivePoolingSystem(IPoolingSystem poolingSystem)
    {
        _activePool = poolingSystem;
    }

    public static T Spawn<T>(T prefab, Transform parent = null) where T : Component
    {
        if (!prefab)
        {
            return null;
        }

        var instance = _activePool.Spawn(prefab.gameObject, Vector3.zero, Quaternion.identity, parent);
        instance.TryGetComponent(out T component);
        return component;
    }

    public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
    {
        if (!prefab)
        {
            return null;
        }

        var instance = _activePool.Spawn(prefab.gameObject, position, rotation, parent);
        instance.TryGetComponent(out T component);
        return component;
    }

    public static GameObject Spawn(GameObject prefab, Transform parent = null)
    {
        if (!prefab)
        {
            return null;
        }

        return _activePool.Spawn(prefab.gameObject, Vector3.zero, Quaternion.identity, parent);
    }

    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!prefab)
        {
            return null;
        }

        return _activePool.Spawn(prefab.gameObject, position, rotation, parent);
    }

    public static void Despawn(GameObject gameObject, float delay = 0f)
    {
        if (!gameObject)
        {
            return;
        }

        _activePool.Despawn(gameObject, delay);

        // // an object in scene and spawned by pool 
        // if (LeanPool.Links.ContainsKey(gameObject))
        // {
        //     // Debug.Log("case 1: spawned in scene");
        //     LeanPool.Despawn(gameObject, delay);
        //     return;
        // }
        //
        // // instantiate without pool
        // if (gameObject.activeSelf)
        // {
        //     // Debug.Log("case 2: instantiated in scene and active");
        //     Object.Destroy(gameObject, delay);
        //     return;
        // }
        //
        // Debug.LogWarning($"despawn an inactivated {gameObject}, please check if repeat despawn", gameObject);
    }
}