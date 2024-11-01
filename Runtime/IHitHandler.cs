using System;

[Obsolete]
public interface IHitHandler
{
    void OnHit(HitSubject hitSubject, HurtSubject hurtSubject, HitEventData evtData);
}

[Obsolete]
public interface IHurtHandler
{
    void OnHurt(HitSubject hitSubject, HurtSubject hurtSubject, HitEventData evtData);
}

/**
 * group of components that used by BlendMovement
 */
[Obsolete]
public interface IMoveHandler
{
    /**
     * 尽管大多数运动都是 move position delta
     * 这里不用直接返回 position displacement 的原因是
     * 实现中可能同时操作 rotation 或 position，而不是仅仅操作一种
     * 比如导弹追踪时既要转向，也要改变位移
     */
    void OnMove(MoveSubject movement, float deltaTime);
}