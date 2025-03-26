using System;
using System.Collections.Generic;

/**
 * similar to ILookup but can be modified
 */
public class DictionaryList<TKey, TValue>
{
    private readonly Dictionary<TKey, LinkedList<TValue>> _dic = new();
    private readonly List<TKey> _toBeRemove = new();

    public Dictionary<TKey, LinkedList<TValue>>.KeyCollection Keys => _dic.Keys;

    public LinkedList<TValue> this[TKey key]
    {
        get => _dic[key];
        set => _dic[key] = value;
    }

    public void Add(TKey key, TValue value)
    {
        if (!_dic.TryGetValue(key, out var list))
        {
            list = new LinkedList<TValue>();
            _dic[key] = list;
        }

        list.AddLast(value);
    }

    public bool RemoveKey(TKey key)
    {
        return _dic.Remove(key);
    }

    public bool RemoveValue(TKey key, TValue value)
    {
        if (!_dic.TryGetValue(key, out var list))
        {
            return false;
        }

        var removed = list.Remove(value);
        if (list.Count == 0)
        {
            _dic.Remove(key);
        }

        return removed;
    }

    public void RemoveAll(Predicate<TKey> match)
    {
        _toBeRemove.Clear();
        foreach (var key in _dic.Keys)
        {
            if (match(key))
            {
                _toBeRemove.Add(key);
            }
        }

        foreach (var key in _toBeRemove)
        {
            _dic.Remove(key);
        }
    }

    public void Clear()
    {
        _dic.Clear();
        _toBeRemove.Clear();
    }

    public bool ContainsKey(TKey key)
    {
        return _dic.ContainsKey(key);
    }
}