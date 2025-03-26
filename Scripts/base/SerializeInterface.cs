using System;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class SerializeInterface<T> where T : class
{
    [SerializeField] private Object value;

    public T GetReference()
    {
        return value as T;
    }
}