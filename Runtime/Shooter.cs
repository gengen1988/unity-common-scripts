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
    public float DelayAtStart;

    private int _currentGunIndex = -1;
    private float _cooling;
    private int _burstRemaining;
    private IFFTransponder _transponder;

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

    private void OnEnable()
    {
        _cooling = DelayAtStart;
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
                DoShoot();
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

    public void Shoot()
    {
        if (_burstRemaining > 0 || _cooling > 0)
        {
            return;
        }

        _burstRemaining = Profile.Burst;
    }

    private void DoShoot()
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
        projectile.LaunchedBy(this);
    }

    public Quaternion GetLaunchRotation()
    {
        return LaunchTransform.rotation;
    }

    public Vector2 GetLaunchPosition()
    {
        return LaunchTransform.position;
    }
}