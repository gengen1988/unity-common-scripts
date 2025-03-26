using UnityEngine;

public interface IPoolingSystem
{
    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null);
    public void Despawn(GameObject gameObject, float delay = 0f);
}