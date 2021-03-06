﻿using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
    public GameObject prefabToSpawn;

    protected virtual GameObject Spawn()
    {
        var root = transform;
        return Spawn(root.position, root.rotation);
    }

    protected GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        if (prefabToSpawn)
        {
            Debug.Log($"spawn: {prefabToSpawn.name}");
            var instance = Instantiate(prefabToSpawn, position, rotation);
            foreach (var spawnable in instance.GetComponentsInChildren<ISpawnable>())
            {
                spawnable.OnSpawn(this);
            }

            return instance;
        }

        Debug.LogWarning("no prefab to spawn");
        return null;
    }
}

public interface ISpawnable
{
    void OnSpawn(Spawner spawner);
}