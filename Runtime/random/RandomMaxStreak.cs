using System;
using System.Linq;
using UnityEngine;

public class RandomMaxStreak<T> where T : IEquatable<T>
{
    private readonly BoundedQueue<T> _history;

    public RandomMaxStreak(int maxStreak = 3)
    {
        _history = new BoundedQueue<T>(maxStreak);
    }

    public T Next(Func<T> sampler, int maxReroll = 20)
    {
        int i;
        T result = default;

        // roll
        for (i = 0; i < maxReroll; ++i)
        {
            result = sampler();

            // 数量不足的情况
            if (_history.Count < _history.Capacity)
            {
                break;
            }

            // 有不同的情况
            if (_history.Any(prev => !prev.Equals(result)))
            {
                break;
            }

            Debug.Log($"over streak count({_history.Capacity}), reroll");
        }

        // 超出上限的警告
        if (i >= maxReroll)
        {
            Debug.LogWarning($"over reroll count({maxReroll}). please check sampler");
        }

        // record history
        _history.Enqueue(result);
        return result;
    }

    public void Clear()
    {
        _history.Clear();
    }
}