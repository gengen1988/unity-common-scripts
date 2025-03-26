using UnityEngine;

public class Rocket : MonoBehaviour, IMovement<ProjectileContext>
{
    [SerializeField] private float forceAmount = 10f;
    [SerializeField] private float accelerationTime = 2f;

    private float _elapsedTime;
    private Health _health;

    private void Awake()
    {
        TryGetComponent(out _health);
        _health.OnDeath += HandleDeath;
    }

    private void OnEnable()
    {
        _elapsedTime = 0;
    }

    private void HandleDeath()
    {
        PoolUtil.Despawn(gameObject);
    }

    public void Move(ProjectileContext context, float deltaTime)
    {
        if (_elapsedTime < accelerationTime)
        {
            // linear acceleration
            var force = (Vector2)(context.Rotation * Vector3.right * forceAmount);
            context.Velocity += force * deltaTime;
            _elapsedTime += deltaTime;
        }

        context.Position += context.Velocity * deltaTime;
    }
}