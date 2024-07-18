using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(5000)] // after other fixed update logic (such as rb.MovePosition)
public class HitHurtSystem : MonoBehaviour, ISystem
{
    public bool DebugLog;

    private readonly List<HitSubject> _cooldownKeys = new();
    private readonly Dictionary<HitSubject, float> _cooldownByHit = new();
    private readonly DictionaryList<HitHurtPair, CollisionEventData> _collisionsByPair = new();
    private bool _bypassHit;

    private void FixedUpdate()
    {
        Tick(Time.deltaTime);
    }

    private void Tick(float deltaTime)
    {
        // cooldown
        _cooldownKeys.Clear();
        _cooldownKeys.AddRange(_cooldownByHit.Keys);
        foreach (HitSubject hitSubject in _cooldownKeys)
        {
            // cleanup dead hit subjects
            if (!PoolWrapper.IsAlive(hitSubject))
            {
                if (DebugLog)
                {
                    Debug.Log($"cleanup subject that destroyed: {hitSubject}");
                }

                _cooldownByHit.Remove(hitSubject);
                continue;
            }

            // cleanup old data
            float cooldownTime = _cooldownByHit[hitSubject];
            if (cooldownTime <= 0)
            {
                if (DebugLog)
                {
                    Debug.Log($"cleanup cooldown: {hitSubject}");
                }

                _cooldownByHit.Remove(hitSubject);
                continue;
            }

            // execute cooldown
            float newTime = cooldownTime - deltaTime;
            _cooldownByHit[hitSubject] = newTime;
            if (DebugLog)
            {
                Debug.Log($"cooldown: {newTime}");
            }
        }

        // collision handling
        IOrderedEnumerable<HitHurtPair> collisionPairs = _collisionsByPair.Keys
            .Where(pair => _cooldownByHit.GetValueOrDefault(pair.Src, 0) <= 0) // not cooling
            .Where(pair => PoolWrapper.IsAlive(pair.Src) && PoolWrapper.IsAlive(pair.Dest)) // not dead
            .OrderBy(pair => pair.Dest.Priority);
        foreach (HitHurtPair pair in collisionPairs)
        {
            HitSubject hitSubject = pair.Src;
            HurtSubject hurtSubject = pair.Dest;

            // merge vectors
            List<CollisionEventData> events = _collisionsByPair[pair];
            CollisionEventData mergedEvent = new()
            {
                ContactPoint = events.CenterOfMass(evt => evt.ContactPoint),
                HitVelocity = events.CenterOfMass(evt => evt.HitVelocity),
            };

            // collision handling
            _bypassHit = false;
            hitSubject.TriggerHitEvent(hitSubject, hurtSubject, mergedEvent);
            if (_bypassHit)
            {
                if (DebugLog)
                {
                    Debug.Log("hit bypassed");
                }

                continue;
            }

            hurtSubject.TriggerHurtEvent(hitSubject, hurtSubject, mergedEvent);

            if (DebugLog)
            {
                Debug.Log($"hit happened: {pair.Src} to {pair.Dest}, at {mergedEvent.ContactPoint}");
            }

            // store cooldown time (not precise but looks even)
            _cooldownByHit[hitSubject] = hitSubject.HitInterval;
        }

        _collisionsByPair.Clear();
    }

    public void EnqueueCollisionEvent(HitSubject src, HurtSubject dest, CollisionEventData evtData)
    {
        HitHurtPair pair = new()
        {
            Src = src,
            Dest = dest,
        };

        _collisionsByPair.Add(pair, evtData);
    }

    public bool IsCooling(HitSubject src)
    {
        return _cooldownByHit.ContainsKey(src);
    }

    public void BypassCurrentHit()
    {
        _bypassHit = true;
    }
}