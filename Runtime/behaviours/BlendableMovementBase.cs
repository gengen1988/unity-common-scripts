using System;
using UnityEngine;

public abstract class BlendableMovementBase : MonoBehaviour, IMoveSubject
{
    public bool ConstraintByRaycast;
    public PlatformerRaycaster _raycaster;

    private bool _isGrounded;
    private Vector2 _velocity;
    private Vector2 _linearDisplacement;
    private Quaternion _angularDisplacement;
    private Rigidbody2D _rb;
    public event Action OnGrounded;

    private SubComponents<IMoveHandler> _handlers;
    private Vector2 _targetPosition;

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
        _handlers = SubComponents<IMoveHandler>.CreateCache(this);
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
        foreach (IMoveHandler movement in _handlers.GetCachedComponents())
        {
            movement.OnMove(this, deltaTime);
        }

        // raycast
        Vector2 displacement = _linearDisplacement;
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
        Quaternion newRotation = currentRotation * _angularDisplacement;

        // use rb for interpolation
        Debug.Assert(_rb.isKinematic, "rigidbody should be kinematic", _rb);
        _rb.MovePosition(newPosition);
        _rb.MoveRotation(newRotation);

        // cleanup
        _targetPosition = newPosition;
        _velocity = displacement / Time.deltaTime;
        _linearDisplacement = Vector2.zero;
        _angularDisplacement = Quaternion.identity;
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
        _linearDisplacement += deltaPosition;
    }

    public void MoveRotationDelta(Quaternion deltaRotation)
    {
        _angularDisplacement *= deltaRotation;
    }

    public bool IsGrounded()
    {
        return _isGrounded;
    }
}