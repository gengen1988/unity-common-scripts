using System.Collections.Generic;
using System.Linq;

/**
 * similar with ILookup but can add after construct
 */
public class GroupDictionary<TKey, TValue>
{
    private readonly Dictionary<TKey, HashSet<TValue>> _dic = new();

    public IEnumerable<TKey> Keys => _dic.Keys;
    public IEnumerable<TValue> Values => _dic.Values.SelectMany(valueSet => valueSet);

    public IEnumerable<TValue> this[TKey key]
    {
        get => _dic.GetValueOrDefault(key).EmptyIfNull();
        set => _dic[key] = new HashSet<TValue>(value);
    }

    public void Add(TKey key, TValue value)
    {
        if (!_dic.TryGetValue(key, out HashSet<TValue> valueSet))
        {
            valueSet = new HashSet<TValue>();
            _dic[key] = valueSet;
        }

        valueSet.Add(value);
    }

    public void Remove(TKey key)
    {
        _dic.Remove(key);
    }

    public void Remove(TKey key, TValue value)
    {
        if (!_dic.TryGetValue(key, out HashSet<TValue> valueSet))
        {
            return;
        }

        valueSet.Remove(value);
        if (valueSet.Count == 0)
        {
            _dic.Remove(key);
        }
    }

    public void Clear()
    {
        _dic.Clear();
    }

    public bool Contains(TKey key)
    {
        return _dic.ContainsKey(key);
    }

    public bool Contains(TKey key, TValue value)
    {
        return _dic.TryGetValue(key, out HashSet<TValue> valueSet) && valueSet.Contains(value);
    }
}