public interface IHurtHandler
{
    void OnHurt(HitSubject src, HurtSubject dest, CollisionEventData evtData);
}