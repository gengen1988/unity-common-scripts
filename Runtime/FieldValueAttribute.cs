using System;

[AttributeUsage(AttributeTargets.Field)]
public class FieldValueAttribute : Attribute
{
    private readonly Type _valueType;

    public FieldValueAttribute(Type type)
    {
        _valueType = type;
    }

    public Type GetValueType()
    {
        return _valueType;
    }
}