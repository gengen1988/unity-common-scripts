using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ListUtil
{
    public static T PopLast<T>(this List<T> list)
    {
        int index = list.Count - 1;
        T result = list[index];
        list.RemoveAt(index);
        return result;
    }

    public static bool TryFindAndTake<T>(this List<T> list, Func<T, bool> criteria, out T found)
    {
        for (int i = list.Count - 1; i >= 0; --i)
        {
            T entry = list[i];
            if (!criteria(entry))
            {
                continue;
            }

            list.RemoveAt(i);
            found = entry;
            return true;
        }

        found = default;
        return false;
    }

    public static IEnumerable<T> FindAndTakeAll<T>(this List<T> list, Func<T, bool> criteria)
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
     * note that list should contains no duplicated entries
     */
    public static bool IterateSolve<T>(
        this IList<T> list, // do not add or remove element when use array
        Func<T, T, bool> criteria,
        Action<IList<T>, int, int> task, // this is for value type (struct)
        int maxIterate = 20
    )
    {
        for (int iterate = 0; iterate < maxIterate; ++iterate)
        {
            // build list reverse index
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
            foreach ((T item1, T item2) in tasks)
            {
                int index1 = indexByItem[item1];
                int index2 = indexByItem[item2];
                task(list, index1, index2);
            }
        }

        // no solution
        Debug.LogWarning($"no solution");
        return false;
    }
}