using System.Collections.Generic;
using UnityEngine;

public class ActorExplosion : MonoBehaviour
{
    [SerializeField] private float OverlapRadius = 5f;
    [SerializeField] private float ShockwaveSpeed = 5f;
    [SerializeField] private LayerMask HurtLayerMask;

    private readonly List<Collider2D> _overlapBuffer = new();
    private ActorOld _actorOld;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, OverlapRadius);
    }

    private void Awake()
    {
        TryGetComponent(out _actorOld);
        _actorOld.OnPerceive += HandlePerceive;
    }

    private void OnEnable()
    {
        Vector2 currentPosition = transform.position;

        // gameplay
        ContactFilter2D filter = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = HurtLayerMask,
        };
        _overlapBuffer.Clear();
        Physics2D.OverlapCircle(currentPosition, OverlapRadius, filter, _overlapBuffer);
        foreach (Collider2D col in _overlapBuffer)
        {
            if (!col.TryGetActor(out ActorOld hurtSubject))
            {
                continue;
            }

            if (IFFTransponder.IsFriend(_actorOld, hurtSubject))
            {
                continue;
            }

            Vector2 contactPoint = col.ClosestPoint(currentPosition);
            Vector2 direction = contactPoint - currentPosition;
            HitEventData evt = new HitEventData
            {
                ContactPoint = contactPoint,
                HitVelocity = direction.normalized * ShockwaveSpeed,
            };
            ActorHitManager.EnqueueHitEvent(_actorOld, hurtSubject, evt);
        }
    }

    private void HandlePerceive(ActorOld actorOld)
    {
        // despawn when first tick
        actorOld.Kill();
    }
}