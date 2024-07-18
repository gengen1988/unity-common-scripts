using UnityEngine;

public class Projectile : MonoBehaviour, IHitHandler
{
    private float _elapsedTime;
    private HitSubject _hitSubject;
    private VFXManager _vfxMgr;
    private IFFTransponder _transponder;
    private BlendableMovementBase _movement;
    private ShooterProfile _profile;

    private void Awake()
    {
        _vfxMgr = SystemManager.GetSystem<VFXManager>();
        TryGetComponent(out _transponder);
        TryGetComponent(out _movement);
        Debug.Assert(_movement);
    }

    private void FixedUpdate()
    {
        Tick(Time.deltaTime);
    }

    private void Tick(float deltaTime)
    {
        if (_elapsedTime >= _profile.LifeTime)
        {
            Die();
            return;
        }

        _elapsedTime += deltaTime;
    }

    public void OnHit(HitSubject hitSubject, HurtSubject hurtSubject, CollisionEventData evtData)
    {
        // 自己人不打自己人
        // 注意：hurt Subject 可能被 multipart enemy 包含
        // 另外，如果对方没有 transponder，算中立。这种 actor 也会被命中
        IFFTransponder hurtTransponder = hurtSubject.GetComponentInParent<IFFTransponder>();
        if (hurtTransponder && !hurtTransponder.IsFoe(_transponder))
        {
            HitHurtSystem sys = SystemManager.GetSystem<HitHurtSystem>();
            sys.BypassCurrentHit();
            return;
        }

        Quaternion rotation = MathUtil.QuaternionByVector(evtData.HitVelocity);
        _vfxMgr.SpawnIfExists(_profile.VFXImpact, evtData.ContactPoint, rotation);
        AudioWrapper.PlayOneShotIfExists(_profile.SFXImpact);
        Die();
    }

    private void Die()
    {
        PoolWrapper.Despawn(gameObject);
    }

    public void LaunchedBy(Shooter source)
    {
        // 不能记录 source。比方说有时可能 source 先死掉，然后 source 发射的子弹才命中敌人
        _profile = source.Profile;
        Transform self = transform;
        Unit unit = source.GetComponentInParent<Unit>(); // 这里是对应 multipart 的写法
        Debug.Assert(unit, "the shooter should be an actor (or under a actor if it is a turret)", source);

        // reset timer
        _elapsedTime = 0;

        // init iff
        if (_transponder && unit.TryGetComponent(out IFFTransponder sourceTransponder))
        {
            _transponder.Identity = sourceTransponder.Identity;
        }

        // init motion
        unit.TryGetComponent(out BlendableMovementBase sourceMovement);
        ArcadeMove arcade = _movement.GetMovement<ArcadeMove>();
        Vector2 sourceVelocity = _profile.InheritVelocity && sourceMovement
            ? sourceMovement.GetVelocity()
            : Vector2.zero;
        Vector2 projectileVelocity = sourceVelocity + (Vector2)self.right * _profile.LaunchSpeed;
        arcade.SetVelocity(projectileVelocity);

        // effects
        _vfxMgr.SpawnIfExists(_profile.VFXMuzzle, self.position, self.rotation);
        _vfxMgr.SpawnIfExists(_profile.VFXTrail, self);
        AudioWrapper.PlayOneShotIfExists(_profile.SFXMuzzle);
    }
}