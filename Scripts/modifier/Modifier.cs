using System;
using UnityEngine;
using UnityEngine.Serialization;

// life cycle:
// - on add
// - attach
// - tick
// - on remove
// - detach
public class Modifier : MonoBehaviour
{
    public event Action OnAdd;
    public event Action OnRemove;

    [SerializeField] private CueChannel cueAlive;

    public ModifierProfile Profile { get; set; }
    public ModifierState State { get; set; }
    public float LifeTime { get; set; }
    public float ElapsedTime { get; set; }
    public int StackCount { get; set; }
    public ModifierManager Manager { get; set; }

    private IModifierAttach[] _attachHandlers;
    private IModifierDetach[] _detachHandlers;
    private IModifierFrame[] _frameHandlers;

    private Guid _cueId;

    private void Awake()
    {
        _attachHandlers = GetComponents<IModifierAttach>();
        _detachHandlers = GetComponents<IModifierDetach>();
        _frameHandlers = GetComponents<IModifierFrame>();
    }

    public void Attach(GameEntity entity)
    {
        foreach (var handler in _attachHandlers)
        {
            handler.OnModifierAttach(entity);
        }
    }

    public void Detach(GameEntity entity)
    {
        foreach (var handler in _detachHandlers)
        {
            handler.OnModifierDetach(entity);
        }
    }

    public void Tick(GameEntity entity, float deltaTime)
    {
        foreach (var handler in _frameHandlers)
        {
            handler.OnModifierFrame(entity, deltaTime);
        }
    }

    public void Add()
    {
        ElapsedTime = 0;

        if (StackCount < Profile.StackCapacity)
        {
            StackCount++;
        }

        OnAdd?.Invoke();
        _cueId = cueAlive.PlayIfNotNull(Manager.transform);
    }

    public void Remove()
    {
        OnRemove?.Invoke();
        cueAlive.StopIfNotNull(_cueId);
    }
}

public interface IModifierAttach
{
    public void OnModifierAttach(GameEntity entity);
}

public interface IModifierDetach
{
    public void OnModifierDetach(GameEntity entity);
}

public interface IModifierFrame
{
    public void OnModifierFrame(GameEntity entity, float deltaTime);
}