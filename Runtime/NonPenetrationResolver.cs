using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(100)]
public class NonPenetrationResolver : MonoBehaviour
{
    [SerializeField] private float DistanceToPlayer = 32f;

    private ActorOld _playerActorOld;
    private readonly List<ActorOld> _queryBuffer = new();
    private readonly List<ActorNonPenetration> _toBeCorrect = new();

    private void FixedUpdate()
    {
        if (!GameController.Instance || !GameController.Instance.CurrentPlayer)
        {
            return;
        }

        Tick(Time.fixedDeltaTime);
    }

    private void Tick(float deltaTime)
    {
        // non-penetration constraint (only apply to actors near player)
        _playerActorOld.FindActorCircle(DistanceToPlayer, _queryBuffer);
        _toBeCorrect.Clear();
        foreach (var actor in _queryBuffer)
        {
            if (actor.TryGetComponent(out ActorNonPenetration movement))
            {
                _toBeCorrect.Add(movement);
            }
        }

        // resolve
        foreach (var m1 in _toBeCorrect)
        {
            foreach (var m2 in _toBeCorrect)
            {
                // only resolve once for a pair
                if (m1.GetInstanceID() >= m2.GetInstanceID())
                {
                    continue;
                }

                var los = m1.NextPlacementPosition - m2.NextPlacementPosition;
                var distance = los.magnitude;
                var minDistance = m1.Radius + m2.Radius;
                if (distance < minDistance)
                {
                    var correction = los.normalized * (minDistance - distance) / 2;
                    m1.AddCorrection(correction);
                    m2.AddCorrection(-correction);
                }
            }
        }

        // apply correction
        foreach (var movement in _toBeCorrect)
        {
            movement.Commit();
        }
    }
}