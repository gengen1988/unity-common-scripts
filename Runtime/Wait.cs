using System;

public class Wait
{
    private bool ready;
    private event Action callback;

    public void Ready()
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