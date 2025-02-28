using System.Collections.Generic;
using UnityEngine;
using Weaver;

public class GameWorld : WeaverSingletonBehaviour<GameWorld>
{
    [AssetReference] private static readonly GameWorld SingletonPrefab;

    private readonly Dictionary<GameEntity, GameEntityBridge> _bridgeByEntity = new();
    private readonly Dictionary<GameObject, GameEntity> _entityByGameObject = new();
    private readonly Dictionary<FrameType, LinkedList<GameEntity>> _frameGroupByType = new();
    private readonly List<GameEntity> _toBeAttach = new();

    private void Awake()
    {
        _frameGroupByType[FrameType.Early] = new LinkedList<GameEntity>();
        _frameGroupByType[FrameType.Normal] = new LinkedList<GameEntity>();
        _frameGroupByType[FrameType.Late] = new LinkedList<GameEntity>();
    }

    private void FixedUpdate()
    {
        var frameTime = Time.fixedDeltaTime;

        // tick
        Tick(_frameGroupByType[FrameType.Early], frameTime); // usually system before actor
        Tick(_frameGroupByType[FrameType.Normal], frameTime); // usually actor
        Tick(_frameGroupByType[FrameType.Late], frameTime); // usually cleanup

        // attach after other logic - ensure feedback does not lag 1 frame
        while (_toBeAttach.Count > 0)
        {
            var entity = _toBeAttach.PopLast();
            var bridge = _bridgeByEntity[entity];
            bridge.Attach(entity);
        }
    }

    private void Tick(LinkedList<GameEntity> entities, float frameTime)
    {
        var node = entities.First;
        while (node != null)
        {
            var next = node.Next;
            var entity = node.Value;
            var bridge = _bridgeByEntity.GetValueOrDefault(entity);

            // cleanup
            if (!bridge)
            {
                // Debug.Log("cleanup entity");
                entities.Remove(node); // this cause freeze a little while
                node = next;
                continue;
            }

            // do not tick not attached entity
            if (!bridge.IsAttached)
            {
                node = next;
                continue;
            }

            // tick
            bridge.Tick(entity, frameTime);
            node = next;
        }
    }

    public void RegisterEntity(GameObject unityGameObject, FrameType frameType = FrameType.Normal)
    {
        if (_entityByGameObject.ContainsKey(unityGameObject))
        {
            return;
        }

        // create entity
        var entity = new GameEntity(); // use c# object as pool stamp
        var bridge = unityGameObject.EnsureComponent<GameEntityBridge>();

        // register to index
        _entityByGameObject[unityGameObject] = entity;
        _bridgeByEntity[entity] = bridge;
        _toBeAttach.Add(entity);
        _frameGroupByType[frameType].AddFirst(entity); // AddFirst ensures tick in next frame
    }

    public void DeregisterEntity(GameObject unityGameObject)
    {
        if (!_entityByGameObject.TryGetValue(unityGameObject, out var entity))
        {
            return;
        }

        // mark as unregistered
        var bridge = _bridgeByEntity[entity];
        if (bridge.IsAttached)
        {
            bridge.Detach(entity);
        }
        else
        {
            Debug.LogWarning("detach before attachment");
            _toBeAttach.Remove(entity); // rarely happen, so use iterate remove
        }

        // remove game object relationship immediately
        // other relationships removed at next tick
        _bridgeByEntity.Remove(entity);
        _entityByGameObject.Remove(unityGameObject);
    }

    public GameEntity GetEntity(GameObject unityGameObject)
    {
        if (!unityGameObject)
        {
            return null;
        }

        return _entityByGameObject.GetValueOrDefault(unityGameObject);
    }

    public GameEntityBridge GetBridge(GameEntity entity)
    {
        if (entity == null)
        {
            return null;
        }

        return _bridgeByEntity.GetValueOrDefault(entity);
    }
}

public enum FrameType
{
    Early,
    Normal,
    Late,
}