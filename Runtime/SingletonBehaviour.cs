﻿using UnityEngine;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
{
    protected static T _instance;

    public static T Instance => _instance;

    private void Awake()
    {
        Debug.Assert(_instance == null || _instance == this, $"{typeof(T)} has more than one instances", this);
        _instance = this as T;
        AfterAwake();
    }

    protected virtual void AfterAwake()
    {
    }

    public void EnsureSingleton()
    {
        if (!_instance)
        {
            _instance = this as T;
        }
    }
}