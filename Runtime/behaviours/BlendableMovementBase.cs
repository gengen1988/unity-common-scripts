using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BlendableMovementBase : MonoBehaviour, IMoveSubject
{
    public bool DebugLog;
    public bool ConstraintByRaycast;
    public PlatformerRaycaster _raycaster;

    private bool _isGrounded;
    private Vector2 _velocity;
    private Vector2 _deltaVelocity;
    private Vector2 _deltaPosition;
    private Quaternion _deltaRotation;
    private Rigidbody2D _rb;
    public event Action OnGrounded;

    private Vector2 _targetPosition;

    private readonly List<IMoveHandler> _handlers = new();


    private void OnDrawGizmosSelected()
    {
        if (ConstraintByRaycast)
        {
            _raycaster.DrawGizmos(transform.position);
        }
    }

    private void Awake()
    {
        TryGetComponent(out _rb);
        Debug.Assert(_rb, this);
        UnityUtil.FindAttachedComponents(this, _handlers);
    }

    private void OnEnable()
    {
        // reset for pooling
        _deltaPosition = Vector2.zero;
        _deltaRotation = Quaternion.identity;
    }

    private void OnDestroy()
    {
        OnGrounded = null;
    }

    private void FixedUpdate()
    {
        Vector2 currentPosition = GetPosition();
        Quaternion currentRotation = GetRotation();

        // game logic
        float timeScale = TimeCtrl.GetGameplayTimeScale(this);
        float deltaTime = timeScale * Time.deltaTime;
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

            movement.OnMove(this, deltaTime);
        }

        // raycast
        Vector2 displacement = _deltaPosition;
        if (ConstraintByRaycast)
        {
            _isGrounded = _raycaster.Cast(currentPosition, ref displacement);
            if (_isGrounded)
            {
                OnGrounded?.Invoke();
            }
        }

        // execute move
        Vector2 newPosition = currentPosition + displacement;
        Quaternion newRotation = currentRotation * _deltaRotation;

        // apply movement (use rb for interpolation)
        Debug.Assert(_rb.isKinematic, "rigidbody should be kinematic", _rb);
        _rb.MovePosition(newPosition);
        _rb.MoveRotation(newRotation);
        _targetPosition = newPosition;
        _velocity = displacement / deltaTime + 0.5f * _deltaVelocity;

        // cleanup
        _deltaPosition = Vector2.zero;
        _deltaRotation = Quaternion.identity;
        _deltaVelocity = Vector2.zero;
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

    public Vector2 GetTargetPosition()
    {
        return _targetPosition;
    }

    public void MovePositionDelta(Vector2 deltaPosition)
    {
        _deltaPosition += deltaPosition;
    }

    public void MoveRotationDelta(Quaternion deltaRotation)
    {
        _deltaRotation *= deltaRotation;
    }

    public void MoveKinematic(Vector2 velocity, Vector2 acceleration, float deltaTime)
    {
        Vector2 deltaVelocity = acceleration * deltaTime;
        Vector2 deltaPosition = deltaTime * (velocity + .5f * deltaVelocity);
        _deltaPosition += deltaPosition;
        _deltaVelocity += deltaVelocity;
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