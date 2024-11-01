using System;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public partial class ActorManager : SingletonBehaviour<ActorManager>
{
    private enum ActorState
    {
        Birth,
        Normal,
        Dead,
    }

    private class ManagedEntry
    {
        public Actor Actor;
        public ActorState State;
        public float ElapsedTime;
    }

    private readonly List<ManagedEntry> _toBeTick = new(); // event scheduler
    private readonly List<ManagedEntry> _toBeBorn = new(); // event scheduler
    private readonly List<ManagedEntry> _toBeDie = new(); // event scheduler
    private readonly Dictionary<Actor, ManagedEntry> _entryByActor = new(); // index that covers entire actor lifespan

    private void Start()
    {
        // manual tick behavior trees
        // Debug.Assert(BehaviorManager.instance.UpdateInterval == UpdateIntervalType.Manual);

        // auto register actors in scene
        Actor[] actorsAlreadyInScene = FindObjectsByType<Actor>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Actor actor in actorsAlreadyInScene)
        {
            ManagedEntry entry = new ManagedEntry
            {
                Actor = actor,
                State = ActorState.Normal
            };
            _entryByActor[actor] = entry;
            _toBeTick.Add(entry);
        }
    }

    private void FixedUpdate()
    {
        Tick(Time.fixedDeltaTime);

        // Physics.Simulate by unity here
        // it triggers OnCollisionEnter ...
        // these physics callbacks will be handled in next tick
    }


    private void Tick(float deltaTime)
    {
        // die handling
        for (int i = _toBeDie.Count - 1; i >= 0; i--)
        {
            ManagedEntry entry = _toBeDie[i];

            // trigger die event
            if (entry.ElapsedTime <= 0)
            {
                // entry.Actor.Die();
            }

            // recycle
            if (entry.ElapsedTime >= entry.Actor.DieTime)
            {
                _toBeDie.RemoveAt(i);
                _entryByActor.Remove(entry.Actor);
                PoolUtil.Despawn(entry.Actor.gameObject);
            }

            entry.ElapsedTime += deltaTime;
        }

        // cleanup
        _toBeTick.RemoveAll(entry => entry.State == ActorState.Dead);

        // actor logic
        foreach (var entry in _toBeTick)
        {
            // entry.Actor.Tick(deltaTime);
        }

        // collisions between previous frame to this frame
        foreach (ManagedEntry entry in _toBeTick)
        {
            // entry.Actor.HitManager.Tick(deltaTime);
        }

        // born (newly actors start tick at next frame)
        for (int i = _toBeBorn.Count - 1; i >= 0; i--)
        {
            ManagedEntry entry = _toBeBorn[i];

            // trigger born event
            if (entry.ElapsedTime <= 0)
            {
                StagingSystem.Commit(entry.Actor.gameObject); // event listen happened at this timing
                // entry.Actor.Born();
            }

            // state transition
            if (entry.ElapsedTime >= entry.Actor.BornTime)
            {
                _toBeBorn.RemoveAt(i);
                _toBeTick.Add(entry);
                entry.State = ActorState.Normal;
                // entry.Actor.Ready();
            }

            entry.ElapsedTime += deltaTime;
        }
    }

    private Actor Spawn(GameObject actorPrefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject actorObject = StagingSystem.Prepare(
            t => PoolUtil.Spawn(actorPrefab, position, rotation, t),
            parent
        );
        actorObject.TryGetComponent(out Actor actor);
        ManagedEntry entry = new ManagedEntry
        {
            Actor = actor,
            State = ActorState.Birth
        };
        _entryByActor[entry.Actor] = entry;
        _toBeBorn.Add(entry);
        return actor;
    }

    private void Despawn(Actor actor)
    {
        if (!actor)
        {
            Debug.LogError("despawn null actor");
            return;
        }

        if (!Instance._entryByActor.TryGetValue(actor, out ManagedEntry entry))
        {
            Debug.LogWarning("actor not found", actor);
            return;
        }

        if (entry.State == ActorState.Dead)
        {
            Debug.LogWarning("actor is already dead", actor);
            return;
        }

        entry.State = ActorState.Dead;
        entry.ElapsedTime = 0;
        _toBeDie.Add(entry);
    }
}

// shortcuts
public partial class ActorManager
{
    public static Actor SpawnActor(
        GameObject actorPrefab,
        Vector3 position,
        Quaternion rotation,
        Transform parent = null
    )
    {
        return Instance.Spawn(actorPrefab, position, rotation, parent);
    }

    public static void DespawnActor(Actor actor)
    {
        Instance.Despawn(actor);
    }

    public static bool IsExists(Actor actor)
    {
        if (!actor)
        {
            return false;
        }

        return Instance._entryByActor.ContainsKey(actor);
    }

    public static bool IsAlive(Actor actor)
    {
        if (!actor)
        {
            return false;
        }

        if (!Instance._entryByActor.TryGetValue(actor, out ManagedEntry entry))
        {
            return false;
        }

        return entry.State == ActorState.Normal;
    }
}