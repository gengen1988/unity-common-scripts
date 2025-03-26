using System;
using System.Collections.Generic;
using UnityEngine;

// This script uses raycast to report hits
// Needs correct movement to contact surface
// it doesn't care hit unit or project, only care some can be hurt, or blocked (obstacle, like terrain)
public class Projectile : MonoBehaviour, IEntityFrame, IMoveState
{
    // private static readonly List<Hurtable> DicKeys = new();

    // [SerializeField] private float hitProtectTime = 1f;
    // [SerializeField] private int hitCount = 1;
    // [SerializeField] private float knockbackForce = 10f;

    [Header("Parallel Cast")]
    [SerializeField] private float castOffset;
    [SerializeField] private int castCount = 1;
    [SerializeField] private float castWidth;
    // [SerializeField] private LayerMask hurtLayerMask = LayerUtil.LayerToMask(CustomLayer.Hurtbox);

    [Header("Gameplay")]
    [SerializeField] private int defaultDamage = 100;
    [SerializeField] private HitstopType hitstopType = HitstopType.None;
    [SerializeField] private float knockbackSpeed = 5f;
    [SerializeField] private GameObject explodeOnHit;
    [SerializeField] private ModifierProfile buffOnHit;

    [Header("Cues")]
    [SerializeField] private CueChannel cueTrail;

    private float _lifeTime;
    private int _hitRemaining;
    private bool _isHitOver;
    private Hitable _hitSubject;
    private Rigidbody2D _rb;
    private IMovement<ProjectileContext> _movement;

    private readonly ProjectileContext _context = new();
    private readonly List<RaycastHit2D> _queryBuffer = new();

    private Guid _trailCue;
    // private readonly Dictionary<Hurtable, float> _hitProtections = new();

    public Vector2 Position => _context.Position;
    public Vector2 Velocity => _context.Velocity;

    private void OnDrawGizmosSelected()
    {
        // if (_movement != null)
        // {
        //     var deltaTime = Time.fixedDeltaTime;
        //     var displacement = _actor.Velocity * deltaTime;
        //     Gizmos.color = Color.green;
        //     RaycastUtil.DrawGizmosParallelRays(_actor.Position, castCount, castWidth, displacement);
        // }
        // else
        // {
        //     Gizmos.color = Color.red;
        //     RaycastUtil.DrawGizmosParallelRays(transform.position, castCount, castWidth, transform.right);
        // }
    }

    private void Awake()
    {
        TryGetComponent(out _rb);
        TryGetComponent(out _hitSubject);
        TryGetComponent(out _movement);
        _hitSubject.OnHitBegin += HandleHitBegin;
        _hitSubject.OnHitEnd += HandleHitEnd;
    }

    private void OnEnable()
    {
        _rb.isKinematic = true;
        _trailCue = cueTrail.PlayIfNotNull(transform);
    }

    private void OnDisable()
    {
        cueTrail.StopIfNotNull(_trailCue);
    }

    private void HandleHitBegin(HitInfo evt)
    {
        if (explodeOnHit)
        {
            PoolUtil.Spawn(explodeOnHit, transform.position, Quaternion.identity);
        }

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

        if (hurtEntity.Proxy.TryGetComponent(out ModifierManager modifierManager))
        {
            modifierManager.AddModifier(buffOnHit);
        }
    }

    private void HandleHitEnd(HitInfo evt)
    {
        _isHitOver = true;
    }

    public void OnEntityFrame(GameEntity entity, float deltaTime)
    {
        if (_isHitOver)
        {
            PoolUtil.Despawn(gameObject);
            return;
        }

        if (_movement == null)
        {
            Debug.LogAssertion("projectile never hit if no movement", this);
            return;
        }

        // do not move when hitting (just an optimize)
        if (_hitSubject.IsHitting)
        {
            return;
        }

        // Calculate move
        var currentPosition = _context.Position;
        _movement.Move(_context, deltaTime);

        var nextPosition = _context.Position;
        var displacement = nextPosition - currentPosition;
        var direction = displacement.normalized;
        var filter = new ContactFilter2D
        {
            useTriggers = true,
            useLayerMask = true,
            layerMask = 1 << CustomLayer.Hurtbox,
        };

        var castOrigin = currentPosition + direction * castOffset;
        var castDistance = displacement.magnitude;
        var castVector = direction * castDistance;
        RaycastUtil.ParallelRaycastAll(
            castOrigin,
            castCount,
            castWidth,
            direction,
            filter,
            _queryBuffer,
            castDistance
        );
        Debug.DrawLine(castOrigin, nextPosition, Color.red);

        // Find the nearest surface
        var hurtSubject = (Hurtable)null;
        foreach (var found in _queryBuffer)
        {
            var hurtTrans = found.collider.GetAttachedTransform();

            // Only apply to hurtable
            if (!hurtTrans.TryGetComponent(out Hurtable destination))
            {
                continue;
            }

            // Do not block by friends
            if (IFFTransponder.IsFriend(this, hurtTrans))
            {
                continue;
            }

            // Find nearest surface
            var hitVector = direction * found.distance;
            if (castVector.sqrMagnitude <= hitVector.sqrMagnitude)
            {
                continue;
            }

            hurtSubject = destination;
            castVector = hitVector;
        }

        // report hit and modify movement
        if (hurtSubject)
        {
            var contactPoint = currentPosition + castVector;
            // Debug.Log($"[{Time.frameCount}] {this} report hit {hurtSubject}", this);
            HitManager.Instance.ReportHit(_hitSubject, hurtSubject, new HitInfo
            {
                ContactPoint = contactPoint,
                HitVelocity = castVector,
                Hitstop = hitstopType,
            });

            // Update move state to reflect the collision
            _context.Position = contactPoint;
        }

        // apply movement
        _rb.MoveRotation(_context.Rotation);
        _rb.MovePosition(_context.Position);

        // lifetime (timed fuze)
        _lifeTime -= deltaTime;
        if (_lifeTime <= 0)
        {
            // Die for timeout
            PoolUtil.Despawn(gameObject);
        }
    }

    // We can't store the shooter reference. For example, sometimes the shooter might die
    // before the bullet hits an enemy
    public void Init(Vector2 initialVelocity, float lifeTime)
    {
        // Initialize self
        _lifeTime = lifeTime;
        _isHitOver = false;

        // init movement
        _context.Rotation = transform.rotation;
        _context.Position = transform.position;
        _context.Velocity = initialVelocity;

        // notify other gameplay
        GlobalEventBus<OnProjectileInit>.Raise(new OnProjectileInit
        {
            Subject = this
        });
    }
}