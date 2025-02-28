using UnityEngine;

public class BuffChangeOwnerLocalTimeScale : MonoBehaviour, ISubmoduleMount<Buff>, ISubmoduleUnmount<Buff>
{
    [SerializeField] private TimeDomain Domain;
    [SerializeField] private float LocalTimeScale = 1f;

    private GameEntityBridge _bridge;

    public void Mount(Buff submodule)
    {
        _bridge = submodule.Owner.EnsureComponent<GameEntityBridge>();
        _bridge.Clock.EnterDomain(Domain, LocalTimeScale);
    }

    public void Unmount(Buff submodule)
    {
        _bridge.Clock.LeaveDomain(Domain);
    }
}