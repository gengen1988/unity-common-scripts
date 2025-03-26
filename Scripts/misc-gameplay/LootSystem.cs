using UnityEngine;

public class LootSystem : MonoBehaviour
{
    [SerializeField] private GameObject lootPrefab;
    [SerializeField] private float activateSpeed;

    private EventBinding<OnUnitDeath> _binder;

    private void Awake()
    {
        _binder = new EventBinding<OnUnitDeath>(HandleUnitDeath);
    }

    private void OnEnable()
    {
        GlobalEventBus<OnUnitDeath>.Register(_binder);
    }

    private void OnDisable()
    {
        GlobalEventBus<OnUnitDeath>.Deregister(_binder);
    }

    private void HandleUnitDeath(OnUnitDeath evtData)
    {
        var unit = evtData.Subject;
        var center = unit.CenterPosition;

        // Spawn a loot item at the unit's position
        if (lootPrefab != null)
        {
            // Instantiate the loot at the unit's center position
            var lootInstance = PoolUtil.Spawn(lootPrefab, center, Quaternion.identity);

            // If the loot prefab has a Loot component, initialize it
            if (lootInstance.TryGetComponent(out Pickup pickup))
            {
                // Optional: Set any properties on the loot based on the unit
                // For example, you could vary the value based on the unit type
                // loot.SetValue(unit.LootValue);
                var initialVelocity = Vector2.up * activateSpeed;
                pickup.Born(initialVelocity);
            }
        }
    }
}