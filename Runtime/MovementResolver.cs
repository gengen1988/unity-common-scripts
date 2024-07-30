using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(10)]
public class MovementResolver : MonoBehaviour, ISystem
{
    private readonly HashSet<IManagedMovement> _subjects = new();

    private void FixedUpdate()
    {
        float deltaTime = Time.deltaTime;
        foreach (IManagedMovement subject in _subjects)
        {
            subject.Tick(deltaTime);
        }

        foreach (IManagedMovement subject in _subjects)
        {
            subject.Commit();
        }
    }

    public void RegisterSubject(IManagedMovement subject)
    {
        _subjects.Add(subject);
    }

    public void RemoveSubject(IManagedMovement subject)
    {
        _subjects.Remove(subject);
    }
}