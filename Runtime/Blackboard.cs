using System;
using System.Collections.Generic;
using System.Reflection;

public class Blackboard<TKey> where TKey : struct, Enum
{
    private readonly Dictionary<TKey, object> _store = new();

    public TValue GetValue<TValue>(TKey key)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        TypeCheck<TValue>(key);
#endif
        return (TValue)_store[key];
    }

    public TValue GetValueOrDefault<TValue>(TKey key, TValue defaultValue = default)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        TypeCheck<TValue>(key);
#endif
        return (TValue)_store.GetValueOrDefault(key, defaultValue);
    }

    public void SetValue<TValue>(TKey key, TValue value)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        TypeCheck<TValue>(key);
#endif
        _store[key] = value;
    }

    public bool Contains(TKey key)
    {
        return _store.ContainsKey(key);
    }

    public void Remove(TKey key)
    {
        _store.Remove(key);
    }

    private static void TypeCheck<TValue>(TKey key)
    {
        Type keyType = typeof(TKey);
        string enumName = Enum.GetName(keyType, key);
        FieldInfo keyField = keyType.GetField(enumName);
        FieldValueAttribute fieldValueAttribute = keyField.GetCustomAttribute<FieldValueAttribute>();
        if (fieldValueAttribute == null)
        {
            return;
        }

        Type expectedType = fieldValueAttribute.GetValueType();
        Type valueType = typeof(TValue);
        if (!expectedType.IsAssignableFrom(valueType))
        {
            throw new InvalidOperationException(
                $"The value type '{valueType}' does not match the expected type '{expectedType}'.");
        }
    }
}