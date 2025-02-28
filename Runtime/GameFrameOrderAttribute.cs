using System;

[AttributeUsage(AttributeTargets.Class)]
public class GameFrameOrderAttribute : Attribute
{
    public int Order;

    public GameFrameOrderAttribute(int i)
    {
        Order = i;
    }
}