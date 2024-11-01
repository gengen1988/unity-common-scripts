using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * 管理多个 collider 作为单一效果来源的的情况
 */
[Obsolete]
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
    private readonly DictionaryList<HurtSubject, HitEventData> _eventsByHurtSubject = new();

    private void Awake()
    {
        _handlers = this.GetAttachedComponents<IHitHandler>();
    }

    private void OnEnable()
    {
        // reset for pooling
        _coolingTime = 0;
        _eventsByHurtSubject.Clear();

        // notify manager
        IComponentManager<HitSubject>.NotifyEnabled(this);
    }

    private void OnDisable()
    {
        IComponentManager<HitSubject>.NotifyDisabled(this);
    }

#if UNITY_EDITOR
    private void Start()
    {
        Debug.Assert(SystemManager.GetSystem<HitManager>());
    }
#endif

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
        HitEventData evtData = new()
        {
            HurtStampWhenHit = PoolUtil.GetStamp(hurtSubject),
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

    private void TriggerHitEvent(HitSubject hitSubject, HurtSubject hurtSubject, HitEventData evtData)
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

        if (_eventsByHurtSubject.Keys.Count == 0)
        {
            return;
        }

        if (Penetrating)
        {
            foreach (HurtSubject hurtSubject in _eventsByHurtSubject.Keys)
            {
                TriggerEvents(this, hurtSubject);
            }
        }
        else
        {
            HurtSubject minPriorityHurtSubject = null;
            foreach (HurtSubject hurtSubject in _eventsByHurtSubject.Keys)
            {
                if (!minPriorityHurtSubject || hurtSubject.Priority < minPriorityHurtSubject.Priority)
                {
                    minPriorityHurtSubject = hurtSubject;
                }
            }

            TriggerEvents(this, minPriorityHurtSubject);
        }

        _eventsByHurtSubject.Clear();
    }

    private void TriggerEvents(HitSubject hitSubject, HurtSubject hurtSubject)
    {
        LinkedList<HitEventData> events = _eventsByHurtSubject[hurtSubject];
        int hitCount = 0;
        Vector2 contactPoint = Vector2.zero;
        Vector2 hitVelocity = Vector2.zero;
        foreach (HitEventData evt in events)
        {
            if (!PoolUtil.IsAlive(hurtSubject, evt.HurtStampWhenHit))
            {
                continue;
            }

            contactPoint += evt.ContactPoint;
            hitVelocity += evt.HitVelocity;
            hitCount++;
        }

        if (hitCount == 0)
        {
            return;
        }

        HitEventData mergedEvent = new()
        {
            HurtStampWhenHit = PoolUtil.GetStamp(hurtSubject),
            ContactPoint = contactPoint / hitCount,
            HitVelocity = hitVelocity / hitCount,
        };

        _bypassHit = false;
        _allowBypass = true;
        TriggerHitEvent(hitSubject, hurtSubject, mergedEvent);
        _allowBypass = false;
        if (_bypassHit)
        {
            return;
        }

        hurtSubject.NotifyHurtEvent(hitSubject, hurtSubject, mergedEvent);
        _coolingTime = HitInterval;
    }
}