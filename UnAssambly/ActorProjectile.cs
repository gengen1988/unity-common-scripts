using System.Collections.Generic;
using UnityEngine;

public class ActorProjectile : MonoBehaviour
{
    private static readonly List<Actor> DicKeys = new();

    [Header("Hit")]
    [SerializeField] private float HitProtectTime = 1f;
    [SerializeField] private int HitCount = 1;
    [SerializeField] private float KnockbackForce = 10f;
    [SerializeField] private int CastCount = 1;
    [SerializeField] private float CastWidth;
    [SerializeField] private LayerMask HurtLayerMask; // Layers.HurtboxMask;

    [Header("Gameplay")]
    [SerializeField] private Actor ExplosionOnDie;
    [SerializeField] private BuffProfile BuffOnHit;

    private float _lifeTime;
    private int _hitRemaining;
    private readonly List<RaycastHit2D> _raycastBuffer = new();
    private readonly Dictionary<Actor, float> _hitProtections = new();

    private Actor _actor;
    private HitController _hitCtrl;
    private ArcadeMovement _movement;

    private void OnDrawGizmosSelected()
    {
        if (_movement)
        {
            var position = _movement.Position;
            var displacement = _movement.PredictNextPosition(Time.fixedDeltaTime) - position;
            Gizmos.color = Color.green;
            RaycastUtil.DrawGizmosParallelRays(position, CastCount, CastWidth, displacement);
        }
        else
        {
            Gizmos.color = Color.red;
            RaycastUtil.DrawGizmosParallelRays(transform.position, CastCount, CastWidth, transform.right);
        }
    }

    private void Awake()
    {
        _actor = this.EnsureComponent<Actor>();
        _hitCtrl = this.EnsureComponent<HitController>();
        _movement = this.EnsureComponent<ArcadeMovement>();
        _hitCtrl.OnHit += HandleHit;
        _actor.OnMove += HandleMove;
    }

    private void HandleHit(HitEventData evt)
    {
        // knockback
        if (evt.HurtEntity.IsAlive())
        {
            var hurtBridge = evt.HurtEntity.GetBridge();
            var knockback = hurtBridge.EnsureComponent<ActorKnockback>();
            knockback.Knockback(evt.HitVelocity.normalized * KnockbackForce);
        }
        else
        {
            Debug.LogWarning("hurt entity already dead");
        }
    }

    private void HandleMove()
    {
        if (_lifeTime <= 0)
        {
            Die(); // die for timeout
            return;
        }

        if (_hitRemaining <= 0)
        {
            Die();
            return;
        }

        // reduce hit protection time
        var deltaTime = _actor.LocalDeltaTime;
        DicKeys.Clear();
        DicKeys.AddRange(_hitProtections.Keys);
        foreach (var hurtActor in DicKeys)
        {
            var cooldownTime = _hitProtections[hurtActor];
            var remainingTime = cooldownTime - deltaTime;
            if (remainingTime <= 0)
            {
                _hitProtections.Remove(hurtActor);
            }
            else
            {
                _hitProtections[hurtActor] = remainingTime;
            }
        }

        // physics query
        var currentPosition = _movement.Position;
        var nextPosition = _movement.PredictNextPosition(deltaTime);
        var displacement = nextPosition - currentPosition;
        var direction = displacement.normalized;
        var filter = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = HurtLayerMask,
        };

        var castOrigin = currentPosition; // + direction * ShellThickness;
        var castDistance = displacement.magnitude; //- ShellThickness;
        var castVector = direction * castDistance;
        RaycastUtil.ParallelRaycastAll(
            castOrigin,
            CastCount,
            CastWidth,
            direction,
            filter,
            _raycastBuffer,
            castDistance
        );
        Debug.DrawLine(castOrigin, nextPosition, Color.white);

        // find the nearest surface
        Actor hittingActor = null;
        foreach (var hit in _raycastBuffer)
        {
            // only apply to actor
            if (!hit.collider.TryGetActor(out var hurtActor))
            {
                continue;
            }

            // do not block by friend
            if (IFFTransponder.IsFriend(this, hurtActor))
            {
                continue;
            }

            // do not block by killed actor
            if (hurtActor.IsKilled())
            {
                continue;
            }

            // do not block by penetrating
            if (_hitProtections.ContainsKey(hurtActor))
            {
                continue;
            }

            // find nearest surface
            var hitVector = direction * hit.distance;
            if (castVector.sqrMagnitude <= hitVector.sqrMagnitude)
            {
                continue;
            }

            hittingActor = hurtActor;
            castVector = hitVector;
        }

        // hit and modify movement
        if (hittingActor)
        {
            var contactPoint = currentPosition + castVector; // + direction * ShellThickness;
            HitUtil.CreateHit(this, hittingActor, contactPoint, _movement.Velocity);
            _movement.SetPosition(contactPoint);
            _hitProtections[hittingActor] = HitProtectTime;
            _hitRemaining--;
            // Debug.Log($"[{Time.frameCount}] hit {hittingActor}({hittingActor.GetInstanceID()}) at {contactPoint}");
            // DebugTools.DrawCross(contactPoint, Color.red);
        }

        // reduce lifetime
        _lifeTime -= deltaTime;
    }

    private void Die()
    {
        if (_actor.IsKilled())
        {
            return;
        }

        // gameplay
        // if (ExplosionOnDie)
        // {
        //     Vector2 position = transform.position;
        //     var rotation = transform.rotation;
        //     var explosion = ExplosionOnDie.Spawn(position, rotation);
        //     IFFTransponder.CopyIdentity(this, explosion);
        // }

        _actor.Kill();
    }

    // 不能记录 shooter。比方说有时可能 shooter 先死掉，然后 shooter 发射的子弹才命中敌人
    public void Launch(Vector2 initialVelocity, float lifeTime)
    {
        // init self
        _lifeTime = lifeTime;
        _hitRemaining = HitCount;
        _movement.SetInitialVelocity(initialVelocity);
        _hitProtections.Clear();
    }
}