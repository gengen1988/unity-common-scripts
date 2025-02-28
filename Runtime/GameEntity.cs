using UnityEngine;

// just a plain object
public class GameEntity
{
}

public static class GameEntityUtil
{
    public static bool IsAlive(this GameEntity entity)
    {
        return GameWorld.Instance.GetBridge(entity);
    }

    public static GameEntityBridge FindEntityBridge(this Component component)
    {
        return component.GetComponentInParent<GameEntityBridge>();
    }

    public static GameEntity GetEntity(this GameObject gameObject)
    {
        return GameWorld.Instance.GetEntity(gameObject);
    }

    public static GameEntityBridge GetBridge(this GameEntity entity)
    {
        return GameWorld.Instance.GetBridge(entity);
    }
}