using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * 管理多个 collider 作为单一效果来源的的情况
 */
public class HitSubject : MonoBehaviour
{
    public float HitInterval = 0.1f;
    public bool Penetrating;

    private bool _bypassHit;
    private bool _allowBypass;
    private float _coolingTime;

    private HitManager _manager;
    private IHitHandler[] _handlers;

    private readonly List<ContactPoint2D> _contactBuffer = new();
    private readonly List<HurtSubject> _hurtSubjectBuffer = new(); // for sort
    private readonly DictionaryList<HurtSubject, CollisionEventData> _eventsByHurtSubject = new();

    private void Awake()
    {
        _handlers = this.GetAttachedComponents<IHitHandler>();
    }

    private void OnEnable()
    {
        // reset for pooling
        _coolingTime = 0;

        // register to system
        _manager = SystemManager.GetSystem<HitManager>();
        _manager.RegisterSubject(this);
    }

    private void OnDisable()
    {
        if (_manager)
        {
            _manager.RemoveSubject(this);
        }
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
        if (_coolingTime > 0)
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
        Vector2 contactPoint = _contactBuffer.CenterOfMass(contact => contact.point);

        // leave hit manager processing
        CollisionEventData evtData = new()
        {
            HitStamp = PoolWrapper.GetStamp(this),
            HurtStamp = PoolWrapper.GetStamp(hurtSubject),
            ContactPoint = contactPoint,
            HitVelocity = -other.relativeVelocity,
        };
        _eventsByHurtSubject.Add(hurtSubject, evtData);
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

    private void TriggerHitEvent(HitSubject hitSubject, HurtSubject hurtSubject, CollisionEventData evtData)
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

    public void BypassCurrentHit()
    {
        if (!_allowBypass)
        {
            throw new InvalidOperationException("can't bypass in this timing");
        }

        _bypassHit = true;
    }

    public void Tick(float deltaTime)
    {
        if (_coolingTime > 0)
        {
            _coolingTime -= deltaTime;
            return;
        }

        _hurtSubjectBuffer.Clear();
        _hurtSubjectBuffer.AddRange(_eventsByHurtSubject.Keys);
        if (!Penetrating)
        {
            _hurtSubjectBuffer.SortBy(s => s.Priority);
        }

        foreach (HurtSubject hurtSubject in _hurtSubjectBuffer)
        {
            LinkedList<CollisionEventData> events = _eventsByHurtSubject[hurtSubject];
            int hitCount = 0;
            Vector2 contactPoint = Vector2.zero;
            Vector2 hitVelocity = Vector2.zero;
            foreach (CollisionEventData evt in events)
            {
                if (!PoolWrapper.IsAlive(this, evt.HitStamp))
                {
                    continue;
                }

                if (!PoolWrapper.IsAlive(hurtSubject, evt.HurtStamp))
                {
                    continue;
                }

                contactPoint += evt.ContactPoint;
                hitVelocity += evt.HitVelocity;
                hitCount++;
            }

            if (hitCount == 0)
            {
                continue;
            }

            CollisionEventData mergedEvent = new()
            {
                HitStamp = PoolWrapper.GetStamp(this),
                HurtStamp = PoolWrapper.GetStamp(hurtSubject),
                ContactPoint = contactPoint / hitCount,
                HitVelocity = hitVelocity / hitCount,
            };

            _bypassHit = false;
            _allowBypass = true;
            TriggerHitEvent(this, hurtSubject, mergedEvent);
            _allowBypass = false;
            if (_bypassHit)
            {
                continue;
            }

            hurtSubject.TriggerHurtEvent(this, hurtSubject, mergedEvent);
            _coolingTime = HitInterval;

            if (!Penetrating)
            {
                break;
            }
        }

        _eventsByHurtSubject.Clear();
    }
}