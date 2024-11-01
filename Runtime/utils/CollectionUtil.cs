using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

public static class CollectionUtil
{
    /**
     * use CollectionExtensions.GetValueOrDefault instead
     */
    [Obsolete]
    public static TValue GetWithDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dic,
        TKey key,
        TValue defaultValue = default
    )
    {
        if (dic == null)
        {
            Debug.LogWarning("trying access null dictionary");
            return defaultValue;
        }

        if (key == null)
        {
            Debug.LogWarning("trying access dictionary with null key");
            return defaultValue;
        }

        if (!dic.TryGetValue(key, out TValue value))
        {
            return defaultValue;
        }

        return value;
    }

    public static IEnumerable EmptyIfNull(this IEnumerable collection)
    {
        return collection ?? Enumerable.Empty<object>();
    }

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> collection)
    {
        return collection ?? Enumerable.Empty<T>();
    }

    private static T ReadField<T>(object o, string fieldName)
    {
        Type t = o.GetType();
        FieldInfo f = t.GetField(fieldName);
        return (T)f.GetValue(o);
    }

    public static void ToMap<TKey, TValue>(
        this IEnumerable entries,
        string keyColumn,
        string valueColumn,
        ref Dictionary<TKey, TValue> dic)
    {
        if (dic == null)
        {
            dic = new Dictionary<TKey, TValue>();
        }
        else
        {
            dic.Clear();
        }

        foreach (object entry in entries)
        {
            TKey key = ReadField<TKey>(entry, keyColumn);
            TValue value = ReadField<TValue>(entry, valueColumn);
            if (dic.ContainsKey(key))
            {
                throw new ArgumentException($"duplicated key: {key}");
            }

            dic[key] = value;
        }
    }

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
     * note that list should contains no duplicated entries.
     * return iterate count. -1 means no solution when reached maxIterate
     */
    public static int IterateSolve<T>(
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
                return iterate;
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
        return -1;
    }

    public static void SortBy<T>(this IList<T> collection, Func<T, IComparable> orderBy)
    {
        if (collection == null)
        {
            throw new ArgumentNullException();
        }

        if (collection is List<T> list)
        {
            list.Sort(Comparison);
        }
        else if (collection is T[] array)
        {
            Array.Sort(array, Comparison);
        }
        else
        {
            throw new ArgumentException("only support list or array", nameof(collection));
        }

        return;

        int Comparison(T a, T b)
        {
            IComparable valueA = orderBy(a);
            IComparable valueB = orderBy(b);
            return valueA.CompareTo(valueB);
        }
    }

    public static int FindIndexMinBy<T>(this IList<T> collection, Func<T, IComparable> orderBy)
    {
        if (collection == null || collection.Count == 0)
        {
            return -1;
        }

        int index = 0;
        IComparable value = orderBy(collection[0]);
        for (int i = 1; i < collection.Count; ++i)
        {
            T item = collection[i];
            IComparable newValue = orderBy(item);
            if (newValue.CompareTo(value) < 0)
            {
                index = i;
                value = newValue;
            }
        }

        return index;
    }

    public static int FindIndexMaxBy<T>(this IList<T> collection, Func<T, IComparable> orderBy)
    {
        if (collection == null || collection.Count == 0)
        {
            return -1;
        }

        int index = 0;
        IComparable value = orderBy(collection[0]);
        for (int i = 1; i < collection.Count; ++i)
        {
            T item = collection[i];
            IComparable newValue = orderBy(item);
            if (newValue.CompareTo(value) > 0)
            {
                index = i;
                value = newValue;
            }
        }

        return index;
    }

    public static int RemoveAll<T>(this LinkedList<T> list, Predicate<T> match)
    {
        int count = 0;
        foreach (T e in list)
        {
            if (match(e))
            {
                list.Remove(e);
                count++;
            }
        }

        return count;
    }
}