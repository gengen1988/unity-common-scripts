using System.Collections.Generic;
using UnityEngine;

/**
 * such as poison cloud, multiple overlap doesn't count
 * 1. use a global dummy actor as hit subject
 * 2. each hit handler use different merge method, field use a key string to combine hit events
 */
public class ActorField : MonoBehaviour
{
    // [SerializeField] private float OverlapRadius = 5f;
    // [SerializeField] private float HitInterval = 0.1f;
    // [SerializeField] private LayerMask HurtLayerMask;
    //
    // private ActorHitManager _hitManager;
    // private readonly List<Collider2D> _overlapBuffer = new();
    //
    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.DrawWireSphere(transform.position, OverlapRadius);
    // }
    //
    // private void Awake()
    // {
    //     TryGetComponent(out ActorOld actor);
    //     TryGetComponent(out _hitManager);
    //     actor.OnMove += HandleMove;
    // }
    //
    // private void HandleMove(ActorOld hitSubject)
    // {
    //     if (_hitManager.IsCooldown())
    //     {
    //         return;
    //     }
    //
    //     var overlapCenter = (Vector2)transform.position;
    //     var filter = new ContactFilter2D
    //     {
    //         useTriggers = true,
    //         useLayerMask = true,
    //         layerMask = HurtLayerMask,
    //     };
    //     _overlapBuffer.Clear();
    //     Physics2D.OverlapCircle(overlapCenter, OverlapRadius, filter, _overlapBuffer);
    //     foreach (var col in _overlapBuffer)
    //     {
    //         if (!col.TryGetActor(out var hurtSubject))
    //         {
    //             continue;
    //         }
    //
    //         if (IFFTransponder.IsFriend(hitSubject, hurtSubject))
    //         {
    //             continue;
    //         }
    //
    //         var evt = new HitEventData
    //         {
    //             ContactPoint = col.bounds.center,
    //             HitVelocity = Vector2.zero,
    //             CooldownTime = HitInterval,
    //         };
    //         // ActorHitManager.EnqueueHitEvent(hitSubject, hurtSubject, evt);
    //     }
    // }
}