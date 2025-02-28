using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour, IEntityFrame
{
    public FlowInput<Vector3> PositionProvider;
    public Actor ToBeSpawn;
    public int MaxAmount = 3;
    public float SpawnTime = 1f;

    private float _cooldownTime;
    private readonly List<GameEntity> _spawned = new();

    public void OnEntityFrame(GameEntity entity)
    {
        // count check
        _spawned.RemoveAll(e => !e.IsAlive());
        if (_spawned.Count >= MaxAmount)
        {
            return;
        }

        // produce
        if (_cooldownTime <= 0)
        {
            var position = PositionProvider.Pull();
            var newActor = ToBeSpawn.Spawn(position, Quaternion.identity);
            _spawned.Add(newActor.gameObject.GetEntity());
            _cooldownTime += SpawnTime;
        }

        var bridge = this.FindEntityBridge();
        var deltaTime = bridge.Clock.LocalDeltaTime;
        _cooldownTime -= deltaTime;
    }
}