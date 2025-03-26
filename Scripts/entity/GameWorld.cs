using System.Collections.Generic;
using UnityEngine;
using Weaver;

[DefaultExecutionOrder(CustomExecutionOrder.Late)] // for other system use fixed update directly
public class GameWorld : WeaverSingletonBehaviour<GameWorld>
{
    [AssetReference] private static readonly GameWorld SingletonPrefab;

    private readonly Dictionary<GameObject, GameEntity> _entityByGameObject = new();
    private readonly LinkedList<GameEntity> _entities = new();

    private void FixedUpdate()
    {
        // tick
        var frameTime = Time.fixedDeltaTime;
        Tick(_entities, frameTime); // newly appended entities while be scheduled to next frame
    }

    private void Tick(LinkedList<GameEntity> entities, float frameTime)
    {
        var node = entities.First;
        while (node != null)
        {
            var next = node.Next;
            var entity = node.Value;
            var bridge = entity.Proxy;

            if (entity.State == EntityState.Born)
            {
                entity.SetState(EntityState.Alive);
                bridge.Attach(entity); // this callback like Start in unity
            }

            if (entity.State == EntityState.Alive)
            {
                bridge.Tick(entity, frameTime);
            }

            if (entity.State == EntityState.Dead)
            {
                bridge.Detach(entity); // like OnDestroy in unity
                entities.Remove(node);
                entity.Dispose();
            }

            node = next;
        }
    }

    public void RegisterEntity(GameObject unityGameObject)
    {
        if (_entityByGameObject.ContainsKey(unityGameObject))
        {
            // already registered
            Debug.LogAssertion("game object already registered as entity", unityGameObject);
            return;
        }

        // create entity
        var bridge = unityGameObject.EnsureComponent<EntityProxy>();
        var entity = new GameEntity(bridge); // use c# object as pool stamp

        // register to index
        _entityByGameObject[unityGameObject] = entity;
        _entities.AddFirst(entity); // AddFirst ensures tick in next frame
    }

    public void DeregisterEntity(GameObject unityGameObject)
    {
        if (!_entityByGameObject.Remove(unityGameObject, out var entity))
        {
            // not registered yet
            Debug.LogAssertion("game object not registered as entity", unityGameObject);
            return;
        }

        entity.SetState(EntityState.Dead);
    }

    public GameEntity GetEntity(GameObject unityGameObject)
    {
        if (!unityGameObject)
        {
            return null;
        }

        return _entityByGameObject.GetValueOrDefault(unityGameObject);
    }
}