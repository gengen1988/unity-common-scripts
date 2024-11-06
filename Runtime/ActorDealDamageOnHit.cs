using UnityEngine;

public class ActorDealDamageOnHit : MonoBehaviour
{
    [SerializeField] private int DefaultDamage = 100;
    // public float KnockBackForce = 10f;

    private void Awake()
    {
        ActorHitManager hitManager = GetComponentInParent<ActorHitManager>();
        hitManager.OnHit += HandleHit;
    }

    private void HandleHit(ActorOld hitSubject, ActorOld hurtSubject, HitEventData evtData)
    {
        if (hurtSubject.TryGetComponent(out ActorHealth health))
        {
            health.DealDamage(DefaultDamage);
        }
    }
}