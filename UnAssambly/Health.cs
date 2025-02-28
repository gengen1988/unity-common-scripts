using System;
using UnityEngine;

public class Health : MonoBehaviour, IEntityAttach
{
    public event Action<int> OnTakeDamage;

    [SerializeField] private int DefaultHP = 200;
    [SerializeField] private int DefaultHurtDamage = 100;

    private int _currentHP;
    private int _maxHP;
    private HitController _hitController; // optional

    public int MaxHP => _maxHP;
    public int CurrentHP => _currentHP;
    public float HealthRatio => (float)_currentHP / _maxHP;

    private void Awake()
    {
        if (TryGetComponent(out _hitController))
        {
            _hitController.OnHurt += HandleHurt;
        }
    }

    private void OnDestroy()
    {
        OnTakeDamage = null;
    }

    public void OnEntityAttach(GameEntity entity)
    {
        _maxHP = DefaultHP;
        _currentHP = _maxHP;
    }

    private void HandleHurt(HitEventData evt)
    {
        var hitBridge = evt.HitEntity.GetBridge();
        var overrideDamage = hitBridge.GetComponent<HitDamageOverride>();
        var damage = overrideDamage.GetValueOrDefault(DefaultHurtDamage);

        // // stun
        // var buff = _actor.AddBuff("buff_stun");
        // buff.LifeTime = StunTime;

        // knock back
        // var dir = evt.HitVelocity.normalized;
        // gameObject.AddForce(dir * 5f);

        // hp
        DealDamage(damage);
    }

    /**
     * public for other damage method such as buffs
     */
    public void DealDamage(int damage)
    {
        // (maybe do some defence modify?)

        // decrease hp
        _currentHP -= damage;
        OnTakeDamage?.Invoke(damage);
    }
}

public static class HitDamageOverrideExtensions
{
    public static int GetValueOrDefault(this HitDamageOverride overrideHit, int defaultValue)
    {
        if (overrideHit == null)
        {
            return defaultValue;
        }

        return overrideHit.Damage;
    }
}