using System;
using System.Collections.Generic;
using System.Linq;
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

    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> collection)
    {
        return collection ?? Enumerable.Empty<T>();
    }
}