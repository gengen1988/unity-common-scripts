using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ITargetProvider
{
    public GameEntity LockedTarget { get; }
    public Vector2 LastKnownTargetPosition { get; }
    public Vector2 LastKnownTargetVelocity { get; }
    public void Refresh();
}

public class TargetProviderStandalone : MonoBehaviour, ITargetProvider
{
    [SerializeField] private float queryRange = 5f;
    [SerializeField] private float trackingRange = 10f;

    private EntityProxy _self;
    private IMoveState _selfMove;
    private IMoveState _targetMove;

    private readonly List<Collider2D> _queryBuffer = new();

    public GameEntity LockedTarget { get; private set; }
    public Vector2 LastKnownTargetPosition { get; private set; }
    public Vector2 LastKnownTargetVelocity { get; private set; }

    private void Awake()
    {
        _self = GetComponentInParent<EntityProxy>();
        _selfMove = _self.GetComponent<IMoveState>();
    }

    public void Refresh()
    {
        // distance check
        if (LockedTarget)
        {
            var targetPosition = _targetMove.Position;
            var selfPosition = _selfMove.Position;
            var distance = Vector2.Distance(targetPosition, selfPosition);
            if (distance > trackingRange)
            {
                // target lost
                SetLockedTarget(null);
            }
        }

        // query new target
        if (!LockedTarget)
        {
            var filter = new ContactFilter2D
            {
                useTriggers = true,
                useLayerMask = true,
                layerMask = 1 << CustomLayer.Default,
            };

            var queryCenter = _selfMove.Position;
            Physics2D.OverlapCircle(queryCenter, queryRange, filter, _queryBuffer);
            var nearestFoe = _queryBuffer
                .Where(col => !col.transform.IsChildOf(_self.transform))
                .Select(col => col.GetAttachedTransform().GetComponent<Unit>())
                .Where(unit => unit && IFFTransponder.IsFoe(_self, unit))
                .OrderBy(unit => Vector2.Distance(_selfMove.Position, unit.Position))
                .FirstOrDefault();
            if (nearestFoe)
            {
                SetLockedTarget(nearestFoe);
            }
        }

        // update monitoring
        if (LockedTarget)
        {
            LastKnownTargetPosition = _targetMove.Position;
            LastKnownTargetVelocity = _targetMove.Velocity;
        }
    }

    private void SetLockedTarget(Unit target)
    {
        if (target)
        {
            var entity = target.GetEntity();
            var move = entity.GetMove();
            LockedTarget = entity;
            _targetMove = move;
        }
        else
        {
            LockedTarget = null;
            _targetMove = null;
        }
    }
}