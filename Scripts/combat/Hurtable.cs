using System;
using UnityEngine;

/**
 * root of multiple part hurt destination
 */
public class Hurtable : MonoBehaviour
{
    public event Action<HitInfo> OnHurtBegin;
    public event Action<HitInfo> OnHurtEnd;

    [SerializeField] private HurtType surfaceType;

    private EntityProxy _proxy;
    private bool _isHurting;

    public GameEntity CurrentEntity => _proxy.Entity;
    public bool IsHurting => _isHurting;

    private void Awake()
    {
        _proxy = GetComponentInParent<EntityProxy>();
    }

    private void OnDestroy()
    {
        OnHurtBegin = null;
        OnHurtEnd = null;
    }

    public void BeginHurt(HitInfo evt)
    {
        _isHurting = true;
        OnHurtBegin?.Invoke(evt);
    }

    public void EndHurt(HitInfo evtData)
    {
        _isHurting = false;
        OnHurtEnd?.Invoke(evtData);
    }
}

public enum HurtType
{
    Flesh,
    Metal,
    Wood,
    Stone,
    Glass,
    Cloth,
    Liquid,
    ForceField,
}