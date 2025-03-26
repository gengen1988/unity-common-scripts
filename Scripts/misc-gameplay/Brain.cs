using System.Collections.Generic;
using UnityEngine;

public abstract class BrainManager<T> : MonoBehaviour, IBrainManager<T>
{
    private readonly List<IBrain<T>> _brains = new();

    public void AddBrain(IBrain<T> brain)
    {
        _brains.Add(brain);
    }

    public void RemoveBrain(IBrain<T> brain)
    {
        _brains.Remove(brain);
    }
}

public abstract class Brain<T> : MonoBehaviour, IBrain<T>
{
    // self modules
    private IBrainObserve<T> _observeStrategy;
    private IBrainOrient<T> _orientStrategy;
    private IBrainDecide<T> _decideStrategy;

    // parent module
    private IBrainManager<T> _manager;

    private void Awake()
    {
        TryGetComponent(out _observeStrategy);
        TryGetComponent(out _orientStrategy);
        TryGetComponent(out _decideStrategy);
        _manager = GetComponentInParent<IBrainManager<T>>();
    }

    private void OnEnable()
    {
        _manager.AddBrain(this);
    }

    private void OnDisable()
    {
        _manager.RemoveBrain(this);
    }

    public void Refresh(T context)
    {
        _observeStrategy?.Observe(context);
        _orientStrategy?.Orient(context);
        _decideStrategy?.Decide(context);
    }
}