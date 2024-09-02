using System;
using UnityEngine;

[Serializable]
public class InterfaceReference<T> where T : class
{
    public MonoBehaviour Source;

    public T GetInterface()
    {
        return Source as T;
    }
}