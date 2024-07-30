using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class Shooter : MonoBehaviour
{
    public ShooterProfile Profile;
    public Transform LaunchTransform;
    public float LaunchWidth;
    public int GunAmount = 1;

    private int _currentGunIndex = -1;
    private float _cooling;
    private int _burstRemaining;

    private IFFTransponder _transponder;
    private BlendableMovement _movement;

    public bool IsFiring => _burstRemaining > 0;

    private void OnValidate()
    {
        Debug.Assert(LaunchTransform, $"{this} must have LaunchTransform", this);
        Debug.Assert(Profile, $"{this} must have Profile", this);
    }

    private void Reset()
    {
        if (!LaunchTransform)
        {
            LaunchTransform = transform;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (LaunchTransform == null)
        {
            return;
        }

        // Visualize gun width and offsets
        Gizmos.color = Color.red;
        Vector3 position = LaunchTransform.position;
        Quaternion rotation = LaunchTransform.rotation;
        float halfWidth = LaunchWidth / 2;
        IEnumerable<Vector3> points = MathUtil.Progress01(GunAmount, true)
            .Select(p => Mathf.Lerp(-halfWidth, halfWidth, p))
            .Select(y => new Vector2(0, y))
            .Select(o => position + rotation * o);
        foreach (Vector3 p in points)
        {
            Gizmos.DrawSphere(p, 0.1f);
        }
    }

    private void Awake()
    {
        _movement = GetComponentInParent<BlendableMovement>();
    }

    private void OnEnable()
    {
        // pooling
        _currentGunIndex = -1;
        _burstRemaining = 0;
        _cooling = 0;
    }

    private void FixedUpdate()
    {
        Tick(Time.deltaTime);
    }

    private void Tick(float deltaTime)
    {
        if (_burstRemaining > 0)
        {
            if (_cooling <= 0)
            {
                ExecuteShoot();
                _burstRemaining--;
                if (_burstRemaining > 0)
                {
                    _cooling += Profile.IntervalInBurst;
                }
                else
                {
                    _cooling += Profile.CooldownBetweenBurst;
                }
            }
        }

        if (_cooling > 0)
        {
            _cooling -= deltaTime;
        }
    }

    public void Fire()
    {
        if (_burstRemaining > 0 || _cooling > 0)
        {
            return;
        }

        _burstRemaining = Profile.Burst;
    }

    private void ExecuteShoot()
    {
        // calc launch offset and spread
        Quaternion referenceRotation = LaunchTransform.rotation;
        Vector3 referencePosition = LaunchTransform.position;
        float p = (float)_currentGunIndex / GunAmount;
        float padding = LaunchWidth / GunAmount / 2;
        float halfWidth = LaunchWidth / 2;
        float y = Mathf.Lerp(-halfWidth, halfWidth, p) + padding;
        Vector2 offset = new Vector2(0, y);
        Vector3 rotatedOffset = referenceRotation * offset;
        Vector3 launchPosition = referencePosition + rotatedOffset;
        Quaternion launchRotation = referenceRotation * RandomUtil.Rotate(Profile.Spread);
        _currentGunIndex = MathUtil.Mod(_currentGunIndex + 1, GunAmount);

        // spawn
        GameObject go = PoolWrapper.Spawn(Profile.Projectile, launchPosition, launchRotation);

        // init projectile
        go.TryGetComponent(out Projectile projectile);
        Debug.Assert(projectile, "shot should has a Projectile", Profile.Projectile);
        Vector2 launchVelocity = launchRotation * Vector3.right * Profile.LaunchSpeed;
        if (Profile.InheritVelocity && _movement)
        {
            launchVelocity += _movement.GetVelocity();
        }

        projectile.Init(this, launchVelocity);
    }

    public Quaternion GetLaunchRotation()
    {
        return LaunchTransform.rotation;
    }

    public Vector2 GetLaunchPosition()
    {
        return LaunchTransform.position;
    }

    public bool Predict(Vector2 targetPosition, Vector2 targetVelocity, out Vector2 predictLos)
    {
        Vector2 launchPosition = GetLaunchPosition();
        Vector2 los = targetPosition - launchPosition;

        // self movement correction
        Vector2 correctedVelocity = targetVelocity;
        if (Profile.InheritVelocity && _movement)
        {
            correctedVelocity -= _movement.GetVelocity();
        }

        // predict
        if (!KinematicUtil.InterceptTime(los, correctedVelocity, Profile.LaunchSpeed, out float time))
        {
            predictLos = los;
            return false;
        }

        predictLos = los + targetVelocity * time;
        return true;
    }

    public bool IsAimed(Vector2 los, float tolerance)
    {
        Quaternion currentRotation = GetLaunchRotation();
        Vector2 fov = currentRotation * Vector3.up;
        float projection = Vector2.Dot(los, fov);
        return Mathf.Abs(projection) <= tolerance;
    }
}