using System.Collections.Generic;
using UnityEngine;

public class ActorSpawner : MonoBehaviour
{
    public GameObject ToBeSpawn;
    public int MaxAmount = 3;
    public float SpawnTime = 1f;

    private float _cooldownTime;
    private readonly List<ActorOld> spawned = new();

    private void Awake()
    {
        TryGetComponent(out ActorOld actor);
        actor.OnMove += HandleMove;
    }

    private void HandleMove(ActorOld actorOld)
    {
        // cooldown
        if (_cooldownTime > 0)
        {
            _cooldownTime -= actorOld.Timer.LocalDeltaTime;
            return;
        }

        // count check
        spawned.RemoveAll(managed => !ActorManager.IsExists(managed));
        if (spawned.Count >= MaxAmount)
        {
            return;
        }

        // produce
        Vector3 position = transform.position;
        ActorOld spawnedActorOld = ActorManager.SpawnActor(ToBeSpawn, position, Quaternion.identity);
        spawned.Add(spawnedActorOld);
        _cooldownTime += SpawnTime;
    }
}