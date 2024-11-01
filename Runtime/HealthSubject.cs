using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using Object = UnityEngine.Object;

[Obsolete]
public class HealthSubject : MonoBehaviour
{
    public int InitialHP = 100;

    [Header("Health Bar")]
    public Vector2 HealthBarOffset = Vector2.up;
    public Vector2 HealthBarScale = Vector2.one;

    [Header("Death")]
    public float DeathBeforeTime;
    public float DeathAfterTime = 0.1f;
    public GameObject VFXOnDeath;
    public EventReference SFXOnDeath;

    private int _currentHP;
    private bool _isDead;
    private int _stamp;

    private readonly HashSet<Object> _blockers = new();

    public event Action OnDied;

    public int CurrentHP => _currentHP;
    public bool IsDead => _isDead;

    private void OnDestroy()
    {
        OnDied = null;
    }

    private void OnEnable()
    {
        _currentHP = InitialHP;

        // _stamp = PoolWrapper.GetStamp(this);
        // _blockers.Clear();
        // _isDead = false;

        IComponentManager<HealthSubject>.NotifyEnabled(this);
    }

    private void OnDisable()
    {
        IComponentManager<HealthSubject>.NotifyDisabled(this);
    }

    // public void KillSelf()
    // {
    //     Die();
    // }
    //
    // private void Die()
    // {
    //     if (_isDead)
    //     {
    //         return;
    //     }
    //
    //     _isDead = true;
    //     OnDied?.Invoke(); // others may destroy self in this timing
    //     StartCoroutine(DieProcedure());
    // }
    //
    // private IEnumerator DieProcedure()
    // {
    //     // wait for blockers
    //     if (_blockers.Count > 0)
    //     {
    //         yield return null;
    //     }
    //
    //     yield return new WaitForSeconds(DeathBeforeTime);
    //     VFXWrapper.Spawn(VFXOnDeath, transform.position, Quaternion.identity);
    //     AudioWrapper.PlayOneShot(SFXOnDeath);
    //     yield return new WaitForSeconds(DeathAfterTime);
    //
    //     // 可能在之前的 coroutine 中已经命中他人而提前销毁了
    //     int currentStamp = PoolWrapper.GetStamp(gameObject);
    //     if (currentStamp == _stamp)
    //     {
    //         PoolWrapper.Despawn(gameObject);
    //     }
    // }

    public void DealDamage(int damage)
    {
        _currentHP -= damage;
        if (_currentHP <= 0)
        {
            // Die();
        }
    }
}