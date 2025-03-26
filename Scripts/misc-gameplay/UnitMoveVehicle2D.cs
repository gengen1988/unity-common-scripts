using Animancer;
using UnityEngine;

public class UnitMoveVehicle2D : MonoBehaviour, IMovement<UnitContext>
{
    [Header("Raycast")]
    [SerializeField] private int verticalRayCount = 3;
    [SerializeField] private float shellThickness = .01f;
    [SerializeField] private LayerMask obstacleLayers = 1;
    [SerializeField] private Vector2 castOffset = Vector2.zero;
    [SerializeField] private Vector2 bodySize = Vector2.one;

    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private ProportionalControl accelerationControl; // Force applied when accelerating to speed
    [SerializeField] private ProportionalControl decelerationControl; // Force applied when decelerating to 0

    [Header("Tilt Control")]
    [SerializeField] private Transform flipTrans;
    [SerializeField] private float flipTime = 1f; // time required from left to right
    [SerializeField] private float maxRollAngle = 30f; // related with rotation
    [SerializeField] private float maxPitchAngle = 5f; // related with steering force

    [Header("Animation Control")]
    [SerializeField] private AnimancerComponent animancer;
    [SerializeField] private AnimationClip idleClip;
    [SerializeField] private AnimationClip moveClip;
    [SerializeField] private AnimationClip jumpClip;
    [SerializeField] private float moveAnimationThreshold = 1f;

    private bool _isFlipping;
    private float _desiredFlip; // 1 for right, -1 for left, 0 for towards screen
    private float _currentFlip; // linear progression, remap [-1, 1] to [180, 0]
    private float _currentSpeed;

    private void OnDrawGizmosSelected()
    {
        // Draw the raycast body as a wire cube
        Gizmos.color = Color.green;

        // Calculate the center position of the raycast body
        var center = transform.position + new Vector3(castOffset.x, castOffset.y, 0);

        // Draw the body size
        Gizmos.DrawWireCube(center, new Vector3(bodySize.x, bodySize.y, 0.1f));

        // Draw the raycast points
        Gizmos.color = Color.yellow;
        var raySpacing = bodySize.x / (verticalRayCount - 1);
        var leftEdge = -bodySize.x / 2;

        // Draw bottom rays
        for (var i = 0; i < verticalRayCount; i++)
        {
            var x = leftEdge + i * raySpacing;
            var rayStart = center + new Vector3(x, -bodySize.y / 2, 0);
            var rayEnd = rayStart - new Vector3(0, shellThickness, 0);
            Gizmos.DrawLine(rayStart, rayEnd);
        }

        // Draw top rays
        for (var i = 0; i < verticalRayCount; i++)
        {
            var x = leftEdge + i * raySpacing;
            var rayStart = center + new Vector3(x, bodySize.y / 2, 0);
            var rayEnd = rayStart + new Vector3(0, shellThickness, 0);
            Gizmos.DrawLine(rayStart, rayEnd);
        }
    }

    private void OnEnable()
    {
        // Sync the initial flip value to match the current rotation
        if (flipTrans != null)
        {
            // Extract the Y rotation angle and convert it to our flip scale [-1, 1]
            var currentYRotation = flipTrans.localRotation.eulerAngles.y;
            // Normalize the angle to [0, 360]
            if (currentYRotation > 180f)
                currentYRotation -= 360f;

            // Map from [0, 180] to [1, -1]
            _currentFlip = MathUtil.Remap(currentYRotation, 0, 180, 1, -1);
            _desiredFlip = _currentFlip;
        }
        else
        {
            // If no flip transform is assigned, use the gameObject's transform
            var currentYRotation = transform.localRotation.eulerAngles.y;
            if (currentYRotation > 180f)
                currentYRotation -= 360f;

            _currentFlip = MathUtil.Remap(currentYRotation, 0, 180, 1, -1);
            _desiredFlip = _currentFlip;
        }
    }

    public void Move(UnitContext context, float deltaTime)
    {
        var moveIntent = context.MoveIntent;

        // gravity
        var gravity = Physics2D.gravity;
        var nextVelocity = context.Velocity;
        nextVelocity += gravity * deltaTime;

        // ground check
        var displacementY = nextVelocity.y * deltaTime;
        var hit = RaycastUtil.PlatformerRaycast(
            context.Position + castOffset,
            verticalRayCount,
            bodySize.x,
            bodySize.y / 2,
            shellThickness,
            Vector2.up * Mathf.Sign(displacementY),
            Mathf.Abs(displacementY),
            obstacleLayers
        );

        var isGrounded = false;
        if (hit)
        {
            var direction = Mathf.Sign(displacementY);
            displacementY = direction * hit.distance;
            nextVelocity.y = 0;
            isGrounded = true;
        }

        // determine if need flip
        var intentDirection = Mathf.Sign(moveIntent.x);
        var isIntentionalMove = Mathf.Abs(moveIntent.x) > Mathf.Epsilon;
        if (isIntentionalMove && !Mathf.Approximately(intentDirection, _currentFlip) && isGrounded)
        {
            _isFlipping = true;
            _desiredFlip = intentDirection;
        }

        var yawAngle = flipTrans.localRotation.eulerAngles.y;
        var pitchAngle = MathUtil.Remap(_currentSpeed, 0, speed, 0, maxPitchAngle);
        var rollAngle = Mathf.Sin(yawAngle * Mathf.Deg2Rad) * maxRollAngle * _desiredFlip;

        // flip
        if (_isFlipping)
        {
            // rotation
            var flipSpeed = 2 / flipTime;
            var nextFlip = Mathf.MoveTowards(_currentFlip, _desiredFlip, flipSpeed * deltaTime);
            var progress = MathUtil.RemapTo01(nextFlip, -1, 1);
            yawAngle = MathUtil.RemapFrom01(progress, 180, 0);
            _currentFlip = nextFlip;

            // exit condition
            if (Mathf.Approximately(_currentFlip, _desiredFlip))
            {
                nextVelocity.x = _desiredFlip * _currentSpeed;
                _isFlipping = false;
            }
        }

        // jump
        var isJumping = false;
        var requestJump = context.RequestJump;
        if (isGrounded && requestJump)
        {
            var jumpVelocityY = KinematicUtil.JumpVelocityY(jumpHeight);
            nextVelocity.y = jumpVelocityY;
        }

        // horizontal movement
        var desiredSpeed = isIntentionalMove ? speed : 0;
        var error = desiredSpeed - _currentSpeed;
        var force = isIntentionalMove ? accelerationControl.CalcControlForce(error) : decelerationControl.CalcControlForce(error);
        var forceAbs = Mathf.Abs(force);
        var nextSpeed = Mathf.MoveTowards(_currentSpeed, desiredSpeed, forceAbs * deltaTime);
        nextVelocity.x = Mathf.Cos(yawAngle * Mathf.Deg2Rad) * nextSpeed;
        var displacement = nextVelocity * deltaTime;
        displacement.y = displacementY;
        var nextPosition = context.Position + displacement;

        // tilt
        flipTrans.localRotation = Quaternion.Euler(rollAngle, yawAngle, pitchAngle);

        // animation
        if (isGrounded)
        {
            if (_currentSpeed >= moveAnimationThreshold || _isFlipping)
            {
                animancer.Play(moveClip);
            }
            else
            {
                animancer.Play(idleClip);
            }
        }
        else
        {
            animancer.Play(jumpClip);
        }

        // apply to context
        _currentSpeed = nextSpeed;
        context.Position = nextPosition;
        context.Velocity = nextVelocity;
        context.IsGrounded = isGrounded;
    }
}