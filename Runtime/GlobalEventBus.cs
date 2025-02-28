using System;

public static class GlobalEventBus
{
    private static readonly GameEventBus EventBus = new();

    public static void Emit<T>(Action<T> initializer = null) where T : GameEvent, new()
    {
        EventBus.Emit(initializer);
    }

    public static void Subscribe<T>(Action<T> listener) where T : GameEvent, new()
    {
        EventBus.Subscribe(listener);
    }

    public static void Unsubscribe<T>(Action<T> listener) where T : GameEvent, new()
    {
        EventBus.Unsubscribe(listener);
    }
}