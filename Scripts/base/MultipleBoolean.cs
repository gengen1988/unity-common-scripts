using System.Collections.Generic;

/**
 * 由多个对象控制的 bool 变量
 * 只要有一个 acquire，就返回 true。全部 release 后，才是 false
 */
public class MultipleBoolean
{
    private readonly HashSet<object> _keySet = new HashSet<object>();

    public static implicit operator bool(MultipleBoolean self)
    {
        return self._keySet.Count > 0;
    }

    public void Acquire(object key)
    {
        _keySet.Add(key);
    }

    public void Release(object key)
    {
        _keySet.Remove(key);
    }

    public void Clear()
    {
        _keySet.Clear();
    }
}