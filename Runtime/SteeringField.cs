using System;
using UnityEngine;

public enum SteeringField
{
    DesignedSpeed,
    HasTarget,
    SelfPosition,
    SelfVelocity,
    [FieldValue(typeof(Vector3))] TargetPosition,
    TargetVelocity,
    FriendsAround,
    SeparateRadius
}

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