using System;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public event ActorEvent OnMount, OnUnmount;

    [SerializeField] private bool MountOnStart = true;

    private Actor _owner;

    public Actor Owner => _owner;

    private void Start()
    {
        if (MountOnStart)
        {
            var actor = GetComponentInParent<Actor>();
            Mount(actor);
        }
    }

    private void OnDestroy()
    {
        OnMount = null;
        OnUnmount = null;
    }

    public void Mount(Actor actor)
    {
        _owner = actor;
        OnMount?.Invoke(actor);
    }

    public void Unmount(Actor actor)
    {
        OnUnmount?.Invoke(actor);
        _owner = null;
    }
}