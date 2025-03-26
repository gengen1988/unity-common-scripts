using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using Weaver;

// why would need merge:
// - multiple part of one hit subject (multiple collider for complex shape)
// - multiple part of one hurt subject (body, arm, leg, ..etc)
public class HitManager : WeaverSingletonBehaviour<HitManager>
{
    [AssetReference] private static readonly HitManager SingletonPrefab;

    [SerializeField] private float hitstopTimeScale = .1f;
    [SerializeField] private float lightHitstopTime = .08f; // 4 game frames by default
    [SerializeField] private float heavyHitstopTime = .2f; // 10 game frames by default
    [SerializeField] private float criticalDurationScale = 1.3f;

    private readonly List<HitInfo> _pendingEvents = new();
    private readonly List<HitPair> _hittingEvents = new();
    private readonly Dictionary<HitPair, float> _hitTimer = new();

    private void FixedUpdate()
    {
        // tick hitstop
        _hittingEvents.Clear();
        _hittingEvents.AddRange(_hitTimer.Keys);
        var frameTime = Time.fixedDeltaTime;
        foreach (var key in _hittingEvents)
        {
            var remainingTime = _hitTimer[key];
            _hitTimer[key] = remainingTime - frameTime;
        }
    }

    private void Update()
    {
        ProcessEvents(); // means to after physics engine callbacks (e.g. OnTriggerEnter)
    }

    /// you should call this in fixed update or physics callback
    public void ReportHit(Hitable source, Hurtable destination, HitInfo evt)
    {
        // take snapshot
        var pair = new HitPair
        {
            HitEntity = source.CurrentEntity,
            HitSubject = source,
            HurtEntity = destination.CurrentEntity,
            HurtSubject = destination,
        };
        evt.Participants = pair;

        // Add to current frame's pending hits
        _pendingEvents.Add(evt);
    }

    private void ProcessEvents()
    {
        if (_pendingEvents.Count > 0)
        {
            // start new hitstop
            var mergedEvents = MergeEvents(_pendingEvents);
            foreach (var evtData in mergedEvents)
            {
                HitProcedure(evtData).Forget();
            }

            _pendingEvents.Clear();
        }
    }

    private List<HitInfo> MergeEvents(List<HitInfo> events)
    {
        return events
            .Where(evt => !_hitTimer.ContainsKey(evt.Participants)) // no hitting
            .GroupBy(evt => evt.Participants).Select(group =>
            {
                var pair = group.Key;
                var contactPoint = group.CenterOfMass(evt => evt.ContactPoint);
                var hitVelocity = group.CenterOfMass(evt => evt.HitVelocity);

                // merge hitstop
                var hitstop = HitstopType.None;
                foreach (var evt in group)
                {
                    if (evt.Hitstop == HitstopType.Heavy)
                    {
                        hitstop = HitstopType.Heavy;
                        break;
                    }

                    if (evt.Hitstop == HitstopType.Light && hitstop == HitstopType.None)
                    {
                        hitstop = HitstopType.Light;
                        continue;
                    }
                }

                // merge critical
                var isCritical = group.Any(evt => evt.IsCritical);

                // build merged event
                return new HitInfo
                {
                    Participants = pair,
                    ContactPoint = contactPoint,
                    HitVelocity = hitVelocity,
                    Hitstop = hitstop,
                    IsCritical = isCritical,
                };
            })
            .ToList();
    }

    private async UniTask HitProcedure(HitInfo evt)
    {
        var hitEntity = evt.Participants.HitEntity;
        var hurtEntity = evt.Participants.HurtEntity;
        var hitSubject = evt.Participants.HitSubject;
        var hurtSubject = evt.Participants.HurtSubject;

        // hitstop leveling
        _hitTimer[evt.Participants] = evt.Hitstop switch
        {
            HitstopType.Light => lightHitstopTime,
            HitstopType.Heavy => heavyHitstopTime,
            _ => 0,
        };

        // critical scale
        if (evt.IsCritical)
        {
            _hitTimer[evt.Participants] *= criticalDurationScale;
        }

        if (hitEntity)
        {
            hitEntity.Proxy.Clock.SetBaseTimeScale(hitstopTimeScale);
            hitSubject.BeginHit(evt);
        }

        if (hurtEntity)
        {
            hurtEntity.Proxy.Clock.SetBaseTimeScale(hitstopTimeScale);
            hurtSubject.BeginHurt(evt);
        }

        // wait hitstop end
        while (_hitTimer[evt.Participants] > 0)
        {
            await UniTask.Yield(PlayerLoopTiming.LastUpdate); // in next frame, after ProcessHits, prevent edge case
        }

        if (hitEntity)
        {
            hitEntity.Proxy.Clock.SetBaseTimeScale(1);
            hitSubject.EndHit(evt);
        }

        if (hurtEntity)
        {
            hurtEntity.Proxy.Clock.SetBaseTimeScale(1);
            hurtSubject.EndHurt(evt);
        }

        _hitTimer.Remove(evt.Participants);
    }
}

// critical will scale frames one the base 
public enum HitstopType
{
    None,
    Light,
    Heavy,
}