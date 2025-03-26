using UnityEngine;

public class ProjectileBallisticMove : MonoBehaviour, IMovement<ProjectileContext>
{
    public void Move(ProjectileContext context, float deltaTime)
    {
        var displacement = context.Velocity * deltaTime;
        var newPosition = context.Position + displacement;
        context.Position = newPosition;
    }
}