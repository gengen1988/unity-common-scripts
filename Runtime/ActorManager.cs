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
        public ActorOld ActorOld;
        public ActorState State;
        public float ElapsedTime;
    }

    private readonly List<ManagedEntry> _toBeTick = new(); // event scheduler
    private readonly List<ManagedEntry> _toBeBorn = new(); // event scheduler
    private readonly List<ManagedEntry> _toBeDie = new(); // event scheduler
    private readonly Dictionary<ActorOld, ManagedEntry> _entryByActor = new(); // index that covers entire actor lifespan

    private void Start()
    {
        // manual tick behavior trees
        // Debug.Assert(BehaviorManager.instance.UpdateInterval == UpdateIntervalType.Manual);

        // auto register actors in scene
        ActorOld[] actorsAlreadyInScene = FindObjectsByType<ActorOld>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (ActorOld actor in actorsAlreadyInScene)
        {
            ManagedEntry entry = new ManagedEntry
            {
                ActorOld = actor,
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
            if (entry.ElapsedTime >= entry.ActorOld.DieTime)
            {
                _toBeDie.RemoveAt(i);
                _entryByActor.Remove(entry.ActorOld);
                PoolUtil.Despawn(entry.ActorOld.gameObject);
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
                StagingSystem.Commit(entry.ActorOld.gameObject); // event listen happened at this timing
                // entry.Actor.Born();
            }

            // state transition
            if (entry.ElapsedTime >= entry.ActorOld.BornTime)
            {
                _toBeBorn.RemoveAt(i);
                _toBeTick.Add(entry);
                entry.State = ActorState.Normal;
                // entry.Actor.Ready();
            }

            entry.ElapsedTime += deltaTime;
        }
    }

    private ActorOld Spawn(GameObject actorPrefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject actorObject = StagingSystem.Prepare(
            t => PoolUtil.Spawn(actorPrefab, position, rotation, t),
            parent
        );
        actorObject.TryGetComponent(out ActorOld actor);
        ManagedEntry entry = new ManagedEntry
        {
            ActorOld = actor,
            State = ActorState.Birth
        };
        _entryByActor[entry.ActorOld] = entry;
        _toBeBorn.Add(entry);
        return actor;
    }

    private void Despawn(ActorOld actorOld)
    {
        if (!actorOld)
        {
            Debug.LogError("despawn null actor");
            return;
        }

        if (!Instance._entryByActor.TryGetValue(actorOld, out ManagedEntry entry))
        {
            Debug.LogWarning("actor not found", actorOld);
            return;
        }

        if (entry.State == ActorState.Dead)
        {
            Debug.LogWarning("actor is already dead", actorOld);
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
    public static ActorOld SpawnActor(
        GameObject actorPrefab,
        Vector3 position,
        Quaternion rotation,
        Transform parent = null
    )
    {
        return Instance.Spawn(actorPrefab, position, rotation, parent);
    }

    public static void DespawnActor(ActorOld actorOld)
    {
        Instance.Despawn(actorOld);
    }

    public static bool IsExists(ActorOld actorOld)
    {
        if (!actorOld)
        {
            return false;
        }

        return Instance._entryByActor.ContainsKey(actorOld);
    }

    public static bool IsAlive(ActorOld actorOld)
    {
        if (!actorOld)
        {
            return false;
        }

        if (!Instance._entryByActor.TryGetValue(actorOld, out ManagedEntry entry))
        {
            return false;
        }

        return entry.State == ActorState.Normal;
    }
}