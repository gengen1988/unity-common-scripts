using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(EntityProxy), typeof(Hitable))]
public class Explosion : MonoBehaviour, IEntityAttach, IEntityFrame
{
    [SerializeField] private float radius = 5f;
    [FormerlySerializedAs("impactForce")] [SerializeField]
    private float knockbackSpeed = 1f;
    [SerializeField] private int defaultDamage = 100;
    [SerializeField] private CueChannel cue;

    private Hitable _hitable;
    private bool _isExploded;
    private readonly List<Collider2D> _queryBuffer = new();

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void Awake()
    {
        TryGetComponent(out _hitable);
        _hitable.OnHitBegin += HandleHitBegin;
    }

    private void OnEnable()
    {
        cue.PlayIfNotNull(transform.position);
    }

    private void HandleHitBegin(HitInfo evt)
    {
        _isExploded = true;

        var hurtEntity = evt.Participants.HurtEntity;
        if (!hurtEntity)
        {
            // hurt subject already die
            return;
            // EditorUtility.DisplayDialog("something happens", "hit callback got an already died hurt entity but why?", "OK");
            // Debug.Break();
        }

        if (hurtEntity.Proxy.TryGetComponent(out Health health))
        {
            health.DealDamage(defaultDamage);
        }

        if (knockbackSpeed > 0)
        {
            if (hurtEntity.Proxy.TryGetComponent(out Unit unit))
            {
                {
                    var velocity = evt.HitVelocity.normalized * knockbackSpeed;
                    unit.Knockback(velocity);
                }
            }
        }

        // if (hurtEntity.Proxy.TryGetComponent(out ModifierManager modifierManager))
        // {
        //     modifierManager.AddModifier(buffOnHit);
        // }
    }

    // next fixed update after creation
    public void OnEntityAttach(GameEntity entity)
    {
        _isExploded = false;
        var filter = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = 1 << CustomLayer.Hurtbox,
        };
        var explosionCenter = transform.position;
        Physics2D.OverlapCircle(explosionCenter, radius, filter, _queryBuffer);
        foreach (var found in _queryBuffer)
        {
            var attachTrans = found.GetAttachedTransform();
            if (!attachTrans.TryGetComponent(out Hurtable hurtable))
            {
                continue;
            }

            var hurtCenter = attachTrans.position;
            var los = hurtCenter - explosionCenter;
            HitManager.Instance.ReportHit(_hitable, hurtable, new HitInfo
            {
                ContactPoint = hurtCenter,
                HitVelocity = los.normalized,
                Hitstop = HitstopType.Light,
            });
        }
    }

    public void OnEntityFrame(GameEntity entity, float deltaTime)
    {
        if (_isExploded)
        {
            PoolUtil.Despawn(gameObject);
        }
    }
}