using System;
using UnityEngine;

/**
 * root of multiple part hit source
 */
public class Hitable : MonoBehaviour
{
    public event Action<HitInfo> OnHitBegin;
    public event Action<HitInfo> OnHitEnd;

    [SerializeField] private CueChannel cueHit;

    private EntityProxy _proxy;
    private bool _isHitting;

    public GameEntity CurrentEntity => _proxy.Entity;
    public bool IsHitting => _isHitting;

    private void Awake()
    {
        _proxy = GetComponentInParent<EntityProxy>();
    }

    private void OnDestroy()
    {
        OnHitBegin = null;
        OnHitEnd = null;
    }

    public void BeginHit(HitInfo evt)
    {
        _isHitting = true;
        OnHitBegin?.Invoke(evt);
        cueHit.PlayIfNotNull(evt.ContactPoint, MathUtil.QuaternionByVector(evt.HitVelocity));
    }

    public void EndHit(HitInfo evt)
    {
        _isHitting = false;
        OnHitEnd?.Invoke(evt);
    }
}