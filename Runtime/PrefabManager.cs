using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)] // before game manager  
public class PrefabManager : SingletonBehaviour<PrefabManager>
{
    [SerializeField] private PrefabDatabase[] Databases;

    private readonly Dictionary<string, GameObject> _prefabByKey = new();

    protected override void HandleFrameworkInit(OnFrameworkInit evt)
    {
        foreach (var database in Databases)
        {
            foreach (var prefab in database.Entries)
            {
                var key = prefab.name.ToLowerInvariant();
                _prefabByKey.Add(key, prefab);
            }
        }
    }

    public static T GetPrefab<T>(string key) where T : Component
    {
        key = key.ToLowerInvariant();
        if (!Instance._prefabByKey.TryGetValue(key, out var go))
        {
            return null;
        }

        return go.TryGetComponent(out T component) ? component : null;
    }
}