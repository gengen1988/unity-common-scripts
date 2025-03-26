using System;
using UnityEngine;

public class GameEntity : IDisposable
{
    private EntityState _state;
    private EntityProxy _proxy;

    public EntityState State => _state;
    public EntityProxy Proxy => _proxy;

    public GameEntity(EntityProxy proxy)
    {
        _proxy = proxy;
        _state = EntityState.Born;
    }

    public void SetState(EntityState state)
    {
        _state = state;
    }

    public void Dispose()
    {
        _proxy = null;
    }

    // is alive
    public static implicit operator bool(GameEntity exists)
    {
        if (exists == null)
        {
            return false;
        }

        if (exists._state == EntityState.Dead)
        {
            return false;
        }

        return true;
    }
}

public enum EntityState
{
    Born,
    Alive,
    Dead,
}

public static class GameEntityUtil
{
    public static GameEntity GetEntity(this Component component, bool findInParent = false)
    {
        var entityRoot = component.gameObject;
        if (findInParent)
        {
            var bridge = component.GetComponentInParent<EntityProxy>();
            entityRoot = bridge.gameObject;
        }

        return GameWorld.Instance.GetEntity(entityRoot);
    }

    public static GameEntity GetEntity(this GameObject gameObject, bool findInParent = false)
    {
        var entityRoot = gameObject;
        if (findInParent)
        {
            var bridge = gameObject.GetComponentInParent<EntityProxy>();
            entityRoot = bridge.gameObject;
        }

        return GameWorld.Instance.GetEntity(entityRoot);
    }
}