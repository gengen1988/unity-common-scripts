using System;
using System.Collections.Generic;

/**
 * similar to ILookup but can add after construct
 */
public class DictionaryList<TKey, TValue>
{
    private readonly Dictionary<TKey, LinkedList<TValue>> _dic = new();
    private readonly List<TKey> _toBeRemove = new();

    public Dictionary<TKey, LinkedList<TValue>>.KeyCollection Keys => _dic.Keys;
    public Dictionary<TKey, LinkedList<TValue>>.ValueCollection Values => _dic.Values;

    public LinkedList<TValue> this[TKey key]
    {
        get => _dic[key];
        set => _dic[key] = value;
    }

    public void Add(TKey key, TValue value)
    {
        if (!_dic.TryGetValue(key, out LinkedList<TValue> list))
        {
            list = new LinkedList<TValue>();
            _dic[key] = list;
        }

        list.AddLast(value);
    }

    public bool Remove(TKey key)
    {
        return _dic.Remove(key);
    }

    public bool Remove(TKey key, TValue value)
    {
        if (!_dic.TryGetValue(key, out LinkedList<TValue> list))
        {
            return false;
        }

        bool removed = list.Remove(value);
        if (list.Count == 0)
        {
            _dic.Remove(key);
        }

        return removed;
    }

    public void RemoveAll(Predicate<TKey> match)
    {
        _toBeRemove.Clear();
        foreach (TKey key in _dic.Keys)
        {
            if (match(key))
            {
                _toBeRemove.Add(key);
            }
        }

        foreach (TKey key in _toBeRemove)
        {
            _dic.Remove(key);
        }
    }

    public void Clear()
    {
        _dic.Clear();
    }

    public bool ContainsKey(TKey key)
    {
        return _dic.ContainsKey(key);
    }

    public bool ContainsValue(TKey key, TValue value)
    {
        return _dic.TryGetValue(key, out LinkedList<TValue> valueSet) && valueSet.Contains(value);
    }
}