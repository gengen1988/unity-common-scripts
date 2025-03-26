using UnityEngine;

public interface IMoveState
{
    public Vector2 Position { get; }
    public Vector2 Velocity { get; }
}

public static class MoveStateUtil
{
    public static IMoveState GetMove(this GameEntity entity)
    {
        if (!entity)
        {
            return null;
        }

        if (!entity.Proxy.TryGetComponent(out IMoveState moveState))
        {
            return null;
        }

        return moveState;
    }
}