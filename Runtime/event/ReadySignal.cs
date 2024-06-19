using System;

public struct ReadySignal
{
    private bool _ready;
    private event Action _callback;

    public void NotifyReady()
    {
        _ready = true;
        _callback?.Invoke();
        _callback = null;
    }

    public void WhenReady(Action action)
    {
        if (_ready)
        {
            action?.Invoke();
        }
        else
        {
            _callback += action;
        }
    }
}

public struct ScheduledSignal
{
    private event Func<bool> _callback;

    public void Dispatch()
    {
        if (_callback == null)
        {
            return;
        }

        foreach (Delegate d in _callback.GetInvocationList())
        {
            Func<bool> handler = (Func<bool>)d;
            bool success = handler();
            if (success)
            {
                _callback -= handler;
            }
        }
    }

    public void Schedule(Func<bool> action)
    {
        _callback += action;
    }
}