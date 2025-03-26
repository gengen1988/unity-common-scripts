using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IEntityAttach, IEntityFrame, IMoveState, IBrainManager<UnitContext>
{
    [SerializeField] private Vector2 centerOffset;
    [SerializeField] private float recoveryTime = .2f;
    [SerializeField] private float knockbackScale = 1f; // represent mass

    private bool _inControl;
    private float _recoveryCooldown;

    private Rigidbody2D _rb;
    private IMovement<UnitContext> _movement;
    private IKillable _killable;
    private TurretFCS _fcs;

    private readonly UnitContext _context = new(); // blackboard
    private readonly List<IBrain<UnitContext>> _brains = new();

    public Vector2 Position => _context.Position;
    public Vector2 Velocity => _context.Velocity;
    public Vector2 CenterPosition => _context.Position + centerOffset;

    private void Awake()
    {
        TryGetComponent(out _rb);
        TryGetComponent(out _fcs);
        TryGetComponent(out _killable);
        TryGetComponent(out _movement);
        _killable.OnDeath += HandleDeath;
    }

    public void OnEntityAttach(GameEntity entity)
    {
        // reset
        _context.Position = transform.position;
        _context.Velocity = Vector2.zero;
    }

    public void OnEntityFrame(GameEntity entity, float deltaTime)
    {
        // bypass if already die
        Debug.Assert(_killable.IsAlive);

        // refresh control values
        if (_brains.Count > 0)
        {
            var currentBrain = _brains[^1];
            currentBrain.Refresh(_context);
        }

        // self movement
        if (_inControl)
        {
            TickControl(deltaTime);
        }
        else
        {
            TickUncontrol(deltaTime);
        }

        // turrets control
        if (_fcs)
        {
            _fcs.SetTarget(_context.FocusedEnemy);
            _fcs.SetFallbackDirection(_context.LookIntent);
            if (_context.RequestAttack)
            {
                _fcs.RequestFire();
            }

            _fcs.Tick(deltaTime);
        }
    }

    private void TickControl(float deltaTime)
    {
        _movement.Move(_context, deltaTime);
        _rb.MovePosition(_context.Position);
    }

    private void TickUncontrol(float deltaTime)
    {
        _recoveryCooldown -= deltaTime;
        var displacement = _context.Velocity * deltaTime;
        var nextPosition = _context.Position + displacement;
        _context.Position = nextPosition;
        _rb.MovePosition(_context.Position);
        if (_recoveryCooldown <= 0)
        {
            Recover();
        }
    }

    private void Recover()
    {
        _inControl = true;
    }

    private void HandleDeath()
    {
        GlobalEventBus<OnUnitDeath>.Raise(new OnUnitDeath
        {
            Subject = this,
        });
        PoolUtil.Despawn(gameObject);
    }

    public void AddBrain(IBrain<UnitContext> brain)
    {
        _brains.Add(brain);
    }

    public void RemoveBrain(IBrain<UnitContext> brain)
    {
        _brains.Remove(brain);
    }

    public void Knockback(Vector2 velocity)
    {
        _inControl = false;
        _recoveryCooldown = recoveryTime;
        _context.Velocity = velocity * knockbackScale;
    }
}

public struct OnUnitDeath : IGlobalEvent
{
    public Unit Subject;
}