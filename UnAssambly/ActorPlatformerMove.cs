using Sirenix.OdinInspector;
using UnityEngine;

public class ActorPlatformerMove : MonoBehaviour, IEntityAttach
{
    [Header("Setup")]
    [SerializeField] private bool AutoSnap;

    [Header("Move")]
    [SerializeField] private float JumpHeight = 2f;
    [SerializeField] private float MoveSpeed = 5f;
    [SerializeField] private float MaxForce = 100f;
    [SerializeField] private float SteeringScale = 20f;
    [SerializeField] private float GravityScale = 1f;

    [Header("Raycast")]
    [SerializeField] private int VerticleRayCount = 3;
    [SerializeField] private int HorizontalRayCount = 3;
    [SerializeField] private Vector2 CharacterSize = Vector2.one;
    [SerializeField] private float ShellThickness = 0.1f;
    [SerializeField] private LayerMask ObstacleLayers;
    [SerializeField] private LayerMask PlatformLayers;

    private bool _onGround;
    private Actor _actor;
    private Pawn _pawn;
    private ArcadeMovement _movement;

    private void OnDrawGizmosSelected()
    {
        var center = transform.position;
        Gizmos.DrawWireCube(center, CharacterSize);
        Gizmos.DrawWireCube(center, CharacterSize - 2 * ShellThickness * Vector2.one);
    }

    private void Awake()
    {
        _actor = this.EnsureComponent<Actor>();
        _pawn = this.EnsureComponent<Pawn>();
        _movement = this.EnsureComponent<ArcadeMovement>();
        _actor.OnMove += HandleMove;
    }

    private void Start()
    {
        if (AutoSnap)
        {
            SnapToGround();
        }
    }

    public void OnEntityAttach(GameEntity entity)
    {
        _onGround = false;
    }

    private void HandleMove()
    {
        var currentPosition = (Vector2)transform.position;
        var acceleration = Physics2D.gravity * GravityScale;

        // control
        var moveIntent = _pawn.GetVector2(IntentKey.Move);
        var currentVelocity = _movement.Velocity;
        if (_onGround)
        {
            // horizontal move
            var desiredVelocity = moveIntent * MoveSpeed;
            var targetVelocity = desiredVelocity - currentVelocity;
            var steeringForce = Vector2.ClampMagnitude(targetVelocity * SteeringScale, MaxForce);
            steeringForce.y = 0; // walker do not move vertical
            acceleration += steeringForce;

            // jump
            var requireJump = _pawn.GetBool(IntentKey.Jump);
            if (requireJump)
            {
                var velocityX = currentVelocity.x;
                var velocityY = KinematicUtil.JumpVelocityY(JumpHeight);
                currentVelocity = new Vector2(velocityX, velocityY);
                acceleration = Vector2.zero;
            }
        }

        // integral
        var deltaTime = _actor.LocalDeltaTime;
        var deltaVelocity = acceleration * deltaTime;
        var displacement = (currentVelocity + deltaVelocity * 0.5f) * deltaTime;
        var nextVelocity = currentVelocity + deltaVelocity;

        // ground check
        _onGround = false;
        var deltaY = displacement.y;
        var ignorePlatform = deltaY > 0 || moveIntent.y < 0;
        var hit = VerticalCast(currentPosition, deltaY, ignorePlatform);
        if (hit)
        {
            var direction = Mathf.Sign(deltaY);
            // walking on the ground, not just intersection
            displacement = new Vector2(displacement.x, direction * hit.distance);
            nextVelocity = new Vector2(nextVelocity.x, 0);
            if (deltaY < 0)
            {
                _onGround = true;
            }
        }

        // apply
        _movement.SetPosition(currentPosition + displacement);
        _movement.SetVelocity(nextVelocity);
    }

    private RaycastHit2D VerticalCast(Vector2 origin, float deltaY, bool ignorePlatform = false)
    {
        var layers = ObstacleLayers;
        if (!ignorePlatform)
        {
            layers |= PlatformLayers;
        }

        return RaycastUtil.PlatformerRaycast(
            origin,
            VerticleRayCount,
            CharacterSize.x,
            CharacterSize.y / 2,
            ShellThickness,
            Vector2.up * Mathf.Sign(deltaY),
            Mathf.Abs(deltaY),
            layers
        );
    }

    [Button]
    private void SnapToGround()
    {
        var currentPosition = transform.position;
        var hit = VerticalCast(currentPosition, float.NegativeInfinity);
        if (hit)
        {
            transform.position = currentPosition + Vector3.down * hit.distance;
        }
    }
}