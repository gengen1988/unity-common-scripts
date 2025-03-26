using UnityEngine;

public class UnitMovePulse : MonoBehaviour, IMovement<UnitContext>
{
    [SerializeField] private float interval = 2f;
    [SerializeField] private float drag = .1f;
    [SerializeField] private float forceAmount = 10;
    [SerializeField] private float angleRange = 30;
    [SerializeField] private float timeRange = .5f;

    private float _cooldown;

    public void Move(UnitContext context, float deltaTime)
    {
        _cooldown -= deltaTime;
        if (_cooldown > 0)
        {
            var currentVelocity = context.Velocity;
            var speed = currentVelocity.magnitude;
            var nextVelocity = MathUtil.ChangeVectorMagnitude(currentVelocity, -drag * deltaTime);
            var nextPosition = context.Position + nextVelocity * deltaTime;
            context.Position = nextPosition;
            context.Velocity = nextVelocity;
        }
        else
        {
            var dir = context.MoveIntent.normalized;
            var randomAngle = Random.Range(-angleRange, angleRange);
            var nextVelocity = (Vector2)(MathUtil.QuaternionByAngle(randomAngle) * dir * forceAmount);
            var nextPosition = context.Position + nextVelocity * deltaTime;
            context.Position = nextPosition;
            context.Velocity = nextVelocity;
            _cooldown = interval + Random.Range(-timeRange / 2f, timeRange / 2f);
        }
    }
}