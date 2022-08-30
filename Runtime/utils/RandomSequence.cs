using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class RandomSequence<T> where T : class
{
	private readonly List<T> _availableElements;
	private readonly List<T> _releasedElements;

	public RandomSequence(IEnumerable<T> elements)
	{
		var deck = elements.ToList();
		deck.Shuffle();
		_availableElements = deck;
		_releasedElements = new List<T>();
	}

	public int Count => _availableElements.Count + _releasedElements.Count;

	/**
	 * 循环随机化抓牌，保证下一张和上一张不一样
	 */
	public T Acquire()
	{
		if (_availableElements.Count == 0)
		{
			Shuffle();
		}

		var last = _availableElements.PopLast();
		return last;
	}

	/**
	 * 给一个条件，返回满足条件的。
	 */
	public T Acquire(Func<T, bool> condition)
	{
		if (_availableElements.Count == 0)
		{
			Shuffle();
		}

		for (int i = _availableElements.Count - 1; i >= 0; i--)
		{
			if (condition(_availableElements[i]))
			{
				var result = _availableElements[i];
				_availableElements.RemoveAt(i);
				return result;
			}
		}

		return null;
	}

	/**
	 * 卡片放回
	 */
	public void Release(T element)
	{
		_releasedElements.Add(element);
	}

	public bool Contains(T element)
	{
		return _availableElements.Contains(element) || _releasedElements.Contains(element);
	}

	public void Shuffle()
	{
		// 1. 将剩余的牌放到弃牌堆
		_releasedElements.AddRange(_availableElements);
		_availableElements.Clear();

		// 2. 取弃牌堆中，不是最后一个的元素中，随机一个。作为第一个元素
		var firstElement = _releasedElements[Random.Range(0, _releasedElements.Count - 1)];

		// 3. 将弃牌堆打乱作为新的牌库，然后将刚刚找出的牌作为第一个
		_availableElements.AddRange(_releasedElements
			.Where(el => el != firstElement)
			.OrderBy(_ => Random.value)
			.Append(firstElement));

		// 4. 清理弃牌堆
		_releasedElements.Clear();
	}
}