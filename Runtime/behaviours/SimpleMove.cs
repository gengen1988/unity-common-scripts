using UnityEngine;

/**
 * a simple move ctrl for debug. support both rigidbody or transform only
 */
public class SimpleMove : MonoBehaviour
{
    public float Speed = 5f;

    private Vector2 _velocity;

    private void Update()
    {
        // input
        _velocity = UnityUtil.GetInputVector() * Speed;

        // move if no rigidbody
        if (!TryGetComponent(out Rigidbody2D _))
        {
            transform.Translate(_velocity * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        // do not move if no rigidbody
        if (!TryGetComponent(out Rigidbody2D rb))
        {
            return;
        }

        if (rb.isKinematic)
        {
            Vector2 displacement = _velocity * Time.deltaTime;
            rb.MovePosition(rb.position + displacement);
        }
        else
        {
            rb.velocity = _velocity;
        }
    }
}