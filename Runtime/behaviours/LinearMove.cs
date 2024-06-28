using UnityEngine;

public class LinearMove : MonoBehaviour, IMoveHandler
{
    public float Speed = 5f;

    public void OnMove(IMoveSubject subject, float deltaTime)
    {
        Quaternion rotation = subject.GetRotation();
        Vector2 velocity = rotation * Vector3.right * Speed;
        Vector2 displacement = velocity * deltaTime;
        subject.MovePositionDelta(displacement);
    }
}