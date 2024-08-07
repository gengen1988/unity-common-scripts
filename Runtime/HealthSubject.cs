using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using Object = UnityEngine.Object;

public class HealthSubject : MonoBehaviour, IHurtHandler
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
        _stamp = PoolWrapper.GetStamp(this);
        _blockers.Clear();
        _currentHP = InitialHP;
        _isDead = false;

        IComponentManager<HealthSubject>.NotifyEnabled(this);
    }

    private void OnDisable()
    {
        IComponentManager<HealthSubject>.NotifyDisabled(this);
    }

    public void OnHurt(HitSubject src, HurtSubject dest, CollisionEventData evtData)
    {
        if (_isDead)
        {
            return;
        }

        if (!src.TryGetComponent(out DamageSubject damageSubject))
        {
            Debug.LogError("damage subject not found", this);
            return;
        }

        int damageAmount = damageSubject.Damage;
        _currentHP -= damageAmount;
        // OnHealthReduced?.Invoke(this, damageAmount);
        if (_currentHP <= 0)
        {
            Die();
        }
    }

    public void KillSelf()
    {
        Die();
    }

    private void Die()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;
        OnDied?.Invoke(); // others may destroy self in this timing
        StartCoroutine(DieProcedure());
    }

    private IEnumerator DieProcedure()
    {
        // wait for blockers
        if (_blockers.Count > 0)
        {
            yield return null;
        }

        yield return new WaitForSeconds(DeathBeforeTime);
        VFXWrapper.Spawn(VFXOnDeath, transform.position, Quaternion.identity);
        AudioWrapper.PlayOneShot(SFXOnDeath);
        yield return new WaitForSeconds(DeathAfterTime);

        // 可能在之前的 coroutine 中已经命中他人而提前销毁了
        int currentStamp = PoolWrapper.GetStamp(gameObject);
        if (currentStamp == _stamp)
        {
            PoolWrapper.Despawn(gameObject);
        }
    }

    public void AcquireBlock(Object blocker)
    {
        _blockers.Add(blocker);
    }

    public void ReleaseBlock(Object blocker)
    {
        _blockers.Remove(blocker);
    }
}