using UnityEngine;

/*
public class BrainMoveToTarget : MonoBehaviour, ISubmoduleMount
{
    [SerializeField] private float findRadius = 20f;

    private Pawn _pawn;
    private BrainTargetFinder _targetFinder;

    public void Mount(Submodule submodule)
    {
        var owner = submodule.Owner;
        _targetFinder = this.EnsureComponent<BrainTargetFinder>();
        // _actor = owner.EnsureComponent<Actor>();
        // _actor.OnMove += HandleBeforeMove;
        _pawn = owner.EnsureComponent<Pawn>();
    }

    private void HandleBeforeMove()
    {
        if (_targetFinder.HasTarget)
        {
            var targetPosition = _targetFinder.LastKnownTargetPosition;
            // var selfPosition = (Vector2)_actor.transform.position;
            // var los = targetPosition - selfPosition;
            // _pawn.SetVector2(IntentKey.Move, los);
        }
    }
}
*/