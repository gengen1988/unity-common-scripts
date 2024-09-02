using UnityEngine;
using UnityEngine.Events;

public class DebugPlayer : MonoBehaviour
{
    public float Speed = 5f;
    public UnityEvent OnShoot;

    private Vector2 _velocity;
    private Rigidbody2D _rb;

    private void Awake()
    {
        TryGetComponent(out _rb);
    }

    private void Update()
    {
        // input
        Vector2 input = UnityUtil.GetInputVector();
        _velocity = input.normalized * Speed;

        if (Input.GetButton("Fire1"))
        {
            OnShoot.Invoke();
        }

        // for non-rigidbody
        if (!_rb)
        {
            Transform trans = transform;
            trans.Translate(_velocity * Time.deltaTime, Space.World);
        }
    }

    private void FixedUpdate()
    {
        if (_rb)
        {
            if (_rb.isKinematic)
            {
                Vector2 displacement = _velocity * Time.deltaTime;
                _rb.MovePosition(_rb.position + displacement);
            }
            else
            {
                _rb.velocity = _velocity;
            }
        }
    }
}