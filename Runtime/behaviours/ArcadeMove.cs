using UnityEngine;

public class ArcadeMove : MonoBehaviour, IMoveHandler
{
    public float Acceleration;
    public float MinSpeed;
    public float MaxSpeed = 100f;

    private Vector2 _velocity;

    private void OnEnable()
    {
        _velocity = Vector2.zero;
    }

    public void OnMove(IMoveSubject subject, float deltaTime)
    {
        Vector2 currentDirection = _velocity.normalized;
        float currentSpeed = _velocity.magnitude;
        float deltaSpeed = Acceleration * deltaTime;
        float newSpeed = currentSpeed + deltaSpeed;
        float clamped = Mathf.Clamp(newSpeed, MinSpeed, MaxSpeed);
        Vector2 actualAcceleration = (clamped - currentSpeed) * currentDirection;
        Vector2 displacement = KinematicUtil.Displacement(_velocity, actualAcceleration, deltaTime);
        _velocity = newSpeed * currentDirection;
        subject.MovePositionDelta(displacement);
        // subject.MoveVelocityDelta(actualAcceleration * deltaTime);
    }

    public void SetVelocity(Vector2 velocity)
    {
        // Debug.Log($"{this} assigned new velocity: {velocity}", this);
        _velocity = velocity;
    }
}