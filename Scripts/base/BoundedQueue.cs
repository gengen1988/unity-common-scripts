using System.Collections;
using System.Collections.Generic;

public class BoundedQueue<T> : IEnumerable<T>
{
    private readonly int _capacity;

    private readonly LinkedList<T> _queue;

    public int Capacity => _capacity;
    public int Count => _queue.Count;

    public BoundedQueue(int capacity)
    {
        _capacity = capacity;
        _queue = new LinkedList<T>();
    }

    public void Enqueue(T item)
    {
        _queue.AddFirst(item);
        while (_queue.Count > _capacity)
        {
            _queue.RemoveLast();
        }
    }

    public T Dequeue()
    {
        T result = _queue.Last.Value;
        _queue.RemoveLast();
        return result;
    }

    public T PeekHead()
    {
        return _queue.First.Value;
    }

    public T PeekTail()
    {
        return _queue.Last.Value;
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