using UnityEngine;

// - paralyzed
[RequireComponent(typeof(Modifier))]
public class BuffDealDamageOneShot : MonoBehaviour
{
    [SerializeField] private int damageAmount;

    private Modifier _modifier;
    private Health _health;

    private void Awake()
    {
        TryGetComponent(out _modifier);
        _modifier.Manager.TryGetComponent(out _health);
        _modifier.OnAdd += HandleModifierAdd;
    }

    private void OnDestroy()
    {
        _modifier.OnAdd -= HandleModifierAdd;
    }

    private void HandleModifierAdd()
    {
        if (_health)
        {
            _health.DealDamage(damageAmount);
        }
    }
}