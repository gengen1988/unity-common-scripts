using UnityEngine;

public class DebugMovement : MonoBehaviour
{
    private Vector2 _velocity;
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

        Vector2 displacement = _velocity * Time.deltaTime;
        _rb.MovePosition(_rb.position + displacement);
    }

    public void SetVelocity(Vector2 velocity)
    {
        _velocity = velocity;
        if (_rb && !_rb.isKinematic)
        {
            _rb.velocity = velocity;
        }
    }
}