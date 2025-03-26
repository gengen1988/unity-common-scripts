using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public delegate void ShooterLaunchEvent(Projectile projectile, Vector2 position, Vector2 velocity);

public class Shooter2D : MonoBehaviour
{
    private const string MSG_FIRE = "fire";

    private enum BarrelPattern
    {
        Loop,
        Random
    }

    public event ShooterLaunchEvent OnLaunch;

    [Header("Fire Parameters")]
    [SerializeField] [Range(1, 3000)] [Tooltip("shots per minute")]
    private int fireRate = 1000;
    [SerializeField] private int burstAmount = 1;
    [SerializeField] private int roundsPerShot = 1;

    [Header("Barrel Parameters")]
    [SerializeField] private Transform launchTrans;
    [SerializeField] private float launchWidth;
    [SerializeField] private int barrelAmount = 1;
    [SerializeField] private BarrelPattern barrelType;
    [SerializeField] [Range(0, 360)] private float spread = 10;

    [Header("Projectile Parameters")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private float launchSpeed = 40f;
    [SerializeField] private float lifeTime = 0.5f;

    [Header("Gameplay Cues")]
    [SerializeField] private CueChannel cueOnLaunch;

    private float _elapsedTime;
    private int _burstCount;
    private int _currentBarrelIndex;

    // public float LaunchSpeed => launchSpeed;

    private StateMachine<Shooter2D> _fsm;
    public Vector2 LaunchPosition => launchTrans.position;

    private void OnDrawGizmos()
    {
        // Draw barrel launch positions
        if (launchTrans != null)
        {
            Gizmos.color = Color.yellow;

            // Draw the main launch position
            Gizmos.DrawWireSphere(launchTrans.position, 0.1f);

            // Draw the spread area
            if (barrelAmount > 1 || roundsPerShot > 1 || spread > 0)
            {
                // Draw spread cone
                Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // Orange with transparency
                var forward = launchTrans.forward;
                var right = launchTrans.right;

                // Draw spread lines
                var halfSpread = spread * 0.5f * Mathf.Deg2Rad;
                var spreadDistance = 2f; // Visual distance for spread lines

                var leftSpreadDir = Quaternion.AngleAxis(-spread * 0.5f, Vector3.forward) * forward;
                var rightSpreadDir = Quaternion.AngleAxis(spread * 0.5f, Vector3.forward) * forward;

                Gizmos.DrawLine(launchTrans.position, launchTrans.position + leftSpreadDir * spreadDistance);
                Gizmos.DrawLine(launchTrans.position, launchTrans.position + rightSpreadDir * spreadDistance);

                // Draw barrel positions if multiple barrels
                if (barrelAmount > 1)
                {
                    Gizmos.color = Color.cyan;
                    for (var i = 0; i < barrelAmount; i++)
                    {
                        var offset = (i / (float)(barrelAmount - 1) - 0.5f) * launchWidth;
                        var barrelPos = launchTrans.position + right * offset;
                        Gizmos.DrawWireSphere(barrelPos, 0.05f);
                    }
                }
            }
        }
    }

    private void Reset()
    {
        launchTrans = transform;
    }

    private void Awake()
    {
        _fsm = new StateMachine<Shooter2D>(this);
        _fsm.TransitionTo(IdleState.Instance);
    }

    private class IdleState : State<Shooter2D, IdleState>
    {
        public override void OnMessage(Shooter2D ctx, string msg)
        {
            if (msg == MSG_FIRE)
            {
                ctx._burstCount = ctx.burstAmount;
                ctx._fsm.TransitionTo(FiringState.Instance);
            }
        }
    }

    private class FiringState : State<Shooter2D, FiringState>
    {
        public override void OnEnter(Shooter2D ctx)
        {
            ctx.CreateProjectiles();
            ctx._burstCount--;
            ctx._elapsedTime = 0;
        }

        public override void OnRefresh(Shooter2D ctx, float deltaTime)
        {
            ctx._elapsedTime += deltaTime;

            var intervalTime = 1 / (ctx.fireRate / 60f);
            if (ctx._elapsedTime >= intervalTime)
            {
                if (ctx._burstCount <= 0)
                {
                    ctx._fsm.TransitionTo(IdleState.Instance);
                }
                else
                {
                    ctx._fsm.TransitionTo(Instance);
                }
            }
        }
    }

    private void CreateProjectiles()
    {
        var referenceRotation = launchTrans.rotation;
        var referencePosition = launchTrans.position;
        var padding = launchWidth / barrelAmount / 2;
        var halfWidth = launchWidth / 2;

        for (var i = 0; i < roundsPerShot; i++)
        {
            // calc barrel offset
            var ratio = (float)_currentBarrelIndex / barrelAmount;
            var y = Mathf.Lerp(-halfWidth, halfWidth, ratio) + padding;
            var offset = new Vector3(0, y);
            var rotatedOffset = (Vector2)(referenceRotation * offset);
            var launchPosition = (Vector2)referencePosition + rotatedOffset;

            // apply launch spread
            var launchRotation = RandomRotate(referenceRotation, spread);

            // spawn projectile
            var projectile = PoolUtil.Spawn(projectilePrefab, launchPosition, launchRotation);

            // init projectile
            var launchVector = (Vector2)(launchRotation * Vector3.right); // ignore z axis
            var launchVelocity = launchVector * launchSpeed;
            projectile.Init(launchVelocity, lifeTime);
            OnLaunch?.Invoke(projectile, launchPosition, launchVelocity); // notify feedbacks
            cueOnLaunch.PlayIfNotNull(launchPosition, launchRotation);
            // _bridge.EventBus.Emit<OnWeaponLaunch>(evt => evt.Subject = gameObject); // notify heat system

            // change barrel
            _currentBarrelIndex = GetNextBarrelIndex(_currentBarrelIndex);
        }
    }

    private static Quaternion RandomRotate(Quaternion reference, float angleRange)
    {
        var half = angleRange / 2;
        var r = RandomUtil.BatesSample(3);
        var angle = Mathf.Lerp(-half, half, r);
        var rotation = Quaternion.Euler(0, 0, angle);
        return reference * rotation;
    }

    private int GetNextBarrelIndex(int currentBarrelIndex)
    {
        switch (barrelType)
        {
            case BarrelPattern.Loop:
                return MathUtil.Mod(currentBarrelIndex + 1, barrelAmount);

            case BarrelPattern.Random:
                return Random.Range(0, barrelAmount);
        }

        return 0;
    }

    public bool Predict(Vector2 targetPosition, Vector2 targetVelocity, out Vector2 interceptVector)
    {
        var launchPosition = LaunchPosition;
        var speed = launchSpeed;
        var los = targetPosition - launchPosition;
        var canIntercept = KinematicUtil.InterceptTime(los, targetVelocity, speed, out var interceptTime);
        interceptVector = los + targetVelocity * interceptTime;
        return canIntercept;
    }

    public Vector2 GetLaunchDirection()
    {
        return launchTrans.right;
    }

    // this method should be invoked before fire in same frame
    public void Cooldown(float deltaTime)
    {
        _fsm.Tick(deltaTime);
    }

    public void Fire()
    {
        _fsm.Send(MSG_FIRE);
    }
}

// modeling other gameplay in different scripts, like charge trigger, continuous firing, ammo, heat, ... etc.
// for example, continuous firing do not invoke cooldown while holding fire button

// public interface IProjectileModifier
// {
//     void Apply(Projectile projectile);
// }
//
// // a demo to show how to modify projectile launched by shooter
// // it aligns faction and inherit velocity
// public class ProjectileModifier : MonoBehaviour, IProjectileModifier
// {
//     [SerializeField] private bool inheritVelocity;
//
//     private Movement _movement; // provide velocity inherit
//     private GameObject _owner; // provide iff align
//
//     public void Apply(Projectile projectile)
//     {
//         // align faction
//         IFFTransponder.CopyIdentity(_owner, projectile.gameObject);
//
//         // inherit velocity
//         if (inheritVelocity)
//         {
//             if (_movement)
//             {
//                 // projectile.InitialVelocity += _movement.Velocity;
//             }
//         }
//     }
// }