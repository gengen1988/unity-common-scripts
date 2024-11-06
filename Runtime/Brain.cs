using System;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public event ActorEvent OnMount, OnUnmount;

    [SerializeField] private bool MountOnStart = true;

    private ActorOld _owner;

    public ActorOld Owner => _owner;

    private void Start()
    {
        if (MountOnStart)
        {
            var actor = GetComponentInParent<ActorOld>();
            Mount(actor);
        }
    }

    private void OnDestroy()
    {
        OnMount = null;
        OnUnmount = null;
    }

    public void Mount(ActorOld actorOld)
    {
        _owner = actorOld;
        OnMount?.Invoke(actorOld);
    }

    public void Unmount(ActorOld actorOld)
    {
        OnUnmount?.Invoke(actorOld);
        _owner = null;
    }
}