using UnityEngine;

public class PerkIgnitionBullet : MonoBehaviour, IModifierAttach, IModifierDetach
{
    private EventBinding<OnProjectileInit> _binding;

    private void Awake()
    {
        _binding = new EventBinding<OnProjectileInit>(HandleProjectileLaunch);
    }

    private void OnDestroy()
    {
        _binding.Dispose();
    }

    public void OnModifierAttach(GameEntity entity)
    {
        GlobalEventBus<OnProjectileInit>.Register(_binding);
    }

    public void OnModifierDetach(GameEntity entity)
    {
        GlobalEventBus<OnProjectileInit>.Deregister(_binding);
    }

    private void HandleProjectileLaunch(OnProjectileInit evt)
    {
    }
}

public struct OnProjectileInit : IGlobalEvent
{
    public Projectile Subject;
}