using System.Collections;
using System.Collections.Generic;

public class BoundedQueue<T> : IEnumerable<T>
{
    private readonly int _capacity;

    private readonly Queue<T> _queue;

    public int Capacity => _capacity;
    public int Count => _queue.Count;

    public BoundedQueue(int capacity)
    {
        _capacity = capacity;
        _queue = new Queue<T>();
    }

    public void Enqueue(T item)
    {
        _queue.Enqueue(item);
        while (_queue.Count > _capacity)
        {
            _queue.Dequeue();
        }
    }

    public T Dequeue()
    {
        return _queue.Dequeue();
    }

    public T Peek()
    {
        return _queue.Peek();
    }

    public void Clear()
    {
        _queue.Clear();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _queue.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}