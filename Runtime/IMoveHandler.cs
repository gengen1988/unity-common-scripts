using UnityEngine;

/**
 * group of components that used by BlendMovement
 */
public interface IMoveHandler
{
    /**
     * 尽管大多数运动都是 move position delta
     * 这里不用直接返回 position displacement 的原因是
     * 实现中可能同时操作 rotation 或 position，而不是仅仅操作一种
     * 比如导弹追踪时既要转向，也要改变位移
     */
    void OnMove(IMoveSubject subject, float deltaTime);
}

public interface IMoveSubject
{
    Quaternion GetRotation();
    Vector2 GetPosition();
    Vector2 GetVelocity();
    void MovePositionDelta(Vector2 deltaPosition);
    void MoveRotationDelta(Quaternion deltaRotation);
    void ResetVelocity();
}

public interface IManagedMovement
{
    void Tick(float deltaTime);
    void Commit();
}