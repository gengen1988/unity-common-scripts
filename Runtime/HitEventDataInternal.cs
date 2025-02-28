using UnityEngine;

public struct HitEventData
{
    public GameEntity HitEntity;
    public GameEntity HurtEntity;
    public Vector2 ContactPoint;
    public Vector2 HitVelocity;
}