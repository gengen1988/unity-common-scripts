using System;
using UnityEngine;

/// <summary>
/// sort of self-manage, to cover optional health for different actor type
/// and, module can have health itself, separated with host
/// kill submodule may cause side effects, such as debuff or damage host, so it is a strategy
/// or, overkill has many behaviors, such as delay death struggle 
/// </summary>
public class Health : MonoBehaviour, IKillable
{
    public event Action<int> OnTakeDamage;
    public event Action OnDeath;

    [SerializeField] private int defaultMaxHP = 1000;
    [SerializeField] private CueChannel cueDie;

    private int _currentHP;
    private int _maxHP;

    // private IKillable _killable; // actor (unit / projectile (optional health) ) or submodule (turret)
    private bool _isAlive;

    public int MaxHP => _maxHP;
    public int CurrentHP => _currentHP;
    public float HPRatio => (float)_currentHP / _maxHP;
    public bool IsAlive => _isAlive;

    // private void Awake()
    // {
    //     TryGetComponent(out _killable);
    // }

    private void OnEnable()
    {
        _isAlive = true;
        _maxHP = defaultMaxHP;
        _currentHP = _maxHP;

        // notify create hp bar
        GlobalEventBus<OnHealthEnabled>.Raise(new OnHealthEnabled
        {
            Subject = this,
        });
    }

    private void OnDisable()
    {
        // notify destroy hp bar
        GlobalEventBus<OnHealthDisabled>.Raise(new OnHealthDisabled
        {
            Subject = this,
        });
    }

    /// <summary>
    /// change both max hp and current hp
    /// </summary>
    /// <param name="amount"></param>
    public void Growth(int amount)
    {
        _maxHP += amount;
        _currentHP += amount;
    }

    public void Recover(int amount)
    {
        _currentHP += amount;
    }

    /**
     * public for other damage method such as buffs
     * (other side effects should put on other components, such as knockback)
     */
    public void DealDamage(int amount)
    {
        // (maybe do some defence modify?)

        // decrease hp
        _currentHP -= amount;
        OnTakeDamage?.Invoke(amount); // local notify

        // kill logic
        if (_currentHP <= 0)
        {
            Kill();

            // if (_killable != null && _killable.IsAlive)
            // {
            //     _killable.Kill();
            //     OnDeath?.Invoke();
            // }
            // else
            // {
            //     // Debug.LogWarning("health do not found killable, despawn as fallback");
            //     PoolUtil.Despawn(gameObject);
            //     OnDeath?.Invoke();
            // }
        }

        // event notify (change bar and popup number)
        // always trigger for overkill
        GlobalEventBus<OnHealthTakeDamage>.Raise(new OnHealthTakeDamage
        {
            Subject = this,
            DamageAmount = amount
        });
    }

    /// <summary>
    /// be notice this method do not despawn its gameObject
    /// despawn logic should be implemented in specific controller, to extents death behavior, like death struggle 
    /// </summary>
    public void Kill()
    {
        if (!_isAlive)
        {
            return;
        }

        _isAlive = false;
        OnDeath?.Invoke();
        cueDie.PlayIfNotNull(transform.position);
    }
}

public struct OnHealthEnabled : IGlobalEvent
{
    public Health Subject;
}

public struct OnHealthDisabled : IGlobalEvent
{
    public Health Subject;
}

public struct OnHealthTakeDamage : IGlobalEvent
{
    public Health Subject;
    public int DamageAmount;
}