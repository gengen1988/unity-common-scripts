using System;
using System.Collections.Generic;

[Serializable]
public class LimitQueue<T> : Queue<T>
{
    int max;

    public LimitQueue(int maxSize)
    {
        max = maxSize;
    }

    public new void Enqueue(T obj)
    {
        while (Count > max) Dequeue();
        base.Enqueue(obj);
    }
}