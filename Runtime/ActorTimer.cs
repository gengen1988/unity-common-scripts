using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActorTimer
{
    private class TimeMultiplier
    {
        public string Key;
        public float Value;
        public float LifeTime;
    }

    private float _localTimeScale;
    private float _localDeltaTime, _deltaTime;
    private Dictionary<string, TimeMultiplier> _multiplierByKey = new();

    public float LocalDeltaTime => _localDeltaTime;
    public float DeltaTime => _deltaTime;

    public void Perceive(float deltaTime)
    {
        _localTimeScale = CalcTimeScale();
        _deltaTime = deltaTime;
        _localDeltaTime = deltaTime * _localTimeScale;
    }

    public void ChangeTimeScale(string key, float value, float duration)
    {
        _multiplierByKey[key] = new TimeMultiplier
        {
            Key = key,
            Value = value,
            LifeTime = duration,
        };
    }

    private float CalcTimeScale()
    {
        // cleanup
        foreach (TimeMultiplier entry in _multiplierByKey.Values)
        {
            entry.LifeTime -= Time.deltaTime;
        }

        _multiplierByKey = _multiplierByKey.Where(pair => pair.Value.LifeTime > 0)
            .ToDictionary(pair => pair.Key, pair => pair.Value);

        // calc scale
        float scale = 1f;
        foreach (TimeMultiplier entry in _multiplierByKey.Values)
        {
            scale *= entry.Value;
        }

        return scale;
    }

    public void Cleanup()
    {
        _multiplierByKey.Clear();
    }

    public void AddMultiplier(string key, float value)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveMultiplier(string key)
    {
        throw new System.NotImplementedException();
    }
}