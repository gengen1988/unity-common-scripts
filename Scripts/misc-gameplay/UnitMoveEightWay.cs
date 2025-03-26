using UnityEngine;

public class UnitMoveEightWay : MonoBehaviour, IMovement<UnitContext>
{
    [SerializeField] private float speed = 5f;

    public void Move(UnitContext context, float deltaTime)
    {
        var moveIntent = context.MoveIntent;
        var velocity = moveIntent.normalized * speed;
        var displacement = moveIntent.normalized * (speed * deltaTime);
        var nextPosition = context.Position + displacement;
        context.Position = nextPosition;
        context.Velocity = velocity;
    }
}