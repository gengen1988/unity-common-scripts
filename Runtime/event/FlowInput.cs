using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class FlowInput<T>
{
    [SerializeField] private bool IsOverride;
    [SerializeField] private T Value;
    [SerializeField] private Object SourceObject;
    [SerializeField] private ExtractType ExtractType;
    [SerializeField] private string MemberName;

    public T Pull()
    {
        if (IsOverride)
        {
            return Value;
        }

        // get raw value
        if (!SourceObject)
        {
            Debug.LogError("no output object");
            return default;
        }

        var value = ExtractType switch
        {
            ExtractType.Field => ExtractFromField(SourceObject, MemberName),
            ExtractType.Property => ExtractFromProperty(SourceObject, MemberName),
            ExtractType.Method => ExtractFromMethod(SourceObject, MemberName),
            _ => throw new ArgumentOutOfRangeException()
        };

        if (value is not T typedValue)
        {
            Debug.LogError($"type mismatch, require {typeof(T)}, but {MemberName} is {value?.GetType()}", SourceObject);
            return default;
        }

        return typedValue;
    }

    public void SetOverride(T value)
    {
        IsOverride = true;
        Value = value;
    }

    public void ClearOverride()
    {
        IsOverride = false;
    }

    private object ExtractFromMethod(Object src, string memberName)
    {
        var type = src.GetType();
        var method = type.GetMethod(memberName);
        var value = method?.Invoke(src, null);
        return value;
    }

    private object ExtractFromProperty(Object src, string memberName)
    {
        var type = src.GetType();
        var property = type.GetProperty(memberName);
        var value = property?.GetValue(src);
        return value;
    }

    private object ExtractFromField(Object src, string memberName)
    {
        var type = src.GetType();
        var field = type.GetField(memberName);
        var value = field?.GetValue(src);
        return value;
    }
}

public enum ExtractType
{
    Field,
    Property,
    Method,
}