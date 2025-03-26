using UnityEngine;

public class ProjectileMissileMove : MonoBehaviour, IMovement<ProjectileContext>
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float accelerationTime = 1f;
    [SerializeField] private SeekControl control;

    private ITargetProvider _designator;
    private float _elapsedTime;

    private void Awake()
    {
        TryGetComponent(out _designator);
    }

    private void OnEnable()
    {
        _elapsedTime = 0;
    }

    public void Move(ProjectileContext context, float deltaTime)
    {
        var currentVelocity = context.Velocity;
        var currentPosition = context.Position;
        var force = Vector2.zero;
        if (_elapsedTime < accelerationTime)
        {
            // linear acceleration
            force = context.Rotation * Vector3.right * control.MaxOutput;
            context.Velocity += force * deltaTime;
            context.Position += context.Velocity * deltaTime;
            _elapsedTime += deltaTime;
            return;
        }

        // observe
        _designator.Refresh();
        if (!_designator.LockedTarget)
        {
            // linear acceleration
            force = context.Rotation * Vector3.right * control.MaxOutput;
            context.Velocity += force * deltaTime;
            context.Position += context.Velocity * deltaTime;
            return;
        }

        var targetPosition = _designator.LastKnownTargetPosition;
        var los = targetPosition - currentPosition;
        force = control.CalcControlForce(los, currentVelocity, speed);

        // decide
        var nextVelocity = currentVelocity + force * deltaTime;
        var nextPosition = currentPosition + nextVelocity * deltaTime;
        var nextRotation = MathUtil.QuaternionByVector(nextVelocity);
        context.Position = nextPosition;
        context.Velocity = nextVelocity;
        context.Rotation = nextRotation;

        // act
        // rb.MovePosition ...
    }
}