using FMODUnity;
using UnityEngine;

[CreateAssetMenu]
public class ShooterProfile : ScriptableObject
{
    [Header("Shot Gameplay")]
    public GameObject Projectile;

    [Header("Effects")]
    public GameObject VFXMuzzle; // on spawn, fixed
    public EventReference SFXMuzzle;
    public GameObject VFXImpact; // on hit, fixed
    public EventReference SFXImpact;
    public GameObject VFXTrail; // on spawn, follow

    [Header("Launch Attributes")]
    public bool InheritVelocity;
    public float LaunchSpeed = 40f;
    public float LifeTime = 0.5f;
    public int Burst = 1;
    public float IntervalInBurst = 0.1f;
    public float CooldownBetweenBurst = 0.2f;
    public float Spread = 10;

    [Header("Movement")]
    public float Acceleration;
    public float MinSpeed;
    public float MaxSpeed = 100f;
}