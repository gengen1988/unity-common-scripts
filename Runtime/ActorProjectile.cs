using System.Collections.Generic;
using UnityEngine;

public class ActorProjectile : MonoBehaviour
{
    [Header("Hit")] [SerializeField] private int HitCount = 1;

    [SerializeField] private float CastRadius = 0.5f;

    // [SerializeField] private int CastCount = 1;
    // [SerializeField] private float CastWidth;
    // [SerializeField] private float CastForward;
    [SerializeField] private float HitInterval = 0.1f;
    [SerializeField] private float HitstopTime = 0.1f;
    [SerializeField] private LayerMask HurtLayerMask;

    [Header("Gameplay")] [SerializeField] private Actor ExplosionOnDie;
    [SerializeField] private BuffProfile BuffOnHit;

    private float _lifeTime;
    private int _hitRemaining;
    private Vector2 _launchVelocity;
    private Rigidbody2D _rb;
    private readonly List<RaycastHit2D> _castBuffer = new();
    private ActorHitManager _hitManager;

    // private readonly Dictionary<Collider2D, RaycastHit2D> _grouper = new();

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, CastRadius);
    }

    private void Awake()
    {
        TryGetComponent(out _rb);
        TryGetComponent(out Actor actor);
        TryGetComponent(out _hitManager);
        actor.OnMove += HandleMove;
        _hitManager.OnHit += HandleHit;
    }

    private void HandleHit(Actor hitSubject, Actor hurtSubject, HitEventData evtData)
    {
        // hitstop
        hitSubject.Timer.ChangeTimeScale("hitstop", 0, HitstopTime);
        hurtSubject.Timer.ChangeTimeScale("hitstop", 0, HitstopTime);

        // hit count
        _hitRemaining--;
    }

    private void HandleMove(Actor moveSubject)
    {
        if (_lifeTime <= 0)
        {
            Die(moveSubject); // die for timeout
            return;
        }

        if (_hitRemaining <= 0)
        {
            Die(moveSubject); // die for hit
            return;
        }

        var velocity = _launchVelocity;
        var localDeltaTime = moveSubject.Timer.LocalDeltaTime;
        var displacement = velocity * localDeltaTime;
        var currentPosition = (Vector2)transform.position;
        if (!_hitManager.IsCooldown())
        {
            var direction = velocity.normalized;
            var distance = displacement.magnitude;
            var filter = new ContactFilter2D
            {
                useTriggers = true,
                useLayerMask = true,
                layerMask = HurtLayerMask,
            };

            // RaycastUtil.ParallelRaycastAll(
            //     castOrigin,
            //     CastCount,
            //     CastWidth,
            //     direction,
            //     distance,
            //     _grouper,
            //     _castBuffer
            // );

            // 不能用只找最近的 cast 对象的形式，因为最近的可能不是要命中的敌人
            Physics2D.CircleCast(currentPosition, CastRadius, direction, filter, _castBuffer, distance);
            // maybe sort before loop
            foreach (RaycastHit2D hit in _castBuffer)
            {
                if (!hit.collider.TryGetActor(out Actor hurtSubject))
                {
                    continue;
                }

                if (IFFTransponder.IsFriend(moveSubject, hurtSubject))
                {
                    continue;
                }

                // enqueue event
                HitEventData hitEventData = new HitEventData
                {
                    ContactPoint = currentPosition + direction * hit.distance,
                    HitVelocity = velocity,
                    CooldownTime = HitInterval,
                };
                ActorHitManager.EnqueueHitEvent(moveSubject, hurtSubject, hitEventData);

                // should not reduce hit remaining here, because hit colliders may come to one actor 

                // change destination for last hit
                if (_hitRemaining <= 1)
                {
                    displacement = direction * hit.distance;
                }

                // Debug.DrawLine(castOrigin, castOrigin + displacement);
                // DebugUtil.DrawWireCircle2D(castOrigin + displacement, CastRadius, Color.green);
            }
        }

        // Move the actor to destination
        _rb.MovePosition(currentPosition + displacement);
        _lifeTime -= localDeltaTime;
    }

    private void Die(Actor projectileActor)
    {
        if (!ActorManager.IsAlive(projectileActor))
        {
            Debug.LogError("projectile already died", this);
            return;
        }

        // Debug.Log($"[{Time.frameCount}] projectile({GetInstanceID()}) die", this);

        // gameplay
        if (ExplosionOnDie)
        {
            Transform projectileTransform = projectileActor.transform;
            Vector2 position = projectileTransform.position;
            Quaternion rotation = projectileTransform.rotation;
            Actor explosionActor = Actor.Spawn(ExplosionOnDie, position, rotation);
            IFFTransponder.CopyIdentity(projectileActor, explosionActor);
        }

        ActorManager.DespawnActor(projectileActor);
    }

    // 不能记录 shooter。比方说有时可能 shooter 先死掉，然后 shooter 发射的子弹才命中敌人
    public void Launch(Vector2 velocity, float lifeTime)
    {
        // Debug.Log($"[{Time.frameCount}] projectile({GetInstanceID()}) launch", this);

        // init self
        _lifeTime = lifeTime;
        _launchVelocity = velocity;
        _hitRemaining = HitCount;
    }
}