using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;

public class StageDirector : MonoBehaviour
{
    [SerializeField] private Spawner2 playerSpawner;
    [SerializeField] private Spawner2 enemySpawner;
    [SerializeField] private CinemachineTargetGroup cameraTargetGroup;
    [SerializeField] private Transform mouseCursorTransform;

    private void Start()
    {
        StageProcedure().Forget();
    }

    private async UniTask StageProcedure()
    {
        // spawn player
        var player = playerSpawner.SpawnUnmanaged();

        // setup enemy spawner
        enemySpawner.TryGetComponent(out PositionConstraint posConstraint);
        Debug.Assert(posConstraint.sourceCount == 0);
        posConstraint.AddSource(new ConstraintSource
        {
            sourceTransform = player.transform,
        });
        posConstraint.constraintActive = true;

        // setup camera follow
        Debug.Assert(cameraTargetGroup.IsEmpty);
        player.TryGetComponent(out Collider2D playerCollider);
        var playerBounds = playerCollider.bounds;
        var radius = Mathf.Max(playerBounds.extents.x, playerBounds.extents.y);
        cameraTargetGroup.AddMember(player.transform, 1, radius);
        cameraTargetGroup.AddMember(mouseCursorTransform, 1, 1);
    }
}