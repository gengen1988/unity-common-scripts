using System;
using UnityEngine;

[Obsolete] // replaced with vehicle move, remained as a reference
public class ControlledPlatformerMove : MonoBehaviour
{
    // [Header("Setup")]
    // [SerializeField] private bool autoSnap;
    //
    // [Header("Move")]
    // [SerializeField] private float jumpHeight = 2f;
    // [SerializeField] private float moveSpeed = 5f;
    // [SerializeField] private float maxForce = 100f;
    // [SerializeField] private float steeringScale = 20f;
    // [SerializeField] private float gravityScale = 1f;
    //
    // [Header("Raycast")]
    // [SerializeField] private int verticalRayCount = 3;
    // [SerializeField] private int horizontalRayCount = 3;
    // [SerializeField] private Vector2 characterSize = Vector2.one;
    // [SerializeField] private Vector2 offset = Vector2.zero;
    // [SerializeField] private float shellThickness = 0.1f;
    // [SerializeField] private LayerMask obstacleLayers = 1;
    // [SerializeField] private LayerMask platformLayers;
    //
    // private bool _onGround;
    // private Actor _actor;
    // private Pawn _pawn;
    // private Movement _movement;
    //
    // private void OnDrawGizmosSelected()
    // {
    //     var center = (Vector2)transform.position + offset;
    //     Gizmos.DrawWireCube(center, characterSize);
    //     Gizmos.DrawWireCube(center, characterSize - 2 * shellThickness * Vector2.one);
    // }
    //
    // private void Awake()
    // {
    //     _actor = this.EnsureComponent<Actor>();
    //     _movement = this.EnsureComponent<Movement>();
    // }
    //
    // private void OnEnable()
    // {
    //     _actor.OnMove += HandleMove;
    // }
    //
    // private void OnDisable()
    // {
    //     _actor.OnMove -= HandleMove;
    // }
    //
    // private void Start()
    // {
    //     if (autoSnap)
    //     {
    //         SnapToGround();
    //     }
    // }
    //
    // public void OnEntityAttach(GameEntity entity)
    // {
    //     _onGround = false;
    // }
    //
    // private void HandleMove()
    // {
    //     var currentPosition = (Vector2)transform.position;
    //     var acceleration = Physics2D.gravity * gravityScale;
    //
    //     // control
    //     var moveIntent = _actor.Pawn.GetVector2(IntentKey.Move);
    //     var currentVelocity = _movement.Velocity;
    //     if (_onGround)
    //     {
    //         // horizontal move
    //         var desiredVelocity = moveIntent * moveSpeed;
    //         var targetVelocity = desiredVelocity - currentVelocity;
    //         var steeringForce = Vector2.ClampMagnitude(targetVelocity * steeringScale, maxForce);
    //         steeringForce.y = 0; // walker do not move vertical
    //         acceleration += steeringForce;
    //
    //         // jump
    //         var requireJump = _actor.Pawn.GetBool(IntentKey.Jump);
    //         if (requireJump)
    //         {
    //             var velocityX = currentVelocity.x;
    //             var velocityY = KinematicUtil.JumpVelocityY(jumpHeight);
    //             currentVelocity = new Vector2(velocityX, velocityY);
    //             acceleration = Vector2.zero;
    //         }
    //     }
    //
    //     // integral
    //     var deltaTime = _actor.Clock.LocalDeltaTime;
    //     var deltaVelocity = acceleration * deltaTime;
    //     var displacement = (currentVelocity + deltaVelocity * 0.5f) * deltaTime;
    //     var nextVelocity = currentVelocity + deltaVelocity;
    //
    //     // ground check
    //     _onGround = false;
    //     var deltaY = displacement.y;
    //     var ignorePlatform = deltaY > 0 || moveIntent.y < 0;
    //     var hit = VerticalCast(currentPosition, deltaY, ignorePlatform);
    //     if (hit)
    //     {
    //         var direction = Mathf.Sign(deltaY);
    //         // walking on the ground, not just intersection
    //         displacement = new Vector2(displacement.x, direction * hit.distance);
    //         nextVelocity = new Vector2(nextVelocity.x, 0);
    //         if (deltaY < 0)
    //         {
    //             _onGround = true;
    //         }
    //     }
    //
    //     // apply
    //     _movement.SetPosition(currentPosition + displacement);
    //     _movement.SetVelocity(nextVelocity);
    // }
    //
    // private RaycastHit2D VerticalCast(Vector2 origin, float deltaY, bool ignorePlatform = false)
    // {
    //     var layers = obstacleLayers;
    //     if (!ignorePlatform)
    //     {
    //         layers |= platformLayers;
    //     }
    //
    //     // Apply the raycast origin offset
    //     var offsetOrigin = origin + offset;
    //
    //     return RaycastUtil.PlatformerRaycast(
    //         offsetOrigin,
    //         verticalRayCount,
    //         characterSize.x,
    //         characterSize.y / 2,
    //         shellThickness,
    //         Vector2.up * Mathf.Sign(deltaY),
    //         Mathf.Abs(deltaY),
    //         layers,
    //         Filter
    //     );
    // }
    //
    // private bool Filter(RaycastHit2D hit)
    // {
    //     var selfTrans = _actor.transform;
    //     var hitTrans = hit.collider.transform;
    //     if (hitTrans.IsChildOf(selfTrans))
    //     {
    //         return false;
    //     }
    //
    //     return true;
    // }
    //
    // [Button]
    // private void SnapToGround()
    // {
    //     var currentPosition = transform.position;
    //     var hit = VerticalCast(currentPosition, float.NegativeInfinity);
    //     if (hit)
    //     {
    //         transform.position = currentPosition + Vector3.down * hit.distance;
    //     }
    // }
    //
    // public MoveStateType CalcNextState(MoveStateType currentStateType, float deltaTime)
    // {
    //     throw new System.NotImplementedException();
    // }
}