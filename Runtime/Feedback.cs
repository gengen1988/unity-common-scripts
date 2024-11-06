using System;
using System.Collections.Generic;
using UnityEngine;

public class Feedback : MonoBehaviour
{
    public event Action<Feedback> OnPlay;
    public event Action<Feedback> OnStop;
    public event Action<Feedback> OnClear;

    [SerializeField] private float MaxActiveTime = -1f;
    [SerializeField] private float MaxDespawningTime = -1f;

    private float _elapsedTime;
    private GameEntity _entity;
    private readonly HashSet<Guid> _blocks = new();

    public bool IsPlaying => _entity.CurrentState == EntityState.Active;

    private void Awake()
    {
        TryGetComponent(out _entity);
        _entity.OnSpawn += HandleSpawn;
        _entity.OnKill += HandleKill;
        _entity.OnTick += HandleTick;
        _entity.OnFinish += HandleFinish;
    }

    private void OnDestroy()
    {
        OnPlay = null;
        OnStop = null;
        OnClear = null;
    }

    private void HandleTick(GameEntity entity, float deltaTime)
    {
        switch (entity.CurrentState)
        {
            case EntityState.Active:
                TickActive(entity, deltaTime);
                break;
            case EntityState.Despawning:
                TickDespawning(entity, deltaTime);
                break;
        }
    }

    private void TickActive(GameEntity entity, float deltaTime)
    {
        // forever
        if (MaxActiveTime < 0)
        {
            return;
        }

        // timeout, graceful stop
        if (_elapsedTime >= MaxActiveTime)
        {
            entity.SendKill();
            return;
        }

        _elapsedTime += deltaTime;
    }

    private void TickDespawning(GameEntity entity, float deltaTime)
    {
        // no longer blocked
        if (_blocks.Count == 0)
        {
            entity.SendFinish();
            return;
        }

        // force clear
        if (MaxDespawningTime < 0 && _elapsedTime >= MaxDespawningTime)
        {
            _blocks.Clear();
            return;
        }

        _elapsedTime += deltaTime;
    }

    private void HandleSpawn(GameEntity entity)
    {
        entity.SendReady(); // instant ready, do not wait for next frame, which means it can be killed right after spawn
        _blocks.Clear();
        _elapsedTime = 0;
        OnPlay?.Invoke(this);
    }

    private void HandleKill(GameEntity entity)
    {
        _elapsedTime = 0;
        if (MaxDespawningTime >= 0)
        {
            _blocks.Add(Guid.NewGuid());
        }

        OnStop?.Invoke(this);
    }

    private void HandleFinish(GameEntity entity)
    {
        OnClear?.Invoke(this);
    }

    public Guid AcquireBlock()
    {
        var blockId = Guid.NewGuid();
        _blocks.Add(blockId);
        return blockId;
    }

    public void ReleaseBlock(Guid blockId)
    {
        _blocks.Remove(blockId);
    }

    public void Stop()
    {
        if (_entity.CurrentState == EntityState.Active)
        {
            _entity.SendKill();
        }
        else
        {
            Debug.LogWarning("Attempting to release an entity that is not active.");
        }
    }

    // static shorthand
    public static Feedback Spawn(Feedback prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!prefab)
        {
            return null;
        }

        var go = EntityManager.Instance.Spawn(prefab.gameObject, position, rotation, parent);
        go.TryGetComponent(out Feedback feedback);
        return feedback;
    }

    public static Feedback Spawn(Feedback prefab, Transform parent = null)
    {
        var position = parent ? parent.position : Vector3.zero;
        var rotation = parent ? parent.rotation : Quaternion.identity;
        return Spawn(prefab, position, rotation, parent);
    }

    public static void Kill(Feedback feedback)
    {
        if (!feedback)
        {
            return;
        }

        EntityManager.Instance.Kill(feedback.gameObject);
    }
}