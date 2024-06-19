using UnityEngine;

public class DebugMovement : MonoBehaviour
{
    private Vector2 _velocity;
    private Vector2 _force;
    private Rigidbody2D _rb;

    private void Awake()
    {
        TryGetComponent(out _rb);
    }

    private void Update()
    {
        if (_rb)
        {
            return;
        }

        transform.Translate(_velocity * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        if (!_rb)
        {
            return;
        }

        if (!_rb.isKinematic)
        {
            return;
        }

        // calc
        Vector2 forceToBeApply = _force * Time.deltaTime;
        Vector2 displacement = _velocity * Time.deltaTime;

        // apply
        _rb.velocity = _velocity;
        _rb.MovePosition(_rb.position + displacement);
        _velocity += forceToBeApply;

        // cleanup
        _force = Vector2.zero;
    }

    public void SetVelocity(Vector2 velocity)
    {
        _velocity = velocity;
        if (_rb)
        {
            _rb.velocity = velocity;
        }
    }

    public void AddForce(Vector2 force)
    {
        _force += force;
        if (_rb)
        {
            _rb.AddForce(force);
        }
    }
}