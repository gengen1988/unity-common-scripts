using UnityEngine;

public class UnitContext
{
    // movement
    public Vector2 Position;
    public Vector2 Rotation;
    public Vector2 Velocity;
    public bool IsGrounded;

    // intent (player and AI control values)
    public Vector2 MoveIntent;
    public Vector2 LookIntent;
    public bool RequestJump;
    public bool RequestAttack;

    // memory for AI
    public GameEntity FocusedEnemy;
    // public IMoveState TargetMove;
}