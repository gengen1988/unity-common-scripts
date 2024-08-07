using System.Collections.Generic;
using UnityEngine;

/**
 * 这个系统的好处是，生成的新对象不会错过那一帧的移动
 */
[DefaultExecutionOrder(10)]
public class MovementResolver : MonoBehaviour, IComponentManager<BlendableMovement>
{
    private readonly HashSet<BlendableMovement> _subjects = new();

    private void Awake()
    {
        IComponentManager<BlendableMovement>.RegisterManager(this);
    }

    private void OnDestroy()
    {
        IComponentManager<BlendableMovement>.DeregisterManager(this);
    }

    private void FixedUpdate()
    {
        float deltaTime = Time.deltaTime;
        foreach (BlendableMovement subject in _subjects)
        {
            if (!subject.isActiveAndEnabled)
            {
                continue;
            }

            subject.Tick(deltaTime);
        }

        foreach (BlendableMovement subject in _subjects)
        {
            if (!subject.isActiveAndEnabled)
            {
                continue;
            }

            subject.Commit();
        }
    }

    public void OnComponentEnabled(BlendableMovement component)
    {
        _subjects.Add(component);
    }

    public void OnComponentDisabled(BlendableMovement component)
    {
        _subjects.Remove(component);
    }
}