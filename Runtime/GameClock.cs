using System.Collections.Generic;

public class GameClock
{
    private readonly Dictionary<TimeDomain, int> _domainRefCounts = new();
    private readonly Dictionary<TimeDomain, float> _localTimeScaleByDomain = new();

    private float _localTimeScale;
    private float _localDeltaTime;
    private float _unscaledDeltaTime;

    public float LocalTimeScale => _localTimeScale;
    public float LocalDeltaTime => _localDeltaTime;
    public float UnscaledDeltaTime => _unscaledDeltaTime;

    public void Reset()
    {
        _domainRefCounts.Clear();
        _localTimeScaleByDomain.Clear();
    }

    public void Tick(float deltaTime)
    {
        _unscaledDeltaTime = deltaTime;
        _localTimeScale = CalcTimeScale();
        _localDeltaTime = deltaTime * _localTimeScale;
    }

    public void EnterDomain(TimeDomain domain)
    {
        if (!domain)
        {
            return;
        }

        // increment reference count
        if (_domainRefCounts.TryGetValue(domain, out int count))
        {
            _domainRefCounts[domain] = count + 1;
        }
        else
        {
            _domainRefCounts[domain] = 1;
        }
    }

    public void EnterDomain(TimeDomain domain, float localTimeScale)
    {
        if (!domain)
        {
            return;
        }

        EnterDomain(domain);
        _localTimeScaleByDomain[domain] = localTimeScale;
    }

    public void LeaveDomain(TimeDomain domain)
    {
        if (!domain)
        {
            return;
        }

        // decrement reference count
        if (_domainRefCounts.TryGetValue(domain, out var count))
        {
            if (count <= 1)
            {
                // remove when last reference is gone
                _domainRefCounts.Remove(domain);
                _localTimeScaleByDomain.Remove(domain);
            }
            else
            {
                _domainRefCounts[domain] = count - 1;
            }
        }
    }

    private float CalcTimeScale()
    {
        var blendedTimeScale = 1f;
        foreach (var domain in _domainRefCounts.Keys)
        {
            if (_localTimeScaleByDomain.TryGetValue(domain, out var localTimeScale))
            {
                blendedTimeScale *= localTimeScale;
            }
            else
            {
                blendedTimeScale *= domain.GlobalTimeScale;
            }
        }

        return blendedTimeScale;
    }
}