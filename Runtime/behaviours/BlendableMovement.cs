using System;
using UnityEngine;

public class BlendableMovement : MonoBehaviour
{
    public bool DebugLog;
    public bool ConstraintByRaycast;
    public PlatformerRaycaster _raycaster;

    private bool _isGrounded;
    private bool _requestStop;
    private Vector2 _deltaPosition;
    private Quaternion _deltaRotation;
    private Vector2 _velocity;
    private Vector2 _additionVelocity;
    private Vector2 _toBeApplyPosition;
    private Quaternion _toBeApplyRotation;
    private Vector2 _toBeApplyVelocity;

    private Rigidbody2D _rb;
    private IMoveHandler[] _handlers;
    private bool _overrideVelocity;

    public event Action OnGrounded;

    private void OnDrawGizmosSelected()
    {
        if (ConstraintByRaycast)
        {
            _raycaster.DrawGizmos(transform.position);
        }

        if (Application.isPlaying)
        {
            Gizmos.DrawRay(_rb.position, GetVelocity());
        }
    }

    private void Awake()
    {
        TryGetComponent(out _rb);
        Debug.Assert(_rb, this);
        Debug.Assert(_rb.interpolation == RigidbodyInterpolation2D.Interpolate, this);
        _handlers = this.GetAttachedComponents<IMoveHandler>();
    }

    private void OnEnable()
    {
        // reset for pooling
        _deltaPosition = Vector2.zero;
        _deltaRotation = Quaternion.identity;
        _velocity = Vector2.zero;

        // notify manager
        IComponentManager<BlendableMovement>.NotifyEnabled(this);
    }

    private void OnDisable()
    {
        IComponentManager<BlendableMovement>.NotifyDisabled(this);
    }

#if UNITY_EDITOR
    private void Start()
    {
        // 检查是否缺少必要系统
        Debug.Assert(SystemManager.GetSystem<MovementResolver>());
    }
#endif

    private void OnDestroy()
    {
        OnGrounded = null;
    }

    public void Tick(float deltaTime)
    {
        Vector2 currentPosition = GetPosition();
        Quaternion currentRotation = GetRotation();

        // game logic
        float timeScale = TimeCtrl.GetGameplayTimeScale(this);
        float scaledDeltaTime = timeScale * deltaTime;
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

            movement.OnMove(this, scaledDeltaTime);
        }

        // raycast
        Vector2 displacement = _deltaPosition;
        if (ConstraintByRaycast)
        {
            _isGrounded = _raycaster.Cast(currentPosition, ref displacement);
            if (_isGrounded)
            {
                // TODO bounce
                OnGrounded?.Invoke();
            }
        }

        // execute move
        _toBeApplyPosition = currentPosition + displacement;
        _toBeApplyRotation = currentRotation * _deltaRotation;
        _toBeApplyVelocity = displacement / deltaTime + _additionVelocity;

        // cleanup
        _deltaPosition = Vector2.zero;
        _additionVelocity = Vector2.zero;
        _deltaRotation = Quaternion.identity;
    }

    public void Commit()
    {
        // apply movement (use rb for interpolation)
        _rb.MovePosition(_toBeApplyPosition);
        _rb.MoveRotation(_toBeApplyRotation);
        _velocity = _toBeApplyVelocity;
        // Debug.Log($"{name} velocity: {_velocity}");
    }

    public Vector2 GetPosition()
    {
        return _rb.position;
    }

    public Quaternion GetRotation()
    {
        return MathUtil.QuaternionByAngle(_rb.rotation);
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

    public void AddVelocity(Vector2 velocity)
    {
        _additionVelocity += velocity;
    }
}