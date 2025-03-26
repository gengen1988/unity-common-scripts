using System.Collections.Generic;
using UnityEngine;

/*
public class BrainTargetFinder : MonoBehaviour, ISubmoduleMount
{
    [SerializeField] private Transform searchOrigin;
    [SerializeField] private float searchDistance;

    // private Actor _actor;
    //
    // private Actor _lockedTarget;
    // private List<Actor> _queryBuffer;
    private Vector2 _lastKnownTargetPosition;
    private Vector2 _lastKnownTargetVelocity;

    public Vector2 LastKnownTargetPosition => _lastKnownTargetPosition;
    public Vector2 LastKnownTargetVelocity => _lastKnownTargetVelocity;
    public bool HasTarget => _lockedTarget.IsAlive();

    public void Mount(Submodule submodule)
    {
        _actor = submodule.Owner.EnsureComponent<Actor>();
        _actor.OnMove += HandleBeforeMove;
    }

    private void HandleBeforeMove()
    {
        if (_lockedTarget)
        {
            if (_lockedTarget.IsAlive())
            {
                var targetPosition = _lockedTarget.transform.position;
                var selfPosition = _actor.transform.position;
                if (Vector2.Distance(targetPosition, selfPosition) <= searchDistance)
                {
                    _lastKnownTargetPosition = targetPosition;
                    _lastKnownTargetVelocity = Vector2.zero;
                }
                else
                {
                    _lockedTarget = null;
                }
            }
            else
            {
                _lockedTarget = null;
            }
        }
        else
        {
            _actor.FindOtherActorCircle(searchDistance, _queryBuffer);
            foreach (var found in _queryBuffer)
            {
                if (IFFTransponder.IsFoe(found, _actor))
                {
                    _lockedTarget = found;
                    _lastKnownTargetPosition = found.transform.position;
                    _lastKnownTargetVelocity = Vector2.zero;
                    break;
                }
            }
        }
    }
}
*/