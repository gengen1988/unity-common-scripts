using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public static class ListUtil
{
	public static T PopLast<T>(this IList<T> list)
	{
		T result = list[list.Count - 1];
		list.RemoveAt(list.Count - 1);
		return result;
	}

	public static void RemoveEmpty<T>(this IList<T> list) where T : Object
	{
		foreach (T entry in list.FindAndRemove(entry => !entry))
		{
			// do nothing
		}
	}

	public static IEnumerable<T> WithoutEmpty<T>(this IEnumerable<T> list) where T : Object
	{
		return list.Where(entry => entry);
	}

	public static IEnumerable<T> FindAndRemove<T>(this IList<T> list, Func<T, bool> criteria)
	{
		for (int i = list.Count - 1; i >= 0; --i)
		{
			T entry = list[i];
			if (!criteria(entry))
			{
				continue;
			}

			list.RemoveAt(i);
			yield return entry;
		}
	}

	/**
	 * note that items should contains no duplicated entries
	 */
	public static bool IterateSolve<T>(
		this IList<T> list,
		Func<T, T, bool> criteria,
		Action<IList<T>, int, int> task,
		int maxIterate = 20
	)
	{
		for (int iterate = 0; iterate < maxIterate; ++iterate)
		{
			// build array reverse index
			Dictionary<T, int> indexByItem = new Dictionary<T, int>();
			for (int i = 0; i < list.Count; ++i)
			{
				T item = list[i];
				indexByItem[item] = i;
			}

			// find pairs
			IEnumerable<(T item1, T item2)> tuples = from item1 in list
				from item2 in list
				where indexByItem[item1] < indexByItem[item2]
				where criteria(item1, item2)
				select (item1, item2);

			(T item1, T item2)[] tasks = tuples.ToArray();

			// end condition
			if (tasks.Length == 0)
			{
				Debug.Log($"iterate {iterate} found solution");
				return true;
			}

			// execute tasks
			foreach ((T item1, T item2) tuple in tasks)
			{
				int index1 = indexByItem[tuple.item1];
				int index2 = indexByItem[tuple.item2];
				task(list, index1, index2);
			}
		}

		// no solution
		Debug.LogWarning($"no solution");
		return false;
	}
}