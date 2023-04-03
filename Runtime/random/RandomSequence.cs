using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomSequence<T> where T : class
{
	private readonly List<T> _availableEntries;
	private readonly List<T> _releasedEntries;

	public RandomSequence(IEnumerable<T> elements)
	{
		List<T> deck = elements.ToList();
		deck.Shuffle();
		_availableEntries = deck;
		_releasedEntries = new List<T>();
	}

	public int Count => _availableEntries.Count + _releasedEntries.Count;

	/**
	 * 循环随机化抓牌，保证下一张和上一张不一样
	 */
	public T Draw()
	{
		if (Count == 0)
		{
			Debug.LogWarning("总数为 0，不可能抓到");
			return null;
		}

		if (_availableEntries.Count == 0)
		{
			Debug.Log("抽光了，重新洗牌");
			Shuffle();
		}

		T last = _availableEntries.PopLast();
		return last;
	}

	/**
	 * 给一个条件，返回第一个满足条件的。只在没抓到的牌中找
	 */
	public T FindAndTake(Func<T, bool> criteria)
	{
		if (Count == 0)
		{
			Debug.LogWarning("总数为 0，不可能抓到");
			return null;
		}

		if (_availableEntries.Count == 0)
		{
			Debug.Log("抽光了，重新洗牌");
			Shuffle();
		}

		return _availableEntries.FindAndRemove(criteria).FirstOrDefault();
	}

	/**
	 * 放回卡片
	 */
	public void Discard(T entry)
	{
		_releasedEntries.Add(entry);
	}

	private void Shuffle()
	{
		// 1. 将剩余的牌放到弃牌堆
		_releasedEntries.AddRange(_availableEntries);
		_availableEntries.Clear();

		// 2. 取弃牌堆中，不是最后一个的元素中，随机一个。作为第一个元素
		T firstElement = _releasedEntries[Random.Range(0, _releasedEntries.Count - 1)];

		// 3. 将弃牌堆打乱作为新的牌库，然后将刚刚找出的牌作为第一个
		_availableEntries.AddRange(_releasedEntries
			.Where(el => el != firstElement)
			.OrderBy(_ => Random.value)
			.Append(firstElement));

		// 4. 清理弃牌堆
		_releasedEntries.Clear();
	}

	public IEnumerable<T> GetReleasedEntries()
	{
		return _releasedEntries;
	}

	public void MoveToRelease(Func<T, bool> criteria)
	{
		IEnumerable<T> toBeMove = _availableEntries.FindAndRemove(criteria);
		_releasedEntries.AddRange(toBeMove);
	}

	public void MoveToTop(Func<T, bool> criteria, bool includeReleasedEntries = false, bool shuffle = false)
	{
		List<T> toBeMove = _availableEntries.FindAndRemove(criteria).ToList();
		if (includeReleasedEntries)
		{
			toBeMove.AddRange(_releasedEntries.FindAndRemove(criteria));
		}

		if (shuffle)
		{
			toBeMove.Shuffle();
		}

		_availableEntries.AddRange(toBeMove);
	}
}