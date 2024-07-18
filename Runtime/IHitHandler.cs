public interface IHitHandler
{
    void OnHit(HitSubject hitSubject, HurtSubject hurtSubject, CollisionEventData evtData);
}