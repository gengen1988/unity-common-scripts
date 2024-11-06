using System;
using UnityEngine;

public class MoveSubjectTimeBak : MonoBehaviour
{
    public bool DebugLog;
    public bool Interpolation;

    [Header("Platformer")]
    public bool ConstraintByRaycast;
    public LayerMask GroundMask;
    public LayerMask PlatformMask;
    public PlatformerRaycaster Raycaster;

    private bool _isGrounded;
    private bool _requestStop;
    private Vector2 _deltaPosition;
    private Quaternion _deltaRotation;
    private Vector2 _velocity;
    private Vector2 _additionalVelocity;
    private Vector2 _toBeApplyPosition;
    private Quaternion _toBeApplyRotation;
    private Vector2 _toBeApplyVelocity;

    private Transform _self;
    private Rigidbody2D _rb;
    private IMoveHandler[] _handlers;
    private bool _overrideVelocity;
    private bool _downFromPlatform;

    public event Action OnGrounded;

    private void OnDrawGizmosSelected()
    {
        if (ConstraintByRaycast)
        {
            Raycaster.DrawGizmos(transform.position);
        }

        if (Application.isPlaying)
        {
            Gizmos.DrawRay(_rb.position, GetVelocity());
        }
    }

    private void Awake()
    {
        if (Interpolation)
        {
            gameObject.EnsureComponent<TransformInterpolator>();
        }

        _self = transform;
        TryGetComponent(out _rb);
        _handlers = this.GetAttachedComponents<IMoveHandler>();
    }

    private void OnEnable()
    {
        // reset for pooling
        _deltaPosition = Vector2.zero;
        _deltaRotation = Quaternion.identity;
        _velocity = Vector2.zero;

        // notify manager
        // IComponentManager<MoveSubject>.NotifyEnabled(this);
    }

    private void OnDisable()
    {
        // IComponentManager<MoveSubject>.NotifyDisabled(this);
    }

#if UNITY_EDITOR
    private void Start()
    {
        // 检查是否缺少必要系统
        // Debug.Assert(SystemManager.GetSystem<MovementManager>());
        // Debug.Assert(_rb, "move subject requires RigidBody2D to work", this);
    }
#endif

    private void OnDestroy()
    {
        OnGrounded = null;
    }

    public void DownFromPlatform()
    {
        _downFromPlatform = true;
    }

    public void Tick(float deltaTime)
    {
        Vector2 currentPosition = GetPosition();
        Quaternion currentRotation = GetRotation();

        // game logic
        float timeScale = 1f;
        // float timeScale = TimeCtrl.GetGameplayTimeScale(this);
        float scaledDeltaTime = timeScale * deltaTime;
        _downFromPlatform = false;
        foreach (IMoveHandler movement in _handlers)
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

            // movement.OnMove(this, scaledDeltaTime);
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
                OnGrounded?.Invoke();
            }
        }

        // execute move
        _toBeApplyPosition = currentPosition + displacement;
        _toBeApplyRotation = currentRotation * _deltaRotation;
        _toBeApplyVelocity = displacement / deltaTime + _additionalVelocity;

        // cleanup
        _additionalVelocity = Vector2.zero;
        _deltaPosition = Vector2.zero;
        _deltaRotation = Quaternion.identity;
    }

    public void Commit()
    {
        // apply movement (use rb for interpolation)
        _rb.MovePosition(_toBeApplyPosition);
        _rb.MoveRotation(_toBeApplyRotation);
        _velocity = _toBeApplyVelocity;

        if (Interpolation)
        {
            _self.position = _toBeApplyPosition;
            _self.rotation = _toBeApplyRotation;
        }
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
}