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
	 * Draw card in the deck to ensure that random sampling does not repeat
	 */
	public T Draw()
	{
		if (Count == 0)
		{
			Debug.LogWarning("Total 0, impossible to draw");
			return null;
		}

		if (_availableEntries.Count == 0)
		{
			Debug.Log("pumped out, reshuffle");
			Shuffle();
		}

		return _availableEntries.PopLast();
	}

	/**
	 * Given a condition, return the first one that satisfies the condition. only found in available cards
	 */
	public T FindAndTake(Func<T, bool> criteria)
	{
		if (Count == 0)
		{
			Debug.LogWarning("Total 0, impossible to draw");
			return null;
		}

		if (_availableEntries.Count == 0)
		{
			Debug.Log("pumped out, reshuffle");
			Shuffle();
		}

		return _availableEntries.FindAndRemove(criteria).FirstOrDefault();
	}

	/**
	 * Put back card 
	 */
	public void Discard(T entry)
	{
		_releasedEntries.Add(entry);
	}

	private void Shuffle()
	{
		// 1. Put the remaining cards on the discard pile
		_releasedEntries.AddRange(_availableEntries);
		_availableEntries.Clear();

		// 2. Take a random one of the elements that are not the last in the discard pile, as the first element
		T firstElement = _releasedEntries[Random.Range(0, _releasedEntries.Count - 1)];

		// 3. Shuffle the discard pile as a new card library, and then use the card just found as the first
		_availableEntries.AddRange(_releasedEntries
			.Where(el => el != firstElement)
			.OrderBy(_ => Random.value)
			.Append(firstElement));

		// 4. clear the discard pile
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