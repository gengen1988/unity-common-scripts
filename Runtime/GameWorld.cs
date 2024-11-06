using System;
using System.Collections.Generic;

public class GameWorld : Singleton<GameWorld>
{
    public static T GetSystem<T>() where T : class, IGameSystem => Instance._GetSystem<T>();
    public static void RegisterSystem<T>(T system) where T : class, IGameSystem => Instance._AddSystem(system);

    private readonly Dictionary<Type, IGameSystem> _systemByType = new();

    public void Tick(float deltaTime)
    {
        foreach (var system in _systemByType.Values)
        {
            system.Tick(deltaTime);
        }

        EntityManager.Instance.Tick(deltaTime);
        GameEventBus.Instance.Process();
    }

    private T _GetSystem<T>() where T : class, IGameSystem
    {
        var system = _systemByType.GetValueOrDefault(typeof(T));
        return system as T;
    }

    private void _AddSystem<T>(T system) where T : class, IGameSystem
    {
        _systemByType.Add(typeof(T), system);
    }
}