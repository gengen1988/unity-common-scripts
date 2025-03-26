using System;
using UnityEngine;

[Obsolete]
public class ShooterFeedback : MonoBehaviour
{
    [SerializeField] private Feedback muzzlePrefab;

    private Shooter2D _shooter;

    private void Awake()
    {
        TryGetComponent(out _shooter);
        _shooter.OnLaunch += HandleLaunch;
    }

    private void HandleLaunch(Projectile projectile, Vector2 position, Vector2 velocity)
    {
        var rotation = MathUtil.QuaternionByVector(velocity);
        PoolUtil.Spawn(muzzlePrefab, position, rotation);
    }
}