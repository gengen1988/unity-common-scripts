using UnityEngine;

public class DebugPlayerMove : MonoBehaviour
{
    public float Speed = 5f;

    private Vector2 _velocity;
    private Rigidbody2D _rb;

    private void Awake()
    {
        TryGetComponent(out _rb);
    }

    private void Update()
    {
        // input
        var input = UnityUtil.GetInputVector();
        _velocity = input.normalized * Speed;

        // non-rigidbody move
        if (!_rb)
        {
            var trans = transform;
            trans.Translate(_velocity * Time.deltaTime, Space.World);
        }
    }

    private void FixedUpdate()
    {
        if (_rb)
        {
            if (_rb.isKinematic)
            {
                var displacement = _velocity * Time.deltaTime;
                _rb.MovePosition(_rb.position + displacement);
            }
            else
            {
                _rb.velocity = _velocity;
            }
        }
    }
}