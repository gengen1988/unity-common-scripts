using UnityEngine;

public class LinearMove : MonoBehaviour, IMoveHandler
{
    public float Speed = 5f;

    public void OnMove(BlendableMovement movement, float deltaTime)
    {
        Quaternion rotation = movement.GetRotation();
        Vector3 velocity = rotation * Vector3.right * Speed;
        Vector2 displacement = velocity * deltaTime;
        movement.MovePositionDelta(displacement);
    }
}