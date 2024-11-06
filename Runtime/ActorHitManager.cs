using System.Collections.Generic;
using UnityEngine;

public delegate void HitHurtEvent(ActorOld hitSubject, ActorOld hurtSubject, HitEventData evtData);

[DefaultExecutionOrder(10)]
public class ActorHitManager : MonoBehaviour
{
    [SerializeField] private bool DebugLog;

    public event HitHurtEvent OnHit;
    public event HitHurtEvent OnHurt;

    private float _cooldownTime;
    private ActorOld _actorOld;
    private readonly DictionaryList<ActorOld, HitEventData> _eventsByHurtSubject = new();

    private void Awake()
    {
        TryGetComponent(out _actorOld);
    }

    private void OnEnable()
    {
        _cooldownTime = 0;
        _eventsByHurtSubject.Clear();
    }

    private void OnDestroy()
    {
        OnHit = null;
        OnHurt = null;
    }

    private void FixedUpdate()
    {
        Tick(Time.fixedDeltaTime);
    }

    private void Tick(float deltaTime)
    {
        // cooling
        if (_cooldownTime > 0)
        {
            if (DebugLog)
            {
                Debug.Log($"cooling: {_cooldownTime}");
            }

            _cooldownTime -= deltaTime;
            return;
        }

        // no collision events since previous frame
        if (_eventsByHurtSubject.Keys.Count == 0)
        {
            return;
        }

        // all subjects get hurt
        foreach (ActorOld hurtSubject in _eventsByHurtSubject.Keys)
        {
            // remove invalid events
            var events = _eventsByHurtSubject[hurtSubject];
            events.RemoveAll(evt => !PoolUtil.IsAlive(hurtSubject, evt.HurtStampWhenHit));
            var mergedEvent = MergeHitEvents(events);
            if (mergedEvent == null)
            {
                continue;
            }

            // set cooldown
            _cooldownTime = mergedEvent.CooldownTime;

            // notify event
            OnHit?.Invoke(_actorOld, hurtSubject, mergedEvent);
            OnHurt?.Invoke(_actorOld, hurtSubject, mergedEvent);
        }

        _eventsByHurtSubject.Clear();
    }

    private HitEventData MergeHitEvents(LinkedList<HitEventData> events)
    {
        var hitCount = 0;
        var contactPoint = Vector2.zero;
        var hitVelocity = Vector2.zero;
        var cooldownTime = 0f;
        foreach (var evt in events)
        {
            if (evt.CooldownTime > cooldownTime)
            {
                cooldownTime = evt.CooldownTime;
            }

            contactPoint += evt.ContactPoint;
            hitVelocity += evt.HitVelocity;
            hitCount++;
        }

        if (hitCount == 0)
        {
            return null;
        }

        HitEventData mergedEvent = new()
        {
            ContactPoint = contactPoint / hitCount,
            HitVelocity = hitVelocity / hitCount,
            CooldownTime = cooldownTime,
        };

        return mergedEvent;
    }

    public bool IsCooldown()
    {
        return _cooldownTime > 0;
    }

    /**
     * use this method to report collision.
     * either in event callback or raycast manually
     */
    public static void EnqueueHitEvent(ActorOld hitSubject, ActorOld hurtSubject, HitEventData evtData)
    {
        hitSubject.TryGetComponent(out ActorHitManager hitManager);
        evtData.HurtStampWhenHit = PoolUtil.GetStamp(hurtSubject);
        hitManager._eventsByHurtSubject.Add(hurtSubject, evtData);
    }
}