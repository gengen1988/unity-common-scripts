using System.Collections.Generic;
using UnityEngine;

public class TurretFCS : MonoBehaviour, ITurretManager
{
    [SerializeField] private bool manualControl;

    private GameEntity _lockedTarget;
    private IMoveState _targetMove;
    private Vector2 _fallbackDirection;
    private bool _requestFire;
    private readonly List<ITurret> _turrets = new();

    public void AddTurret(ITurret turret)
    {
        _turrets.Add(turret);
        var shooter = turret.GetShooter();
        shooter.OnLaunch += HandleLaunch;
    }

    public void RemoveTurret(ITurret turret)
    {
        _turrets.Remove(turret);
        var shooter = turret.GetShooter();
        shooter.OnLaunch -= HandleLaunch;
    }

    private void HandleLaunch(Projectile projectile, Vector2 position, Vector2 velocity)
    {
        // align faction
        IFFTransponder.CopyIdentity(this, projectile);
    }

    public void Tick(float deltaTime)
    {
        foreach (var turret in _turrets)
        {
            // turret.Cooldown(deltaTime); // for recoil

            var shooter = turret.GetShooter();
            shooter.Cooldown(deltaTime);
        }

        if (manualControl)
        {
            foreach (var turret in _turrets)
            {
                turret.RotateTowards(_fallbackDirection, deltaTime);
                if (_requestFire)
                {
                    turret.GetShooter().Fire();
                    _requestFire = false;
                }
            }
        }
        else
        {
            if (_lockedTarget)
            {
                var targetPosition = _targetMove.Position;
                var targetVelocity = _targetMove.Velocity;
                DebugUtil.DrawCross(targetPosition, Color.red);
                Debug.DrawRay(targetPosition, targetVelocity, Color.red);

                foreach (var turret in _turrets)
                {
                    var shooter = turret.GetShooter();
                    if (shooter.Predict(targetPosition, targetVelocity, out var alignVector))
                    {
                        Debug.DrawLine(shooter.LaunchPosition, shooter.LaunchPosition + alignVector, Color.green);

                        turret.RotateTowards(alignVector, deltaTime);
                        if (turret.IsAligned(alignVector, deltaTime))
                        {
                            shooter.Fire();
                        }
                    }
                }
            }
        }
    }

    public void SetFallbackDirection(Vector2 direction)
    {
        _fallbackDirection = direction;
    }

    public void RequestFire()
    {
        _requestFire = true;
    }

    public void SetTarget(GameEntity target)
    {
        _lockedTarget = target;
        _targetMove = target.GetMove();
    }
}