using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEntity : MonoBehaviour
{
    public event Action<GameEntity, float> OnTick;
    public event Action<GameEntity> OnSpawn;
    public event Action<GameEntity> OnReady;
    public event Action<GameEntity> OnKill;
    public event Action<GameEntity> OnFinish;

    private EntityState _state = EntityState.Inactive;

    public EntityState CurrentState => _state;

    private void OnDestroy()
    {
        OnTick = null;
        OnSpawn = null;
        OnReady = null;
        OnKill = null;
        OnFinish = null;
    }

    public void Tick(float deltaTime)
    {
        OnTick?.Invoke(this, deltaTime);
    }

    public void SendSpawn()
    {
        if (CurrentState != EntityState.Inactive)
        {
            return;
        }

        _state = EntityState.Spawning;
        OnSpawn?.Invoke(this);
    }

    public void SendReady()
    {
        if (CurrentState != EntityState.Spawning)
        {
            return;
        }

        _state = EntityState.Active;
        OnReady?.Invoke(this);
    }

    public void SendKill()
    {
        if (CurrentState != EntityState.Active)
        {
            return;
        }

        _state = EntityState.Despawning;
        OnKill?.Invoke(this);
    }

    public void SendFinish()
    {
        if (CurrentState != EntityState.Despawning)
        {
            return;
        }

        _state = EntityState.Inactive;
        OnFinish?.Invoke(this);
    }
}