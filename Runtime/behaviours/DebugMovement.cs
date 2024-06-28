using UnityEngine;

public class DebugMovement : MonoBehaviour
{
    private Quaternion _rotation;
    private Vector2 _velocity;
    private Rigidbody2D _rb;

    private void Awake()
    {
        TryGetComponent(out _rb);
    }

    private void Update()
    {
        if (!_rb)
        {
            Transform trans = transform;
            trans.Translate(_velocity * Time.deltaTime);
            trans.rotation = _rotation;
        }
    }

    private void FixedUpdate()
    {
        if (_rb && _rb.isKinematic)
        {
            // calc
            Vector2 displacement = _velocity * Time.deltaTime;

            // apply
            _rb.MoveRotation(_rotation);
            _rb.MovePosition(_rb.position + displacement);
            _rb.velocity = _velocity;
        }
    }

    public void SetVelocity(Vector2 velocity)
    {
        _velocity = velocity;
        if (_rb)
        {
            _rb.velocity = velocity;
        }
    }

    public void SetRotation(Quaternion rotation)
    {
        _rotation = rotation;
    }

    public Vector2 GetPosition()
    {
        if (_rb)
        {
            return _rb.position;
        }
        else
        {
            return transform.position;
        }
    }
}