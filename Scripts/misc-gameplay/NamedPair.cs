using System;

[Serializable]
public struct NamedPair<T>
{
    public string Name;
    public T Value;
}