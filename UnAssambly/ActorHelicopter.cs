using UnityEngine;
using UnityEngine.Serialization;

public class ActorHelicopter : MonoBehaviour
{
    private const float DEADZONE = 0.5f;
    private const float TURNING_TOLERANCE = 5f;

    [Header("Movement")]
    [SerializeField] private float MaxSpeed = 5f; // 按住向前移动时的速度
    [SerializeField] private float UpSpeed = 3f;
    [SerializeField] private float DownSpeed = 4f;
    [SerializeField] private float TurningTime = 0.8f; // 转弯时间
    [SerializeField] private float MaxPitch = 25;
    [SerializeField] private float DampingX = 0.1f; // 水平回正的阻力系数 (实际阻力和速度平方成比例)。减速到巡航速度

    [Header("Component")]
    [SerializeField] private Transform RotationTransform;

    [FormerlySerializedAs("PitchController")]
    [Header("Controller Characteristics")]
    [SerializeField] private PIDControllerFloat pitchControllerFloat;
    [FormerlySerializedAs("LevelingController")] [SerializeField] private PIDControllerFloat levelingControllerFloat; // 这个是必要的，因为回正速度要小于控制速度
    [FormerlySerializedAs("HorizontalController")] [SerializeField] private PIDControllerFloat horizontalControllerFloat;
    [FormerlySerializedAs("UpwardController")] [SerializeField] private PIDControllerFloat upwardControllerFloat;
    [FormerlySerializedAs("DownwardController")] [SerializeField] private PIDControllerFloat downwardControllerFloat;

    private Vector2 _moveIntent;
    private float _yaw;
    private float _pitch;
    private float _pitchVelocity;
    private int _facing;
    private bool _isTurning;
    private bool _isReverseProtecting;
    private int _turningDirection;
    private float _turningStartPitch;
    private float _elapsedTime;

    // related components
    private Pawn _pawn;
    private Actor _actor;
    private ArcadeMovement _movement;

    private void Awake()
    {
        _pawn = this.EnsureComponent<Pawn>();
        _movement = this.EnsureComponent<ArcadeMovement>();
        _actor = this.EnsureComponent<Actor>();
        _actor.OnMove += HandleMove;
    }

    private void OnEnable()
    {
        _facing = Mathf.Approximately(transform.eulerAngles.y, 0f) ? 1 : -1;
    }

    private void Update()
    {
        // 一旦修改了 transform 那么就会导致插帧失效
        // 因此不能修改 _movement 的 transform
        RotationTransform.rotation = Quaternion.Euler(0, _yaw, _pitch);
    }

    private void HandleMove()
    {
        _moveIntent = _pawn.GetVector2(IntentKey.Move);
        var deltaTime = _actor.LocalDeltaTime;

        if (_isTurning)
        {
            TickTurning(deltaTime);
        }

        TickMovement(deltaTime);

        // // interpolate rotation
        // var rotation = Quaternion.Euler(0, _yaw, _pitch);
        // _movement.MoveRotation(rotation);
    }

    private void TickTurning(float deltaTime)
    {
        _elapsedTime += deltaTime;

        var progress = MathUtil.Remap(_elapsedTime, 0, TurningTime, 0, 1);
        var smoothedProgress = Mathf.SmoothStep(0f, 1, progress);
        var angleProgress = MathUtil.Remap(smoothedProgress, 0, 1, 0, 180f);
        _yaw = _turningDirection > 0 ? angleProgress : 180f - angleProgress;

        // before half
        if (_isReverseProtecting)
        {
            // 俯仰控制
            var desiredPitch = _turningStartPitch * Mathf.Cos(angleProgress * Mathf.Deg2Rad);
            _pitch = desiredPitch;

            // state transition
            if (_elapsedTime >= TurningTime / 2)
            {
                _facing = -_facing;
                _pitchVelocity = -MaxPitch * Mathf.Sin(angleProgress * Mathf.Deg2Rad);
                pitchControllerFloat.Reset();
                _isReverseProtecting = false;
            }
        }
        else
        {
            // test if rotate back?
        }

        // state transition
        if (_elapsedTime >= TurningTime)
        {
            _yaw = _facing > 0 ? 0 : 180f;
            _isTurning = false;
        }
    }

    private void TickMovement(float deltaTime)
    {
        var velocity = _movement.Velocity;
        var vx = velocity.x;
        var vy = velocity.y;

        // 前后键
        if (Mathf.Abs(_moveIntent.x) > DEADZONE)
        {
            // 俯仰控制
            if (!_isReverseProtecting)
            {
                var pitchDirection = -Mathf.Sign(_moveIntent.x * _facing);
                var desiredPitch = pitchDirection * MaxPitch;
                var controlForce = pitchControllerFloat.Update(desiredPitch, _pitch, deltaTime);
                MathUtil.MoveByForce(ref _pitch, ref _pitchVelocity, controlForce, deltaTime);
            }

            // 水平移动
            if (_isReverseProtecting)
            {
                var controlForce = horizontalControllerFloat.Update(0, vx, deltaTime);
                vx += controlForce * deltaTime;
            }
            else
            {
                var controlForce = horizontalControllerFloat.Update(MaxSpeed * _moveIntent.x, vx, deltaTime);
                // Debug.Log($"horizontal force: {controlForce}");
                vx += controlForce * deltaTime;
            }
        }
        else
        {
            if (!_isReverseProtecting)
            {
                // 自动水平
                var controlForce = levelingControllerFloat.Update(0, _pitch, deltaTime);
                MathUtil.MoveByForce(ref _pitch, ref _pitchVelocity, controlForce, deltaTime);
            }

            // 自动巡航
            vx = Mathf.MoveTowards(vx, 0, DampingX * vx * vx * deltaTime);
            horizontalControllerFloat.Reset();
        }

        // 上下键
        if (Mathf.Abs(_moveIntent.y) > DEADZONE)
        {
            // 垂直运动
            if (_moveIntent.y > 0)
            {
                var controlForce = upwardControllerFloat.Update(UpSpeed, vy, deltaTime);
                vy += controlForce * deltaTime;
            }
            else
            {
                // 向下运动应该比向上运动快一些
                var controlForce = downwardControllerFloat.Update(-DownSpeed, vy, deltaTime);
                vy += controlForce * deltaTime;
            }
        }
        else
        {
            // 垂直回零
            // 不使用风阻式轨迹，因为垂直运动更多是受控平衡
            if (velocity.y > 0)
            {
                var controlForce = downwardControllerFloat.Update(0, vy, deltaTime);
                vy += controlForce * deltaTime;
            }
            else if (velocity.y < 0)
            {
                var controlForce = upwardControllerFloat.Update(0, vy, deltaTime);
                vy += controlForce * deltaTime;
            }
        }

        var nextVelocity = new Vector2(vx, vy);
        var deltaVelocity = nextVelocity - velocity;
        var displacement = (velocity + 0.5f * deltaVelocity) * deltaTime;
        // var displacement = nextVelocity * deltaTime; // 内插不能用牛顿法？
        var nextPosition = _movement.Position + displacement;
        // var nextPosition = _movement.Position + 0.7f * Vector2.right;

        // 更新物理状态
        // Debug.Log($"desired displacement: {displacement}");
        _movement.SetPosition(nextPosition);
        _movement.SetVelocity(nextVelocity);

        // 状态转移
        if (_pitch >= MaxPitch - TURNING_TOLERANCE && !_isReverseProtecting)
        {
            _turningStartPitch = _pitch;
            _pitchVelocity = 0;
            _isTurning = true;
            _isReverseProtecting = true;
            _turningDirection = _facing;
            _elapsedTime = 0;
        }
    }
}