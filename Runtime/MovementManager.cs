using System.Collections.Generic;
using UnityEngine;

/**
 * 这个系统的好处是，生成的新对象不会错过那一帧的移动
 */
[DefaultExecutionOrder(10)]
public class MovementManager : MonoBehaviour, IComponentManager<MoveSubject>
{
    private readonly HashSet<MoveSubject> _subjects = new();

    private void Awake()
    {
        IComponentManager<MoveSubject>.RegisterManager(this);
    }

    private void OnDestroy()
    {
        IComponentManager<MoveSubject>.DeregisterManager(this);
    }

    private void FixedUpdate()
    {
        foreach (MoveSubject subject in _subjects)
        {
            if (!subject.isActiveAndEnabled)
            {
                continue;
            }

            float deltaTime = subject.GetDeltaTime() * Time.timeScale;
            subject.Tick(deltaTime);
        }

        foreach (MoveSubject subject in _subjects)
        {
            if (!subject.isActiveAndEnabled)
            {
                continue;
            }

            subject.Commit();
        }
    }

    public void OnComponentEnabled(MoveSubject component)
    {
        _subjects.Add(component);
    }

    public void OnComponentDisabled(MoveSubject component)
    {
        _subjects.Remove(component);
    }
}