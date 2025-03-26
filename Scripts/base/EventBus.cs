using System.Collections.Generic;

/// <summary>
/// Marker interface for all event types that can be used with the EventBus system
/// </summary>
public interface IEvent
{
}

public class EventBus
{
    private readonly ICollection<IEventBinding> _bindings;

    public EventBus(bool ordered = false)
    {
        if (ordered)
        {
            _bindings = new SortedSet<IEventBinding>();
        }
        else
        {
            _bindings = new HashSet<IEventBinding>();
        }
    }

    /// <summary>
    /// Registers an event binding to receive events of type TEvent
    /// </summary>
    /// <param name="binding">The binding to register</param>
    public void Register(IEventBinding binding)
    {
        _bindings.Add(binding);
    }

    /// <summary>
    /// Removes an event binding from receiving events of type TEvent
    /// </summary>
    /// <param name="binding">The binding to deregister</param>
    public void Deregister(IEventBinding binding)
    {
        _bindings.Remove(binding);
    }

    /// <summary>
    /// Raises an event to all registered bindings
    /// </summary>
    public void Raise()
    {
        foreach (var binding in _bindings)
        {
            binding.OnEvent.Invoke();
        }
    }

    /// <summary>
    /// Clears all bindings from this event bus
    /// Used internally for cleanup during application state changes
    /// </summary>
    public void Clear()
    {
        _bindings.Clear();
    }
}

/// <summary>
/// A type-safe event bus implementation that allows for decoupled communication between components.
/// This generic class handles registration, deregistration, and event raising for a specific event type.
/// </summary>
/// <typeparam name="TEvent">The type of event this bus handles</typeparam>
public class EventBus<TEvent> where TEvent : IEvent
{
    private readonly ICollection<IEventBinding<TEvent>> _bindings;

    public EventBus(bool ordered = false)
    {
        if (ordered)
        {
            _bindings = new SortedSet<IEventBinding<TEvent>>();
        }
        else
        {
            _bindings = new HashSet<IEventBinding<TEvent>>();
        }
    }

    /// <summary>
    /// Registers an event binding to receive events of type TEvent
    /// </summary>
    /// <param name="binding">The binding to register</param>
    public void Register(IEventBinding<TEvent> binding)
    {
        _bindings.Add(binding);
    }

    /// <summary>
    /// Removes an event binding from receiving events of type TEvent
    /// </summary>
    /// <param name="binding">The binding to deregister</param>
    public void Deregister(IEventBinding<TEvent> binding)
    {
        _bindings.Remove(binding);
    }

    /// <summary>
    /// Raises an event to all registered bindings
    /// </summary>
    /// <param name="evt">The event instance to broadcast</param>
    public void Raise(TEvent evt)
    {
        foreach (var binding in _bindings)
        {
            binding.OnEvent.Invoke(evt); // a binding must has delegate
        }
    }

    /// <summary>
    /// Clears all bindings from this event bus
    /// Used internally for cleanup during application state changes
    /// </summary>
    public void Clear()
    {
        _bindings.Clear();
    }
}