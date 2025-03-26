using System.Linq;
using UnityEngine;

/// <summary>
/// states:
/// - born (bypass attract)
/// - dropping
/// - bouncing
/// </summary>
[RequireComponent(typeof(EntityProxy))]
public class Pickup : MonoBehaviour, IEntityAttach, IEntityFrame
{
    [SerializeField] private float bodyRadius = .5f;
    [SerializeField] private float elasticFactor = .2f;
    [SerializeField] private float protectTime = 0.5f;
    [SerializeField] private float collectRange = 1f;
    [SerializeField] private float attractSpeed = 10f;
    [SerializeField] private SeekControl attractSteering;

    [Header("Gameplay Cues")]
    [SerializeField] private CueChannel cueCollected;

    private float _elapsedTime;
    private bool _isProtect;
    private Vector2 _position;
    private Vector2 _velocity;
    private Vector2 _initialVelocity;
    private Rigidbody2D _rb;
    private PickupCollector _collector;
    private Collider2D[] _colliders;

    private void Awake()
    {
        TryGetComponent(out _rb);
        _colliders = GetComponentsInChildren<Collider2D>()
            .Where(col => col.gameObject.layer == CustomLayer.Collectable)
            .ToArray();
    }

    private void SetCollidersEnabled(bool value)
    {
        foreach (var col in _colliders)
        {
            col.enabled = value;
        }
    }

    // this covers basic init, to handle direct drag to scene
    public void OnEntityAttach(GameEntity entity)
    {
        _elapsedTime = 0;
        _isProtect = true;
        _position = _rb.position;
        _velocity = _initialVelocity;
        _initialVelocity = Vector2.zero;
        _collector = null;
    }

    public void OnEntityFrame(GameEntity entity, float deltaTime)
    {
        if (_isProtect)
        {
            if (_elapsedTime < protectTime)
            {
                _elapsedTime += deltaTime;
            }
            else
            {
                SetCollidersEnabled(true);
                _isProtect = false;
            }
        }

        if (_collector)
        {
            TickAttractMove(deltaTime);
        }
        else
        {
            TickNeutralMove(deltaTime);
        }
    }

    private bool CorrectMove(float body, Vector2 currentPosition, ref Vector2 nextPosition, int layerMask = 1 << CustomLayer.Obstacle)
    {
        if (Mathf.Approximately(currentPosition.x, nextPosition.x) && Mathf.Approximately(currentPosition.y, nextPosition.y))
        {
            return false;
        }

        var displacement = nextPosition - currentPosition;
        var hit = Physics2D.Raycast(currentPosition, displacement.normalized, displacement.magnitude, layerMask);
        if (hit)
        {
            var actualMoveDistance = hit.distance - bodyRadius;
            var actualDisplacement = displacement.normalized * actualMoveDistance;
            nextPosition = currentPosition + actualDisplacement;
            return true;
        }

        return false;
    }

    private void TickNeutralMove(float deltaTime)
    {
        // apply move
        var gravity = Physics2D.gravity;
        var nextVelocity = _velocity + gravity * deltaTime;
        var displacement = nextVelocity * deltaTime;
        var currentPosition = _position;

        // correction
        var nextPosition = currentPosition + displacement;
        var isHitGround = CorrectMove(bodyRadius, currentPosition, ref nextPosition);
        if (isHitGround)
        {
            nextVelocity.y = -nextVelocity.y * elasticFactor;
        }

        // apply
        _rb.MovePosition(nextPosition);
        _position = nextPosition;
        _velocity = nextVelocity;
    }

    private void TickAttractMove(float deltaTime)
    {
        // apply move
        var los = _collector.CenterPosition - _position;
        var attractForce = attractSteering.CalcControlForce(los, _velocity, attractSpeed);
        var nextVelocity = _velocity + attractForce * deltaTime;
        var displacement = nextVelocity * deltaTime;
        var nextPosition = _position + displacement;
        _rb.MovePosition(nextPosition);
        _position = nextPosition;
        _velocity = nextVelocity;

        // pick check
        var distance = (nextPosition - _collector.CenterPosition).magnitude;
        if (distance < collectRange)
        {
            cueCollected.PlayIfNotNull(_collector.CenterPosition, Quaternion.identity);
            PoolUtil.Despawn(gameObject);
        }
    }

    // this covers spawn by system
    // jump from unit?
    // execute order:
    // --- frame xxx early
    // - on fixed update
    //   - projectile report hit
    // --- frame 1
    // - on pre update
    //   - hitstop end
    //   - deal damage
    //   - health death
    //   - unit handle death event
    //     - trigger on unit death global event
    //     - loot system spawn item
    //     - born
    // --- frame 2
    // - on fixed update
    //   - on entity attach
    //   - on entity frame
    public void Born(Vector2 initialVelocity)
    {
        _initialVelocity = initialVelocity;
    }

    public void AttractBy(PickupCollector collector)
    {
        SetCollidersEnabled(false);
        // _velocity = Vector2.up * activateSpeed;
        _collector = collector;
    }
}