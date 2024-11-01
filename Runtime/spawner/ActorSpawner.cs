using System.Collections.Generic;
using UnityEngine;

public class ActorSpawner : MonoBehaviour
{
    public GameObject ToBeSpawn;
    public int MaxAmount = 3;
    public float SpawnTime = 1f;

    private float _cooldownTime;
    private readonly List<Actor> spawned = new();

    private void Awake()
    {
        TryGetComponent(out Actor actor);
        actor.OnMove += HandleMove;
    }

    private void HandleMove(Actor actor)
    {
        // cooldown
        if (_cooldownTime > 0)
        {
            _cooldownTime -= actor.Timer.LocalDeltaTime;
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
        Actor spawnedActor = ActorManager.SpawnActor(ToBeSpawn, position, Quaternion.identity);
        spawned.Add(spawnedActor);
        _cooldownTime += SpawnTime;
    }
}