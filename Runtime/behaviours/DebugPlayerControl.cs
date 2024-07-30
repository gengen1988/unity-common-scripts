using UnityEngine;

public class DebugPlayerControl : MonoBehaviour
{
    public float Speed = 5f;

    private Shooter _shooter;
    private DebugMovement _movement;

    private void Awake()
    {
        TryGetComponent(out _shooter);
        TryGetComponent(out _movement);
    }

    private void Update()
    {
        // move
        if (_movement)
        {
            // displacement
            float hInput = Input.GetAxisRaw("Horizontal");
            float vInput = Input.GetAxisRaw("Vertical");
            Vector2 dir = new(hInput, vInput);
            Vector2 velocity = Speed * dir.normalized;
            _movement.SetVelocity(velocity);

            // rotation
            Vector2 mousePosition = UnityUtil.GetMouseWorldPosition();
            Vector2 selfPosition = _movement.GetPosition();
            Vector2 los = mousePosition - selfPosition;
            Quaternion rotation = MathUtil.QuaternionByVector(los);
            _movement.SetRotation(rotation);
        }
        else
        {
            Debug.LogWarning("require DebugMovement to move", this);
        }

        // fire
        bool fire = Input.GetButton("Fire1");
        if (fire)
        {
            if (_shooter)
            {
                _shooter.Fire();
            }
            else
            {
                Debug.LogWarning("require Shooter to fire", this);
            }
        }
    }
}