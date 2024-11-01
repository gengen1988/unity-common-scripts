using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public class HitManager : MonoBehaviour, IComponentManager<HitSubject>
{
    private readonly HashSet<HitSubject> _subjects = new();
    private readonly List<HitSubject> _toBeTick = new();

    private void Awake()
    {
        IComponentManager<HitSubject>.RegisterManager(this);
    }

    private void OnDestroy()
    {
        IComponentManager<HitSubject>.DeregisterManager(this);
    }

    // ReSharper disable once IteratorNeverReturns
    private IEnumerator Start()
    {
        for (;;)
        {
            // after physics simulation (such as OnCollisionEnter)
            yield return new WaitForFixedUpdate();

            _toBeTick.Clear();
            _toBeTick.AddRange(_subjects); // these subjects may trigger OnComponentDisabled when tick

            // tick each subject
            float deltaTime = Time.deltaTime;
            foreach (HitSubject subject in _toBeTick)
            {
                subject.Tick(deltaTime);
            }
        }
    }

    public void OnComponentEnabled(HitSubject component)
    {
        _subjects.Add(component);
    }

    public void OnComponentDisabled(HitSubject component)
    {
        _subjects.Remove(component);
    }
}