using Lean.Pool;
using UnityEngine;

public class LeanPoolAdapter : IPoolingSystem
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Init()
    {
        var adapter = new LeanPoolAdapter();
        PoolUtil.SetActivePoolingSystem(adapter);
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        return LeanPool.Spawn(prefab, position, rotation, parent);
    }

    public void Despawn(GameObject gameObject, float delay = 0)
    {
        LeanPool.Despawn(gameObject, delay);
    }
}