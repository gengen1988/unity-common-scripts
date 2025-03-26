using System;
using UnityEngine;

public struct HitInfo
{
    public HitPair Participants;
    public Vector2 ContactPoint;
    public Vector2 HitVelocity;
    public HitstopType Hitstop;
    public bool IsCritical;
}

public struct HitPair : IEquatable<HitPair>
{
    // source
    public GameEntity HitEntity;
    public Hitable HitSubject;

    // destination
    public GameEntity HurtEntity;
    public Hurtable HurtSubject;

    public bool Equals(HitPair other)
    {
        return Equals(HitEntity, other.HitEntity) &&
               Equals(HitSubject, other.HitSubject) &&
               Equals(HurtEntity, other.HurtEntity) &&
               Equals(HurtSubject, other.HurtSubject);
    }

    public override bool Equals(object obj)
    {
        return obj is HitPair other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(HitEntity, HitSubject, HurtEntity, HurtSubject);
    }
}