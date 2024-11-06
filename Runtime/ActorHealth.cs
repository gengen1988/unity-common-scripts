using System;
using UnityEngine;

public class ActorHealth : MonoBehaviour
{
    public event Action<ActorOld, int> OnDamage;

    public int DefaultHP = 100;

    private int _currentHP;
    private int _maxHP;
    private ActorOld _actorOld;

    public int CurrentHP => _currentHP;

    private void Awake()
    {
        TryGetComponent(out _actorOld);
    }

    private void OnEnable()
    {
        // reset states at respawn
        // AttributeManager attrMgr = _actor.Attribute;
        // if (attrMgr)
        // {
        //     _maxHP = attrMgr.GetValue<int>("MAX_HP");
        // }
        // else
        {
            _maxHP = DefaultHP;
        }

        _currentHP = _maxHP;

        // notify event
        IComponentManager<ActorHealth>.NotifyEnabled(this);
    }

    private void OnDisable()
    {
        IComponentManager<ActorHealth>.NotifyDisabled(this);
    }

    public void DealDamage(int damage)
    {
        _currentHP -= damage;
        OnDamage?.Invoke(_actorOld, damage);
        if (_currentHP <= 0)
        {
            _actorOld.Kill();
        }
    }
}