using UnityEngine;

public class LinearMove : MonoBehaviour, IMoveHandler
{
    public float Speed = 5f;

    public void OnMove(IMoveSubject subject, float deltaTime)
    {
        Quaternion rotation = subject.GetRotation();
        Vector2 displacement = rotation * Vector3.right * (Speed * deltaTime);
        subject.MovePositionDelta(displacement);
    }
}