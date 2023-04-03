using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomMaxStreak<T> where T : IEquatable<T>
{
	private readonly int _maxStreak;
	private readonly int _maxReroll;
	private readonly Queue<T> _history = new Queue<T>();

	public RandomMaxStreak(int maxStreak = 3, int maxReroll = 20)
	{
		_maxStreak = maxStreak;
		_maxReroll = maxReroll;
	}

	public T Next(Func<T> sampler)
	{
		T result;
		int rerollCount = 0;

		do
		{
			if (rerollCount > 0)
			{
				Debug.Log($"over streak count({_maxStreak}), reroll");
			}

			result = sampler();
		} while (_history.Count >= _maxStreak
		         && _history.All(prev => prev.Equals(result))
		         && rerollCount++ < _maxReroll);

		if (rerollCount >= _maxReroll)
		{
			Debug.LogWarning($"over reroll count({_maxReroll}). you may check the generator");
		}

		_history.Enqueue(result);
		while (_history.Count > _maxStreak)
		{
			_history.Dequeue();
		}

		return result;
	}

	public void Reset()
	{
		_history.Clear();
	}
}