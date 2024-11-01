using UnityEngine;

public class HitEventData
{
    // public Vector2 Normal;
    public Vector2 ContactPoint;
    public Vector2 HitVelocity;
    public int HurtStampWhenHit = PoolUtil.DEFAULT_STAMP;
    public float CooldownTime;
}