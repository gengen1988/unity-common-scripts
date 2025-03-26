using System;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public class TurretManager : MonoBehaviour
{
    private readonly List<Turret2D> _turrets = new();
    private readonly Dictionary<Turret2D, Shooter2D> _shooterByTurret = new();

    public void AddTurret(Turret2D turret)
    {
        _turrets.Add(turret);
        // Cache the shooter component if it exists on the turret
        if (turret.TryGetComponent(out Shooter2D shooter))
        {
            _shooterByTurret[turret] = shooter;
        }
    }

    public void RemoveTurret(Turret2D turret)
    {
        _turrets.Remove(turret);
        if (_shooterByTurret.ContainsKey(turret))
        {
            _shooterByTurret.Remove(turret);
        }
    }

    public IReadOnlyList<Turret2D> GetTurrets()
    {
        return _turrets;
    }

    public Shooter2D GetShooter(Turret2D turret)
    {
        return _shooterByTurret.GetValueOrDefault(turret);
    }
}