using System.Collections.Generic;
using UnityEngine;

/**
 * 管理多个 collider 作为单一效果来源的的情况
 */
public class HitSubject : MonoBehaviour
{
    public float HitInterval = 0.1f;

    private readonly List<IHitHandler> _handlers = new();
    private readonly List<ContactPoint2D> _contactBuffer = new();

    private void Awake()
    {
        UnityUtil.FindAttachedComponents(this, _handlers);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        EnqueueCollisionEvent(other);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        EnqueueCollisionEvent(other);
    }

    private void EnqueueCollisionEvent(Collision2D other)
    {
        // cooling optimize
        HitHurtSystem system = SystemManager.GetSystem<HitHurtSystem>();
        if (system.IsCooling(this))
        {
            return;
        }

        // filter by hurtbox
        if (!TryGetHurtSubject(other.collider, out HurtSubject hurtSubject))
        {
            return;
        }

        // calculate contact point
        other.GetContacts(_contactBuffer);
        Vector2 hitPosition = _contactBuffer.CenterOfMass(contact => contact.point);

        // leave hit system processing
        CollisionEventData evtData = new()
        {
            ContactPoint = hitPosition,
            HitVelocity = -other.relativeVelocity,
        };
        system.EnqueueCollisionEvent(this, hurtSubject, evtData);
    }

    private bool TryGetHurtSubject(Collider2D from, out HurtSubject subject)
    {
        // rigidbody mode
        if (from.attachedRigidbody)
        {
            return from.attachedRigidbody.TryGetComponent(out subject);
        }

        // collider mode
        return from.TryGetComponent(out subject);
    }

    public void TriggerHitEvent(HitSubject hitSubject, HurtSubject hurtSubject, CollisionEventData evtData)
    {
        foreach (IHitHandler handler in _handlers)
        {
            if (handler is not Behaviour unityBehaviour)
            {
                continue;
            }

            if (!unityBehaviour.isActiveAndEnabled)
            {
                continue;
            }

            handler.OnHit(hitSubject, hurtSubject, evtData);
        }
    }
}