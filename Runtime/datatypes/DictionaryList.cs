using System.Collections.Generic;
using System.Linq;

/**
 * similar with ILookup but can add after construct
 */
public class DictionaryList<TKey, TValue>
{
    private readonly Dictionary<TKey, List<TValue>> _dic = new();

    public IEnumerable<TKey> Keys => _dic.Keys;
    public IEnumerable<TValue> Values => _dic.Values.SelectMany(valueSet => valueSet);

    public List<TValue> this[TKey key]
    {
        get => _dic[key];
        set => _dic[key] = value;
    }

    public void Add(TKey key, TValue value)
    {
        if (!_dic.TryGetValue(key, out List<TValue> valueSet))
        {
            valueSet = new List<TValue>();
            _dic[key] = valueSet;
        }

        valueSet.Add(value);
    }

    public void Remove(TKey key)
    {
        _dic.Remove(key, out _);
    }

    public void Remove(TKey key, TValue value)
    {
        if (!_dic.TryGetValue(key, out List<TValue> valueSet))
        {
            return;
        }

        valueSet.Remove(value);
        if (valueSet.Count == 0)
        {
            _dic.Remove(key, out _);
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
        return _dic.TryGetValue(key, out List<TValue> valueSet) && valueSet.Contains(value);
    }
}