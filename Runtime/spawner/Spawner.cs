using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
    public GameObject prefabToSpawn;

    protected virtual GameObject Spawn()
    {
        var root = transform;
        Debug.Log($"spawn: {prefabToSpawn.name}");
        var instance = Instantiate(prefabToSpawn, root.position, root.rotation);
        foreach (var spawnable in instance.GetComponentsInChildren<ISpawnable>())
        {
            spawnable.OnSpawn(this);
        }

        return instance;
    }
}

public interface ISpawnable
{
    void OnSpawn(Spawner spawner);
}