using UnityEngine;

public interface ITurretManager
{
    public void AddTurret(ITurret turret);
    public void RemoveTurret(ITurret turret);
}

public interface ITurret
{
    public void RotateTowards(Vector2 alignVector, float deltaTime);
    public bool IsAligned(Vector2 alignVector, float deltaTime);
    public Shooter2D GetShooter();
}