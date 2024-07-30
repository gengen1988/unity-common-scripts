using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(5000)] // after other fixed update logic (such as rb.MovePosition)
public class HitManager : MonoBehaviour, ISystem
{
    private readonly HashSet<HitSubject> _hitSubjects = new();

    private void FixedUpdate()
    {
        float deltaTime = Time.deltaTime;
        foreach (HitSubject hitSubject in _hitSubjects)
        {
            hitSubject.Tick(deltaTime);
        }
    }

    public void RegisterSubject(HitSubject subject)
    {
        _hitSubjects.Add(subject);
    }

    public void RemoveSubject(HitSubject subject)
    {
        _hitSubjects.Remove(subject);
    }
}