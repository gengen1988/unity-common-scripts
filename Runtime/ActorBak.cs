using System;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
[DisallowMultipleComponent]
public class ActorBak : MonoBehaviour
{
    /*
    public bool DebugLog;

    // hit
    public float HitInterval = 0.1f;
    public bool Penetrating;
    public int HurtPriority;

    // platform
    [Header("Platformer")]
    public bool ConstraintByRaycast;
    public LayerMask GroundMask;
    public LayerMask PlatformMask;
    public PlatformerRaycaster Raycaster;

    // movement
    private bool _isGrounded;
    private bool _requestStop;
    private Vector2 _deltaPosition;
    private Quaternion _deltaRotation;
    private Vector2 _velocity;
    private Vector2 _additionalVelocity;
    private Vector2 _toBeApplyPosition;
    private Quaternion _nextRotation;
    private Vector2 _nextVelocity;
    private Vector3 _nextPosition;
    private bool _overrideVelocity;
    private bool _downFromPlatform;

    // collision
    private bool _bypassHit;
    private bool _bypassMove;
    private bool _allowBypass;
    private float _coolingTime;
    private readonly List<ContactPoint2D> _contactBuffer = new();
    private readonly DictionaryList<Actor, CollisionEventData> _eventsByHurtSubject = new();

    private Transform _self;
    private LocalClock _localTime;
    private Rigidbody2D _rb;

    // event callbacks
    private IMoveHandler2[] _moveHandlers;
    private IHitHandler2[] _hitHandlers;
    private IHurtHandler2[] _hurtHandlers;
    private IGroundHandler[] _groundHandlers;
    public bool IsMoveBypassed => _bypassMove;

    private void Awake()
    {
        _self = transform;
        TryGetComponent(out _localTime);
        TryGetComponent(out _rb);
        _hitHandlers = this.GetAttachedComponents<IHitHandler2>();
        _hurtHandlers = this.GetAttachedComponents<IHurtHandler2>();
        _moveHandlers = this.GetAttachedComponents<IMoveHandler2>();
        _groundHandlers = this.GetAttachedComponents<IGroundHandler>();
    }

    private void OnEnable()
    {
        IComponentManager<Actor>.NotifyEnabled(this);
    }

    private void OnDisable()
    {
        IComponentManager<Actor>.NotifyEnabled(this);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        TryEnqueueCollisionEvent(other);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        TryEnqueueCollisionEvent(other);
    }

    public void MovementTick(float deltaTime)
    {
        if (_localTime.timeScale <= 0)
        {
            return;
        }

        if (DebugLog)
        {
            Debug.Log($"local time scale: {_localTime.timeScale}");
        }

        Vector2 currentPosition = _self.position;
        Quaternion currentRotation = _self.rotation;

        // reset state
        _downFromPlatform = false;

        // game logic
        float localDeltaTime = deltaTime * _localTime.timeScale;
        foreach (IMoveHandler2 movement in _moveHandlers)
        {
            if (movement is not Behaviour unityBehaviour)
            {
                continue;
            }

            if (!unityBehaviour.isActiveAndEnabled)
            {
                continue;
            }

            if (DebugLog)
            {
                Debug.Log(
                    $"[{Time.frameCount}] {gameObject.name}({gameObject.GetInstanceID()}) move by {movement.GetType()}",
                    this
                );
            }

            movement.OnMove(this, localDeltaTime);
        }

        // raycast
        Vector2 displacement = _deltaPosition;
        if (ConstraintByRaycast)
        {
            // solid ground
            _isGrounded = Raycaster.Cast(currentPosition, GroundMask, ref displacement);

            if (!_downFromPlatform)
            {
                // one way platform
                if (_velocity.y < 0 && !_isGrounded)
                {
                    _isGrounded = Raycaster.Cast(currentPosition, PlatformMask, ref displacement);
                }
            }


            if (_isGrounded)
            {
                NotifyGroundEvent(this);
            }
        }

        // calc next states
        _nextPosition = currentPosition + displacement;
        _nextRotation = currentRotation * _deltaRotation;
        _nextVelocity = displacement / deltaTime + _additionalVelocity; // unscaled deltaTime observed by others

        // cleanup
        _additionalVelocity = Vector2.zero;
        _deltaPosition = Vector2.zero;
        _deltaRotation = Quaternion.identity;
    }

    public void MovementCommit()
    {
        if (_localTime.timeScale <= 0)
        {
            return;
        }

        _rb.MovePosition(_nextPosition);
        _rb.MoveRotation(_nextRotation);
        _velocity = _nextVelocity;
    }

    public void CollisionTick(float deltaTime)
    {
        // if (_localTime.timeScale <= 0)
        // {
        //     return;
        // }

        float localDeltaTime = deltaTime * _localTime.timeScale;
        if (_coolingTime > 0)
        {
            _coolingTime -= localDeltaTime;
            return;
        }

        if (_eventsByHurtSubject.Keys.Count == 0)
        {
            return;
        }

        if (Penetrating)
        {
            foreach (Actor hurtSubject in _eventsByHurtSubject.Keys)
            {
                HandleCollisionEvents(this, hurtSubject);
            }
        }
        else
        {
            Actor minPriorityHurtSubject = null;
            foreach (Actor hurtSubject in _eventsByHurtSubject.Keys)
            {
                if (!minPriorityHurtSubject || hurtSubject.HurtPriority < minPriorityHurtSubject.HurtPriority)
                {
                    minPriorityHurtSubject = hurtSubject;
                }
            }

            HandleCollisionEvents(this, minPriorityHurtSubject);
        }

        _eventsByHurtSubject.Clear();
    }

    public void BypassCurrentHit()
    {
        if (!_allowBypass)
        {
            Debug.LogError("Cannot bypass at this time.");
            return;
        }

        _bypassHit = true;
    }

    public void BypassCurrentMove()
    {
        Debug.Log("request bypass move");
        _bypassMove = true;
    }

    public void ResetBypassMove()
    {
        _bypassMove = false;
    }

    public Vector2 GetPosition()
    {
        return _self.position;
    }

    public Quaternion GetRotation()
    {
        return _self.rotation;
    }

    public Vector2 GetVelocity()
    {
        return _velocity;
    }

    public void MovePositionDelta(Vector2 deltaPosition)
    {
        _deltaPosition += deltaPosition;
    }

    public void MoveRotationDelta(Quaternion deltaRotation)
    {
        _deltaRotation *= deltaRotation;
    }

    public void AddVelocityCorrection(Vector2 correction)
    {
        _additionalVelocity += correction;
    }

    public void Accelerate(Vector2 acceleration, float deltaTime)
    {
        Vector2 deltaVelocity = acceleration * deltaTime;
        Vector2 deltaVelocityMidpoint = deltaVelocity * 0.5f;
        Vector2 displacementByAcceleration = deltaVelocityMidpoint * deltaTime;
        // Debug.Log($"acceleration: {acceleration}, displacement by acceleration: {displacementByAcceleration}");
        _deltaPosition += displacementByAcceleration;
        _additionalVelocity += deltaVelocityMidpoint;
    }

    public bool IsGrounded()
    {
        return _isGrounded;
    }

    private void TryEnqueueCollisionEvent(Collision2D other)
    {
        // cooling optimize
        if (_coolingTime > 0)
        {
            return;
        }

        // filter by hurtbox
        if (!TryGetActor(other.collider, out Actor hurtSubject))
        {
            return;
        }

        // calculate contact point
        other.GetContacts(_contactBuffer);
        Vector2 contactPoint = _contactBuffer.CenterOfMass(contact => contact.point);

        // create event and enqueue, to handling multipart hitbox or hurtbox
        CollisionEventData evtData = new()
        {
            HurtStamp = PoolWrapper.GetStamp(hurtSubject),
            ContactPoint = contactPoint,
            HitVelocity = -other.relativeVelocity,
        };
        _eventsByHurtSubject.Add(hurtSubject, evtData);
    }

    private void HandleCollisionEvents(Actor hitSubject, Actor hurtSubject)
    {
        LinkedList<CollisionEventData> events = _eventsByHurtSubject[hurtSubject];
        int hitCount = 0;
        Vector2 contactPoint = Vector2.zero;
        Vector2 hitVelocity = Vector2.zero;
        foreach (CollisionEventData evt in events)
        {
            if (!PoolWrapper.IsAlive(hurtSubject, evt.HurtStamp))
            {
                continue;
            }

            contactPoint += evt.ContactPoint;
            hitVelocity += evt.HitVelocity;
            hitCount++;
        }

        if (hitCount == 0)
        {
            return;
        }

        CollisionEventData mergedEvent = new()
        {
            HurtStamp = PoolWrapper.GetStamp(hurtSubject),
            ContactPoint = contactPoint / hitCount,
            HitVelocity = hitVelocity / hitCount,
        };

        _bypassHit = false;
        _allowBypass = true;
        NotifyHitEvent(hitSubject, hurtSubject, mergedEvent);
        _allowBypass = false;
        if (_bypassHit)
        {
            return;
        }

        hurtSubject.NotifyHurtEvent(hitSubject, hurtSubject, mergedEvent);
        _coolingTime = HitInterval;
    }

    private bool TryGetActor(Collider2D from, out Actor actor)
    {
        // rigidbody mode
        if (from.attachedRigidbody)
        {
            return from.attachedRigidbody.TryGetComponent(out actor);
        }

        // collider (static) mode
        return from.TryGetComponent(out actor);
    }

    private void NotifyHitEvent(Actor hitSubject, Actor hurtSubject, CollisionEventData evtData)
    {
        foreach (IHitHandler2 handler in _hitHandlers)
        {
            if (handler is not Behaviour unityBehaviour)
            {
                continue;
            }

            if (!unityBehaviour.isActiveAndEnabled)
            {
                continue;
            }

            handler.OnHit(hitSubject, hurtSubject, evtData);
        }
    }

    private void NotifyHurtEvent(Actor hitSubject, Actor hurtSubject, CollisionEventData evtData)
    {
        foreach (IHurtHandler2 handler in _hurtHandlers)
        {
            if (handler is not Behaviour unityBehaviour)
            {
                continue;
            }

            if (!unityBehaviour.isActiveAndEnabled)
            {
                continue;
            }

            handler.OnHurt(hitSubject, hurtSubject, evtData);
        }
    }

    private void NotifyGroundEvent(Actor actor)
    {
        foreach (IGroundHandler handler in _groundHandlers)
        {
            if (handler is not Behaviour unityBehaviour)
            {
                continue;
            }

            if (!unityBehaviour.isActiveAndEnabled)
            {
                continue;
            }

            handler.OnGrounded(actor);
        }
    }
    */
}