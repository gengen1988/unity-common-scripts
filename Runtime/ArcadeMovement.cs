using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ArcadeMovement : MonoBehaviour, IEntityAttach
{
    [SerializeField] private float LinearDrag = 10f;
    // [SerializeField] private float GravityScale = 1f;

    private Vector2 _position;
    private Vector2 _velocity;

    private Vector2 _initialVelocity;
    private Rigidbody2D _rb;

    // private Quaternion _nextRotation;
    // private bool _teleportRotation;

    private Vector2 _lookDirection;
    private Vector2 _nextLookDirection;
    private Vector2 _nextPosition;
    private Vector2 _nextVelocity;
    private Vector2 _forceInThisFrame;
    private bool _isPositionChangedManually;
    private bool _isVelocityChangedManually;
    private bool _isForceApplied;
    private bool _isRotationChangedManually;

    public Vector2 Velocity => _velocity;
    public Vector2 Position => _position;
    public Vector2 LookDirection => _lookDirection;

    private void Awake()
    {
        _rb = this.EnsureComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _rb.bodyType = RigidbodyType2D.Kinematic;
        _position = transform.position;
    }

    public void OnEntityAttach(GameEntity entity)
    {
        _velocity = _initialVelocity; // must after enable
    }

    private Vector2 GetNextVelocity(float deltaTime)
    {
        // controlled changes
        if (_isVelocityChangedManually && _isForceApplied)
        {
            return _nextVelocity + _forceInThisFrame * deltaTime;
        }
        else if (_isVelocityChangedManually && !_isForceApplied)
        {
            return _nextVelocity;
        }
        else if (!_isVelocityChangedManually && _isForceApplied)
        {
            return _velocity + _forceInThisFrame * deltaTime;
        }

        // drag changes 
        if (!Mathf.Approximately(LinearDrag, 0f))
        {
            return MathUtil.ChangeVectorMagnitude(_velocity, -LinearDrag * deltaTime);
        }

        // no change
        return _velocity;
    }

    public void Tick(float deltaTime)
    {
        // position change by manual (usually controlled motion)
        if (_isPositionChangedManually)
        {
            // update velocity
            var nextVelocity = GetNextVelocity(deltaTime); // do we need velocity change here?
            _velocity = nextVelocity;
            _position = _nextPosition;
        }
        // position change by velocity (usually physics simulation)
        else
        {
            var nextVelocity = GetNextVelocity(deltaTime);
            var deltaVelocity = nextVelocity - _velocity;
            var displacement = (_velocity + 0.5f * deltaVelocity) * deltaTime;
            var currentPosition = (Vector2)transform.position; // sync transform changes
            var nextPosition = currentPosition + displacement;
            _velocity = nextVelocity;
            _position = nextPosition;
        }

        // rotation
        if (_isRotationChangedManually)
        {
            _lookDirection = _nextLookDirection;
        }
        else
        {
            _lookDirection = transform.right; // sync transform changes
        }

        // apply to rigidbody
        _rb.MoveRotation(MathUtil.AngleByVector(_lookDirection)); // interpolation
        // transform.rotation = MathUtil.QuaternionByVector(_lookDirection); // direct change
        _rb.MovePosition(_position);

        // reset states
        _forceInThisFrame = Vector2.zero;
        _isForceApplied = false;
        _isRotationChangedManually = false;
        _isVelocityChangedManually = false;
        _isPositionChangedManually = false;
    }

    public void SetForce(Vector2 force)
    {
        _isForceApplied = true;
        _forceInThisFrame = force;
    }

    public void AddForce(Vector2 force)
    {
        _isForceApplied = true;
        _forceInThisFrame += force;
    }

    public void SetVelocity(Vector2 velocity)
    {
        // Debug.Log($"{name} velocity set to {velocity})", this);
        _isVelocityChangedManually = true;
        _nextVelocity = velocity;
    }

    public void SetPosition(Vector2 position)
    {
        _isPositionChangedManually = true;
        _nextPosition = position;
    }

    public void SetLookDirection(Vector2 direction)
    {
        _isRotationChangedManually = true;
        _nextLookDirection = direction;
    }

    public Vector2 PredictNextPosition(float deltaTime)
    {
        var nextVelocity = MathUtil.ChangeVectorMagnitude(_velocity, -LinearDrag * deltaTime);
        var deltaVelocity = nextVelocity - _velocity;
        var displacement = (_velocity + 0.5f * deltaVelocity) * deltaTime;
        return _position + displacement;
    }

    public void SetInitialVelocity(Vector2 velocity)
    {
        _initialVelocity = velocity;
    }
}