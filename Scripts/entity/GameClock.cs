using System.Collections.Generic;

public class GameClock
{
    private readonly Dictionary<TimeDomain, int> _domainRefCounts = new();
    private readonly Dictionary<TimeDomain, float> _localTimeScaleByDomain = new();

    private float _baseTimeScale = 1f;
    private float _localTimeScale;
    private float _localDeltaTime;
    private float _unscaledDeltaTime;

    public float LocalTimeScale => _localTimeScale;
    public float LocalDeltaTime => _localDeltaTime;
    public float UnscaledDeltaTime => _unscaledDeltaTime;

    public void Reset()
    {
        _baseTimeScale = 1;
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
        if (_domainRefCounts.TryGetValue(domain, out var count))
        {
            _domainRefCounts[domain] = count + 1;
        }
        else
        {
            _domainRefCounts[domain] = 1;
        }
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

                // cleanup local time scale override
                if (_localTimeScaleByDomain.ContainsKey(domain))
                {
                    _localTimeScaleByDomain.Remove(domain);
                }
            }
            else
            {
                _domainRefCounts[domain] = count - 1;
            }
        }
    }

    public void SetBaseTimeScale(float timeScale)
    {
        _baseTimeScale = timeScale;
    }

    public void SetDomainLocalTimeScale(TimeDomain domain, float timeScale)
    {
        _localTimeScaleByDomain[domain] = timeScale;
    }

    public void RemoveDomainLocalTimeScale(TimeDomain domain)
    {
        _localTimeScaleByDomain.Remove(domain);
    }

    private float CalcTimeScale()
    {
        var blendedTimeScale = _baseTimeScale;
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