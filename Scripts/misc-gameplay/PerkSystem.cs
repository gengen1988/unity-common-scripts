using Weaver;

public class PerkSystem : WeaverSingletonBehaviour<GameWorld>
{
    [AssetReference] private static readonly GameWorld SingletonPrefab;

    private ModifierManager _modifierManager;

    private void Awake()
    {
        TryGetComponent(out _modifierManager);
    }

    public void AddPerk(ModifierProfile profile)
    {
        _modifierManager.AddModifier(profile);
    }
}