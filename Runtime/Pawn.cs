using System;
using System.Collections.Generic;
using UnityEngine;

[GameFrameOrder(100)] // clear after other logic
public class Pawn : MonoBehaviour, IEntityAttach
{
    public event Action OnQuery;

    private readonly Dictionary<IntentKey, bool> _boolStore = new();
    private readonly Dictionary<IntentKey, Vector2> _vector2Store = new();

    public void OnEntityAttach(GameEntity entity)
    {
        Clear();
    }

    public bool GetBool(IntentKey key)
    {
        return _boolStore.GetValueOrDefault(key);
    }

    public void SetBool(IntentKey key, bool value)
    {
        _boolStore[key] = value;
    }

    public Vector2 GetVector2(IntentKey key)
    {
        return _vector2Store.GetValueOrDefault(key);
    }

    public void SetVector2(IntentKey key, Vector2 value)
    {
        _vector2Store[key] = value;
    }

    public void Query()
    {
        OnQuery?.Invoke();
    }

    public void Clear()
    {
        _boolStore.Clear();
        _vector2Store.Clear();
    }
}

public enum IntentKey
{
    Move,
    Look,
    Jump,
    Fire,
}