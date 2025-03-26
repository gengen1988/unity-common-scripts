using UnityEngine;

// - ignition
// - poison
[RequireComponent(typeof(Modifier))]
public class BuffDealDamageContinuous : MonoBehaviour, IModifierAttach, IModifierFrame
{
    [SerializeField] private float damagePerSeconds = 100f;
    [SerializeField] private float interval = 0.1f;

    private Modifier _modifier;
    private Health _health;
    private float _storeTime;

    public void OnModifierAttach(GameEntity entity)
    {
        TryGetComponent(out _modifier);
        entity.Proxy.TryGetComponent(out _health);
    }

    public void OnModifierFrame(GameEntity entity, float deltaTime)
    {
        if (!_health)
        {
            return;
        }

        if (interval <= 0)
        {
            // deal damage every frame
            DealDamage(deltaTime);
        }
        else
        {
            // deal damage every interval
            _storeTime += deltaTime;
            while (_storeTime >= interval)
            {
                DealDamage(interval);
                _storeTime -= interval;
            }
        }
    }

    private void DealDamage(float time)
    {
        var stackCount = _modifier.StackCount;
        var damage = Mathf.FloorToInt(damagePerSeconds * stackCount * time);
        _health.DealDamage(damage);
    }
}