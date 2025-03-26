using UnityEngine;

public class BrainChaseFoe : MonoBehaviour, IBrainObserve<UnitContext>, IBrainDecide<UnitContext>
{
    // [SerializeField] private bool refreshEveryFrame;

    private EntityProxy _self;
    private IMoveState _selfMove;
    private ITargetProvider _designator;
    private Vector2 _lastKnownTargetPosition;

    private void Awake()
    {
        TryGetComponent(out _designator);
        _self = GetComponentInParent<EntityProxy>();
        _selfMove = _self.GetComponent<IMoveState>();
    }

    public void Observe(UnitContext context)
    {
        _designator.Refresh();
        // if (refreshEveryFrame)
        // {
        // }
        // else if (!_designator.IsTargetLocked)
        // {
        //     _designator.Refresh();
        // }
    }

    public void Decide(UnitContext context)
    {
        if (!_designator.LockedTarget)
        {
            context.MoveIntent = Vector2.zero; // stop when not found
            context.FocusedEnemy = null;
            return;
        }

        var los = _designator.LastKnownTargetPosition - _selfMove.Position;
        context.MoveIntent = los;
        context.FocusedEnemy = _designator.LockedTarget;
    }
}