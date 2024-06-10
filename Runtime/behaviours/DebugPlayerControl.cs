using UnityEngine;

public class DebugPlayerControl : MonoBehaviour
{
    public float Speed = 5f;

    private void Update()
    {
        if (!TryGetComponent(out DebugMovement movement))
        {
            Debug.LogWarning("DebugMovement not found");
            return;
        }

        float hInput = Input.GetAxisRaw("Horizontal");
        float vInput = Input.GetAxisRaw("Vertical");
        Vector2 intent = new(hInput, vInput);
        Vector2 velocity = Speed * intent.normalized;
        movement.SetVelocity(velocity);
    }
}