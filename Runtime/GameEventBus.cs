using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameEventBus : Singleton<GameEventBus>
{
    private static int MAX_POOL_SIZE = 1000;

    private static class TypedCache<T> where T : GameEvent
    {
        public static ObjectPool<T> EventPool;
    }

    private readonly Dictionary<Type, LinkedList<Delegate>> _listenersByType = new();
    private readonly Queue<GameEvent> _eventQueue = new();

    public static void Publish<T>(T gameEvent = null) where T : GameEvent, new() => Instance._Publish(gameEvent);
    public static void Subscribe<T>(Action<T> callback) where T : GameEvent => Instance._Subscribe(callback);
    public static void Unsubscribe<T>(Action<T> callback) where T : GameEvent => Instance._Unsubscribe(callback);

    public void Process()
    {
        var queue = Instance._eventQueue;
        while (queue.Count > 0)
        {
            var evt = queue.Dequeue();
            try
            {
                ProcessEvent(evt);
            }
            finally
            {
                ReleaseEvent(evt);
            }
        }
    }

    // 发送事件（自动管理事件生命周期）
    private void _Publish<T>(T gameEvent = null) where T : GameEvent, new()
    {
        gameEvent ??= AcquireEvent<T>();
        _eventQueue.Enqueue(gameEvent);
    }

    private void _Subscribe<T>(Action<T> callback) where T : GameEvent
    {
        var type = typeof(T);
        if (!_listenersByType.TryGetValue(type, out var list))
        {
            list = new LinkedList<Delegate>();
            _listenersByType.Add(type, list);
        }

        list.AddLast(callback);
    }

    private void _Unsubscribe<T>(Action<T> callback) where T : GameEvent
    {
        var type = typeof(T);
        _listenersByType[type].Remove(callback);
    }

    private void ProcessEvent(GameEvent gameEvent)
    {
        var type = gameEvent.GetType();
        if (!_listenersByType.TryGetValue(type, out var listeners))
        {
            Debug.LogWarning($"{type.Name} has not been registered yet.");
            return;
        }

        foreach (var listener in listeners)
        {
            listener.DynamicInvoke(gameEvent);
        }
    }

    // 清理所有事件和监听器
    public void Clear()
    {
        _eventQueue.Clear();
        _listenersByType.Clear();
    }

    // 事件池工厂方法
    private static ObjectPool<T> GetOrCreatePool<T>() where T : GameEvent, new()
    {
        if (TypedCache<T>.EventPool == null)
        {
            TypedCache<T>.EventPool = new ObjectPool<T>(
                createFunc: () => new T(), // this is why we can not use ObjectPool<GameEvent>
                actionOnGet: evt => evt.OnReset(),
                maxSize: MAX_POOL_SIZE
            );
        }

        return TypedCache<T>.EventPool;
    }

    public static T AcquireEvent<T>() where T : GameEvent, new()
    {
        var pool = GetOrCreatePool<T>();
        return pool.Get();
    }

    public static void ReleaseEvent<T>(T gameEvent) where T : GameEvent, new()
    {
        var pool = GetOrCreatePool<T>();
        pool.Release(gameEvent);
    }
}

public class GameEvent
{
    public virtual void OnReset()
    {
    }
}