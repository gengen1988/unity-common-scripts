using System;

public interface IKillable
{
    public event Action OnDeath;
    public bool IsAlive { get; }
    public void Kill();
}