using System.Collections.Generic;
using UnityEngine;

public class ActorExplosion : MonoBehaviour
{
    [SerializeField] private float LifeTime = 1f;
    [SerializeField] private float OverlapRadius = 5f;
    [SerializeField] private float ShockwaveSpeed = 5f;
    [SerializeField] private LayerMask HurtLayerMask;

    private float _elapsedTime;
    private Actor _actor;

    private readonly List<Collider2D> _overlapBuffer = new();

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, OverlapRadius);
    }

    private void Awake()
    {
        _actor = this.EnsureComponent<Actor>();
        _actor.OnReady += HandleReady;
        _actor.OnPerceive += HandlePerceive;
    }

    private void OnEnable()
    {
        _elapsedTime = 0f;
    }

    private void HandleReady()
    {
        Vector2 currentPosition = transform.position;

        // gameplay
        var filter = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = HurtLayerMask,
        };
        _overlapBuffer.Clear();
        Physics2D.OverlapCircle(currentPosition, OverlapRadius, filter, _overlapBuffer);
        foreach (var col in _overlapBuffer)
        {
            if (!col.TryGetActor(out var hurtSubject))
            {
                continue;
            }

            if (IFFTransponder.IsFriend(_actor, hurtSubject))
            {
                continue;
            }

            var contactPoint = col.ClosestPoint(currentPosition);
            var direction = contactPoint - currentPosition;
            var hitVelocity = direction.normalized * ShockwaveSpeed;

            Debug.Log("enqueue explosion hit");
            // HitUtil.CreateHit(_hitCtrl, hurtSubject.gameObject, contactPoint, hitVelocity);
        }
    }

    private void HandlePerceive() // already scaled by actor
    {
        if (_elapsedTime >= LifeTime)
        {
            _actor.Kill();
            return;
        }

        _elapsedTime += _actor.LocalDeltaTime;
    }
}