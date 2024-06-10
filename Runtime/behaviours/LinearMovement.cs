using UnityEngine;

public class LinearMovement : MonoBehaviour
{
    public float Speed = 5f;

    private Rigidbody2D _rb;
    private BlendMovement _blend;

    private void Awake()
    {
        TryGetComponent(out _rb);
        TryGetComponent(out _blend);
    }

    private void FixedUpdate()
    {
        Vector2 velocity = transform.right * Speed;
        Vector2 displacement = velocity * Time.deltaTime;
        if (_blend)
        {
            _blend.AddDisplacement(displacement);
        }
        else
        {
            _rb.MovePosition(_rb.position + displacement);
        }
    }
}