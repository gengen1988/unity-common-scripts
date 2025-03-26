// Interface for different projectile fuze types

using UnityEngine;

public interface IProjectileFuze
{
    // TODO: Add methods for fuze behavior
}


// Explodes after a set time
public class DelayFuze : MonoBehaviour, IProjectileFuze
{
    // TODO: Implement delay fuze logic
}

// Explodes when near a target
public class ProximityFuze : MonoBehaviour, IProjectileFuze
{
    // TODO: Implement proximity fuze logic
}

// A fuze that control by player (or AI assist)
public class ManualFuze : MonoBehaviour, IProjectileFuze
{
}