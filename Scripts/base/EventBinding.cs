using System;

public interface IEventBinding : IComparable<IEventBinding>
{
    public int CallbackOrder { get; }
    public Action OnEvent { get; }
}

public class EventBinding : IEventBinding, IDisposable
{
    public int CallbackOrder { get; }
    public Action OnEvent { get; private set; }

    /// <summary>
    /// Creates a new event binding with the specified callback
    /// </summary>
    /// <param name="callback">The action to invoke when an event is received</param>
    /// <param name="order"></param>
    public EventBinding(Action callback, int order = 0)
    {
        OnEvent = callback;
        CallbackOrder = order;
    }

    /// <summary>
    /// Cleans up this binding when disposed
    /// </summary>
    public void Dispose()
    {
        OnEvent = null;
    }

    public int CompareTo(IEventBinding other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (other is null) return 1;

        // First compare by order
        var orderComparison = CallbackOrder.CompareTo(other.CallbackOrder);
        if (orderComparison != 0)
        {
            return orderComparison;
        }

        // If same order, use GetHashCode to ensure consistent ordering
        return GetHashCode().CompareTo(other.GetHashCode());
    }
}

public interface IEventBinding<TEvent> : IComparable<IEventBinding<TEvent>>
{
    public int CallbackOrder { get; }
    public Action<TEvent> OnEvent { get; }
}

/// <summary>
/// A disposable event binding implementation that can be used to subscribe to events
/// and automatically clean up when no longer needed
/// </summary>
/// <typeparam name="TEvent">The type of event this binding handles</typeparam>
public class EventBinding<TEvent> : IEventBinding<TEvent>, IDisposable where TEvent : IEvent
{
    public int CallbackOrder { get; }
    public Action<TEvent> OnEvent { get; private set; }

    /// <summary>
    /// Creates a new event binding with the specified callback
    /// </summary>
    /// <param name="callback">The action to invoke when an event is received</param>
    /// <param name="order">less is first</param>
    public EventBinding(Action<TEvent> callback, int order = 0)
    {
        OnEvent = callback;
        CallbackOrder = order;
    }

    /// <summary>
    /// Cleans up this binding when disposed
    /// </summary>
    public void Dispose()
    {
        OnEvent = null;
    }

    public int CompareTo(IEventBinding<TEvent> other)
    {
        if (ReferenceEquals(this, other))
        {
            return 0;
        }

        if (other is null) return 1;

        // First compare by order
        var orderComparison = CallbackOrder.CompareTo(other.CallbackOrder);
        if (orderComparison != 0)
        {
            return orderComparison;
        }

        // If same order, use GetHashCode to ensure consistent ordering
        return GetHashCode().CompareTo(other.GetHashCode());
    }
}