using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameEventBus
{
    private static int MAX_POOL_SIZE = 1000;

    private static class TypedCache<T> where T : GameEvent
    {
        public static ObjectPool<T> EventPool;
    }

    private readonly Dictionary<Type, Action<GameEvent>> _listenerByType = new();
    private readonly Dictionary<Delegate, Action<GameEvent>> _wrappedListeners = new();

    public void Emit<T>(Action<T> initializer = null) where T : GameEvent, new()
    {
        // early return
        var type = typeof(T);
        var listeners = _listenerByType.GetValueOrDefault(type);
        if (listeners == null)
        {
            return;
        }

        var gameEvent = AcquireEvent<T>();
        initializer?.Invoke(gameEvent);
        listeners.Invoke(gameEvent);
        ReleaseEvent(gameEvent);
    }

    public void Subscribe<T>(Action<T> listener) where T : GameEvent
    {
        var type = typeof(T);
        Action<GameEvent> wrappedListener = gameEvent => listener(gameEvent as T);
        if (!_listenerByType.TryAdd(type, wrappedListener))
        {
            _listenerByType[type] += wrappedListener;
        }

        // Store the wrapped listener for later unsubscription
        if (!_wrappedListeners.TryAdd(listener, wrappedListener))
        {
            _wrappedListeners[listener] += wrappedListener;
        }
    }

    public void Unsubscribe<T>(Action<T> listener) where T : GameEvent
    {
        if (!_wrappedListeners.TryGetValue(listener, out var wrappedListener))
        {
            return;
        }

        _listenerByType[typeof(T)] -= wrappedListener;
        _wrappedListeners.Remove(listener);
    }

    private static ObjectPool<T> GetOrCreatePool<T>() where T : GameEvent, new()
    {
        if (TypedCache<T>.EventPool == null)
        {
            TypedCache<T>.EventPool = new ObjectPool<T>(
                createFunc: () => new T(), // this is why we can not use ObjectPool<GameEvent>
                maxSize: MAX_POOL_SIZE
            );
        }

        return TypedCache<T>.EventPool;
    }

    private static T AcquireEvent<T>() where T : GameEvent, new()
    {
        var pool = GetOrCreatePool<T>();
        return pool.Get();
    }

    private static void ReleaseEvent<T>(T gameEvent) where T : GameEvent, new()
    {
        var pool = GetOrCreatePool<T>();
        pool.Release(gameEvent);
    }
}

public class GameEvent
{
}