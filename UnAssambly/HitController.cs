using System;
using UnityEngine;
using Weaver;

[GameFrameOrder(50)] // after other game logic, but before unity physics callbacks, ensure feedback no lag
public class HitController : MonoBehaviour, IEntityAttach, IEntityFrame
{
    [AssetReference] private static Feedback HitstopFeedback;

    public event Action<HitEventData> OnHit;
    public event Action<HitEventData> OnHurt;

    // [SerializeField] private float CooldownTime = 0.08f; // think about spin period

    // private float _cooldownTime;
    // private TimeController _timeCtrl;
    private readonly DictionaryList<GameEntity, HitEventData> _eventByHurtSubject = new();

    // private void Awake()
    // {
    //     TryGetComponent(out _timeCtrl);
    // }

    private void OnDestroy()
    {
        OnHit = null;
        OnHurt = null;
    }

    public void OnEntityAttach(GameEntity entity)
    {
        // _cooldownTime = 0;
        _eventByHurtSubject.Clear();
    }

    public void OnEntityFrame(GameEntity entity)
    {
        // cooling
        // if (_cooldownTime > 0)
        // {
        //     // _cooldownTime -= frameTime; // use unscaled time
        //     _cooldownTime -= _timeCtrl ? _timeCtrl.LocalDeltaTime : frameTime; // use optional local delta time
        //     return;
        // }

        // no hit happened
        if (_eventByHurtSubject.Keys.Count == 0)
        {
            return;
        }

        // event handling
        var hitEntity = gameObject.GetEntity();
        foreach (var hurtEntity in _eventByHurtSubject.Keys)
        {
            // remove recycled entity
            if (!hurtEntity.IsAlive())
            {
                continue;
            }

            // cleanup invalid events
            var events = _eventByHurtSubject[hurtEntity];
            events.RemoveAll(evt =>
            {
                if (!evt.HitEntity.IsAlive() || !evt.HurtEntity.IsAlive())
                {
                    return true;
                }

                if (evt.HitEntity != hitEntity)
                {
                    return true;
                }

                return false;
            });

            // merge
            var position = events.CenterOfMass(evt => evt.ContactPoint);
            var velocity = events.CenterOfMass(evt => evt.HitVelocity);
            var mergedEvent = new HitEventData
            {
                HitEntity = hitEntity,
                HurtEntity = hurtEntity,
                ContactPoint = position,
                HitVelocity = velocity,
            };

            // async hit procedure
            // Debug.Log($"[{Time.frameCount}] hit processed");
            var feedback = HitstopFeedback.Play();
            var hitstop = feedback.EnsureComponent<FeedbackHitstop>();
            hitstop.Setup(mergedEvent);

            // cooldown
            // _cooldownTime = CooldownTime;
        }

        // cleanup handled events
        _eventByHurtSubject.Clear();
    }

    public void EnqueueEvent(GameEntity hurtEntity, Vector2 contactPoint, Vector2 hitVelocity)
    {
        // if (!CanHit())
        // {
        //     return;
        // }

        // Debug.Log($"enqueue hurt: {hurtEntity}");
        var hitEntity = gameObject.GetEntity();
        var evt = new HitEventData
        {
            HitEntity = hitEntity,
            HurtEntity = hurtEntity,
            ContactPoint = contactPoint,
            HitVelocity = hitVelocity,
        };
        _eventByHurtSubject.Add(hurtEntity, evt);
    }

    // public bool CanHit()
    // {
    //     return _cooldownTime <= 0;
    // }

    public void NotifyHit(HitEventData evt)
    {
        OnHit?.Invoke(evt);
    }

    public void NotifyHurt(HitEventData evt)
    {
        OnHurt?.Invoke(evt);
    }
}

public static class HitUtil
{
    public static void CreateHit(
        Component hitSubject,
        Component hurtSubject,
        Vector2 contactPoint,
        Vector2 hitVelocity
    )
    {
        if (!hitSubject || !hurtSubject)
        {
            return;
        }

        var hitCtrl = hitSubject.EnsureComponent<HitController>();
        var hurtEntity = hurtSubject.gameObject.GetEntity();
        hitCtrl.EnqueueEvent(hurtEntity, contactPoint, hitVelocity);

        // Debug.Log($"[{Time.frameCount}] hit enqueued");
    }
}