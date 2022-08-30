using System.Collections.Generic;

public class LimitQueue<T> : Queue<T>
{
	int max;

	public LimitQueue(int maxSize)
	{
		max = maxSize;
	}

	public new void Enqueue(T obj)
	{
		base.Enqueue(obj);
		while (Count > max) Dequeue();
	}
}