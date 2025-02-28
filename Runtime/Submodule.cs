using UnityEngine;

public abstract class Submodule<T> : MonoBehaviour where T : Submodule<T>
{
    [SerializeField] private bool DebugLog;

    private ISubmoduleMount<T>[] _mountables;
    private ISubmoduleUnmount<T>[] _unmountables;
    private ISubmoduleTick<T>[] _tickables;

    public GameObject Owner { get; private set; }

    private void Awake()
    {
        _mountables = GetComponents<ISubmoduleMount<T>>();
        _unmountables = GetComponents<ISubmoduleUnmount<T>>();
        _tickables = GetComponents<ISubmoduleTick<T>>();
    }

    public void Mount(GameObject owner)
    {
        Owner = owner;
        var submodule = this as T;
        foreach (var mountable in _mountables)
        {
            mountable.Mount(submodule);
        }

        if (DebugLog)
        {
            Debug.Log($"submodule {name} mounted");
        }
    }

    public void Unmount()
    {
        var submodule = this as T;
        foreach (var unmountable in _unmountables)
        {
            unmountable.Unmount(submodule);
        }

        Owner = null;

        if (DebugLog)
        {
            Debug.Log($"submodule {name} unmounted");
        }
    }

    public void Tick(float deltaTime)
    {
        if (!Owner)
        {
            Debug.LogWarning($"{this} not mount", this);
            return;
        }

        foreach (var tickable in _tickables)
        {
            tickable.Tick(this as T, deltaTime);
        }
    }
}

public interface ISubmoduleMount<in T>
{
    void Mount(T submodule);
}

public interface ISubmoduleUnmount<in T>
{
    void Unmount(T submodule);
}

public interface ISubmoduleTick<in T>
{
    void Tick(T submodule, float deltaTime);
}