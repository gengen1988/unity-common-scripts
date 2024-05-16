using System;

public struct ReadySignal
{
    private bool ready;
    private event Action callback;

    public void NotifyReady()
    {
        ready = true;
        callback?.Invoke();
        callback = null;
    }

    public void WhenReady(Action action)
    {
        if (ready)
        {
            action?.Invoke();
        }
        else
        {
            callback += action;
        }
    }
}