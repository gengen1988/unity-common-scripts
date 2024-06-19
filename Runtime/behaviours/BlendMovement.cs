using System;
using System.Linq;
using UnityEngine;

public class BlendMovement : MonoBehaviour
{
    public bool ConstraintByRaycast;
    public PlatformerRaycaster _raycaster;

    private bool _isGrounded;
    private Vector2 _velocity;
    private Vector2 _displacement;
    private Quaternion _rotation;
    private Rigidbody2D _rb;

    private IMovement[] _components;
    public event Action OnGrounded;

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
        _components = this.GetComponentsInChildrenDirectly<IMovement>(true).ToArray();
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
        foreach (IMovement movement in _components)
        {
            if (movement is MonoBehaviour behaviour && behaviour.isActiveAndEnabled)
            {
                movement.Tick(deltaTime);
            }
        }

        // raycast
        Vector2 displacement = _displacement;
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
        Quaternion newRotation = currentRotation * _rotation;

        if (_rb)
        {
            // use rb for interpolation
            Debug.Assert(_rb.isKinematic, "rigidbody should be kinematic", _rb);
            _rb.MovePosition(newPosition);
            _rb.MoveRotation(newRotation);
        }
        else
        {
            // use transform directly for slot (others will interpolate itself)
            Transform slot = transform;
            slot.position = newPosition;
            slot.rotation = newRotation;
        }

        // cleanup
        _velocity = displacement / Time.deltaTime;
        _displacement = Vector2.zero;
        _rotation = Quaternion.identity;
    }

    public Vector2 GetPosition()
    {
        if (_rb)
        {
            return _rb.position;
        }

        return transform.position;
    }

    public Quaternion GetRotation()
    {
        if (_rb)
        {
            return MathUtil.QuaternionByAngle(_rb.rotation);
        }

        return transform.rotation;
    }

    public Vector2 GetVelocity()
    {
        return _velocity;
    }

    public void MovePosition(Vector2 position)
    {
        Vector2 displacement = position - GetPosition();
        _displacement += displacement;
    }

    public void MoveRotation(Quaternion rotation)
    {
        Quaternion delta = MathUtil.DeltaQuaternion(GetRotation(), rotation);
        _rotation *= delta;
    }

    public bool IsGrounded()
    {
        return _isGrounded;
    }
}