using System;
using System.Linq;
using UnityEngine;

public class BlendableMovement : MonoBehaviour, IManagedMovement, IMoveSubject
{
    public bool DebugLog;
    public bool ConstraintByRaycast;
    public PlatformerRaycaster _raycaster;

    private bool _isGrounded;
    private bool _isVelocityReset;
    private Vector2 _deltaPosition;
    private Quaternion _deltaRotation;
    private Vector2 _velocity;
    private Vector2 _toBeApplyPosition;
    private Quaternion _toBeApplyRotation;
    private Vector2 _toBeApplyVelocity;

    private Rigidbody2D _rb;
    private IMoveHandler[] _handlers;
    private MovementResolver _manager;

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
        _isVelocityReset = true;

        // register
        _manager = SystemManager.GetSystem<MovementResolver>();
        _manager.RegisterSubject(this);
    }

    private void OnDisable()
    {
        if (_manager)
        {
            _manager.RemoveSubject(this);
        }
    }

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
                _isVelocityReset = true;
                OnGrounded?.Invoke();
            }
        }

        // execute move
        _toBeApplyPosition = currentPosition + displacement;
        _toBeApplyRotation = currentRotation * _deltaRotation;
        if (_isVelocityReset)
        {
            _toBeApplyVelocity = displacement / scaledDeltaTime;
        }
        else
        {
            _toBeApplyVelocity = 2 * displacement / scaledDeltaTime - _velocity;
        }

        // cleanup
        _deltaPosition = Vector2.zero;
        _deltaRotation = Quaternion.identity;
        _isVelocityReset = false;
    }

    public void Commit()
    {
        // apply movement (use rb for interpolation)
        _rb.MovePosition(_toBeApplyPosition);
        _rb.MoveRotation(_toBeApplyRotation);
        _velocity = _toBeApplyVelocity;
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

    /**
     * set velocity if movement is not produce by acceleration.
     * beware this method do not generate any displacement
     */
    public void SetVelocity(Vector2 velocity)
    {
        _velocity = velocity;
    }

    public void MovePositionDelta(Vector2 deltaPosition)
    {
        _deltaPosition += deltaPosition;
    }

    public void MoveRotationDelta(Quaternion deltaRotation)
    {
        _deltaRotation *= deltaRotation;
    }

    public void ResetVelocity()
    {
        _isVelocityReset = true;
    }

    public Rigidbody2D GetRigidbody()
    {
        return _rb;
    }

    public T GetMovement<T>() where T : IMoveHandler
    {
        return _handlers
            .Where(handler => handler is T)
            .Cast<T>()
            .First();
    }
}