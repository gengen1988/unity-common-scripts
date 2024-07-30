using UnityEngine;
using Debug = UnityEngine.Debug;

public class Projectile : MonoBehaviour, IHitHandler, IMoveHandler
{
    private float _elapsedTime;
    private Vector2 _launchVelocity;

    private HitSubject _hitSubject;
    private IFFTransponder _transponder;
    private BlendableMovement _movement;

    private ShooterProfile _profile;
    private VFXCtrl _trail;

    private void Awake()
    {
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
            hitSubject.BypassCurrentHit();
            return;
        }

        Quaternion rotation = MathUtil.QuaternionByVector(evtData.HitVelocity);
        VFXWrapper.Spawn(_profile.VFXImpact, evtData.ContactPoint, rotation);
        AudioWrapper.PlayOneShot(_profile.SFXImpact);
        Die();
    }

    public void OnMove(IMoveSubject subject, float deltaTime)
    {
        if (Mathf.Approximately(_profile.Acceleration, 0))
        {
            subject.MovePositionDelta(_launchVelocity * deltaTime);
        }
        else
        {
            Vector2 direction = _launchVelocity.normalized;
            float currentSpeed = _launchVelocity.magnitude;
            float deltaSpeed = _profile.Acceleration * deltaTime;
            float nextSpeed = currentSpeed + deltaSpeed;
            float clamped = Mathf.Clamp(nextSpeed, _profile.MinSpeed, _profile.MaxSpeed);
            Vector2 correctedDeltaSpeed = (clamped - currentSpeed) * direction;
            Vector2 correctedAcceleration = correctedDeltaSpeed / deltaTime;
            Vector2 displacement = KinematicUtil.Displacement(_launchVelocity, correctedAcceleration, deltaTime);
            subject.MovePositionDelta(displacement);
            _launchVelocity += correctedDeltaSpeed;
        }
    }

    private void Die()
    {
        if (_trail)
        {
            _trail.Finish();
        }

        PoolWrapper.Despawn(gameObject);
    }

    public void Init(Shooter source, Vector2 velocity)
    {
        // init self
        _elapsedTime = 0;
        _profile = source.Profile; // 不能记录 source。比方说有时可能 source 先死掉，然后 source 发射的子弹才命中敌人
        _launchVelocity = velocity;

        // init iff
        Unit unit = source.GetComponentInParent<Unit>(); // 这里是对应 multipart 的写法
        Debug.Assert(unit, "the shooter should be an actor (or under a actor if it is a turret)", source);
        if (_transponder && unit.TryGetComponent(out IFFTransponder sourceTransponder))
        {
            _transponder.Identity = sourceTransponder.Identity;
        }

        // effects
        Transform self = transform;
        AudioWrapper.PlayOneShot(_profile.SFXMuzzle);
        VFXWrapper.Spawn(_profile.VFXMuzzle, self.position, self.rotation);
        _trail = VFXWrapper.Spawn(_profile.VFXTrail, self);
    }
}