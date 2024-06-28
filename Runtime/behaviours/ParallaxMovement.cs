using UnityEngine;

[DefaultExecutionOrder(50)]
public class ParallaxMovement : MonoBehaviour
{
    public BlendableMovement Reference;
    public Vector2 Scale = Vector2.one;

    private BlendableMovement _controlled;

    private void Awake()
    {
        TryGetComponent(out _controlled);
    }

    private void Update()
    {
        if (!Reference)
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.TryGetComponent(out Reference);
        }
    }

    private void FixedUpdate()
    {
        if (!Reference)
        {
            return;
        }

        Vector2 velocity = Reference.GetVelocity();
        float timeScale = TimeCtrl.GetGameplayTimeScale(this);
        float deltaTime = timeScale * Time.deltaTime;
        Vector2 displacement = velocity * deltaTime;
        Vector2 scaledDisplacement = Vector2.Scale(displacement, Scale);

        _controlled.TryGetComponent(out Rigidbody2D rb);
        Vector2 targetPosition = _controlled.GetTargetPosition();
        rb.MovePosition(targetPosition + scaledDisplacement);
    }
}