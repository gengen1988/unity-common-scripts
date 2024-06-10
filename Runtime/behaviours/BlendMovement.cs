using UnityEngine;

[DefaultExecutionOrder(100)]
public class BlendMovement : MonoBehaviour
{
    private Rigidbody2D _rb;
    private Vector2 _displacement;

    private void Awake()
    {
        TryGetComponent(out _rb);
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _displacement);
        _displacement = Vector2.zero;
    }

    public void AddDisplacement(Vector2 displacement)
    {
        _displacement += displacement;
    }
}