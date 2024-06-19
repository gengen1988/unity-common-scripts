using UnityEngine;

public class LinearMovement : MonoBehaviour, IMovement
{
    public float Speed = 5f;
    private BlendMovement _blend;

    private void Awake()
    {
        _blend = GetComponentInParent<BlendMovement>();
    }
    public void Tick(float deltaTime)
    {
        Quaternion rotation = _blend.GetRotation();
        Vector3 velocity = rotation * Vector3.right * Speed;
        Vector3 displacement = velocity * deltaTime;
        Vector3 position = _blend.GetPosition();
        _blend.MovePosition(position + displacement);
    }
}