using System;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class EntityManager : Singleton<EntityManager>
{
    private readonly LinkedList<GameEntity> _entities = new();
    private readonly Dictionary<GameObject, GameEntity> _entityByGameObject = new();
    private Transform _stagingContainer;

    public void Tick(float deltaTime)
    {
        for (var node = _entities.First; node != null; node = node.Next)
        {
            var entity = node.Value;
            if (entity.CurrentState == EntityState.Inactive)
            {
                // recycle
                var go = entity.gameObject;
                _entityByGameObject.Remove(go);
                _entities.Remove(node);
                LeanPool.Despawn(go);
                continue;
            }

            entity.Tick(deltaTime);
        }
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        var stagingContainer = GetOrCreateStagingContainer();
        var go = LeanPool.Spawn(prefab, position, rotation, stagingContainer); // ensure spawn inactive
        var entity = go.EnsureComponent<GameEntity>();
        go.transform.SetParent(parent);
        _entities.AddLast(entity);
        _entityByGameObject[go] = entity;
        entity.SendSpawn();
        return go;
    }

    public void Kill(GameObject gameObject)
    {
        var entity = _entityByGameObject[gameObject];
        entity.SendKill();
    }

    private Transform GetOrCreateStagingContainer()
    {
        if (!_stagingContainer)
        {
            _stagingContainer = UnityUtil.EnsureChild(null, Guid.NewGuid().ToString());
            _stagingContainer.gameObject.SetActive(false);
            _stagingContainer.gameObject.hideFlags =
                HideFlags.DontSave | HideFlags.NotEditable | HideFlags.HideInHierarchy;
        }

        return _stagingContainer;
    }
}