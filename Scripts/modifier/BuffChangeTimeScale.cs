using UnityEngine;

// - paralyzed
// - haste
public class BuffChangeTimeScale : MonoBehaviour, IModifierAttach, IModifierDetach
{
    [SerializeField] private TimeDomain domain;
    [SerializeField] private bool overrideTimeScale;
    [SerializeField] private float localTimeScale = 1f;

    public void OnModifierAttach(GameEntity entity)
    {
        var clock = entity.Proxy.Clock;
        clock.EnterDomain(domain);
        if (overrideTimeScale)
        {
            clock.SetDomainLocalTimeScale(domain, localTimeScale);
        }
    }

    public void OnModifierDetach(GameEntity entity)
    {
        entity.Proxy.Clock.LeaveDomain(domain);
    }
}