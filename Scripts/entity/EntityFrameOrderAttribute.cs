using System;

[AttributeUsage(AttributeTargets.Class)]
public class EntityFrameOrderAttribute : Attribute
{
    public readonly int Order;

    public EntityFrameOrderAttribute(int i = 0)
    {
        Order = i;
    }
}

public struct FrameOrder
{
    public const int Early = -10;
    public const int Default = 0;
    public const int Late = 10;
}