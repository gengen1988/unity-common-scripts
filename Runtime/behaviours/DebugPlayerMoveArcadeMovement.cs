using UnityEngine;

/**
 * this bypass standard brain -> input -> pawn -> eight way move pipeline 
 */
public class DebugPlayerMoveArcadeMovement : MonoBehaviour
{
    public float Speed = 5f;

    private Vector2 _velocity;
    private Rigidbody2D _rb;
    private ArcadeMovement _movement;

    private void Awake()
    {
        _movement = this.EnsureComponent<ArcadeMovement>();
    }

    private void Update()
    {
        var input = UnityUtil.GetInputVector();
        var velocity = input.normalized * Speed;
        _movement.SetVelocity(velocity);
    }
}