using UnityEngine;

[DisallowMultipleComponent]
public class VFXComponentRefs : MonoBehaviour
{
    public ParticleSystem Particles { get; private set; }
    public ParticleSystemEventTrigger Trigger { get; private set; }

    private void Awake()
    {
        Particles = GetComponentInChildren<ParticleSystem>();
        if (!Particles)
        {
            return;
        }

        ParticleSystemEventTrigger trigger = null;
        Particles.gameObject.EnsureComponent(ref trigger, true);
        Trigger = trigger;
    }
}