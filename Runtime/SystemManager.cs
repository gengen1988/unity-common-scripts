using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class GameSystem : MonoBehaviour
{
    private void Start()
    {
        var evt = GameEventBus.AcquireEvent<OnSystemStart>();
        evt.System = this;
        GameEventBus.Publish(evt);
    }
}

public class OnSystemStart : GameEvent
{
    public GameSystem System;
}

[Obsolete]
public class SystemManager : Singleton<SystemManager>, IDisposable
{
    private static class TypedCache<T> where T : GameSystem
    {
        public static T System;
    }

    private readonly Dictionary<Type, GameSystem> _systemByType = new();

    public void Init()
    {
        GameEventBus.Subscribe<OnSystemStart>(HandleSystemStart);
    }

    public void Dispose()
    {
        GameEventBus.Unsubscribe<OnSystemStart>(HandleSystemStart);
    }

    private void HandleSystemStart(OnSystemStart evt)
    {
        _systemByType.Add(evt.System.GetType(), evt.System);
    }

    // public static void RegisterGameObject(GameObject go)
    // {
    //     if (!Instance)
    //     {
    //         Debug.LogError("not ready");
    //         return;
    //     }
    //
    //     Dictionary<Type, GameSystem> index = Instance._systemByType;
    //     MonoBehaviour[] behaviours = go.GetComponents<MonoBehaviour>();
    //     foreach (MonoBehaviour behaviour in behaviours)
    //     {
    //         Type systemType = behaviour.GetType();
    //         if (index.ContainsKey(systemType))
    //         {
    //             Debug.LogError($"{systemType} should not has more than one instance", go);
    //         }
    //         else
    //         {
    //             index[systemType] = behaviour;
    //         }
    //     }
    // }

    public T GetSystem<T>() where T : GameSystem
    {
        // cache to bypass alloc
        var cachedValue = TypedCache<T>.System;
        if (cachedValue)
        {
            return cachedValue;
        }

        // retrieve
        if (!_systemByType.TryGetValue(typeof(T), out var system))
        {
            Debug.LogError($"someone require {typeof(T)} to work");
        }

        // refresh cache
        var newValue = system as T;
        TypedCache<T>.System = newValue;
        return newValue;
    }
}