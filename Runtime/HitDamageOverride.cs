using UnityEngine;

public class HitDamageOverride : MonoBehaviour
{
    [SerializeField] private int DamageOnHit = 100;

    public int Damage => DamageOnHit;
}