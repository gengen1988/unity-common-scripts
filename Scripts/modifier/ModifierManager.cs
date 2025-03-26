using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntityProxy))]
public class ModifierManager : MonoBehaviour, IEntityAttach, IEntityDetach, IEntityFrame
{
    private readonly LinkedList<Modifier> _modifiers = new();
    private readonly Dictionary<ModifierProfile, Modifier> _modifierByProfile = new();

    public void OnEntityAttach(GameEntity entity)
    {
        // reset
        _modifiers.Clear();
        _modifierByProfile.Clear();
    }

    public void OnEntityDetach(GameEntity entity)
    {
        // cleanup for death
        foreach (var modifier in _modifiers)
        {
            modifier.Remove();
            modifier.Detach(entity);
            Destroy(modifier.gameObject);
        }
    }

    public void OnEntityFrame(GameEntity entity, float deltaTime)
    {
        // tick buffs
        var node = _modifiers.First;
        while (node != null)
        {
            var buff = node.Value;
            var next = node.Next;

            // born (first tick)
            if (buff.State == ModifierState.Born)
            {
                buff.Attach(entity);
                buff.State = ModifierState.Alive;
            }

            // alive (tick)
            if (buff.State == ModifierState.Alive)
            {
                buff.ElapsedTime += deltaTime;
                buff.Tick(entity, deltaTime);
                if (buff.LifeTime > 0 && buff.ElapsedTime >= buff.LifeTime)
                {
                    RemoveModifier(buff.Profile);
                }
            }

            // death (cleanup)
            if (buff.State == ModifierState.Dead)
            {
                _modifiers.Remove(node);
                buff.Detach(entity);
                Destroy(buff.gameObject);
            }

            node = next;
        }
    }

    public Modifier AddModifier(ModifierProfile profile)
    {
        if (!profile)
        {
            return null;
        }

        var modifier = GetModifier(profile);

        // stack (already added)
        if (modifier)
        {
            modifier.Add();
            return modifier;
        }

        // create
        var prefab = profile.Prefab;
        if (!prefab)
        {
            Debug.LogWarning($"buff {profile} doesn't have a prefab");
            return null;
        }

        // on enable callback - set default life time
        modifier = Instantiate(prefab, transform);
        modifier.Profile = profile;
        modifier.LifeTime = profile.DefaultLifeTime;
        modifier.StackCount = 0;
        modifier.Manager = this;
        modifier.State = ModifierState.Born;
        modifier.Add(); // trigger on add event

        // indexing
        _modifierByProfile[profile] = modifier;
        _modifiers.AddFirst(modifier);
        return modifier;
    }

    public void RemoveModifier(ModifierProfile profile)
    {
        if (!profile)
        {
            return;
        }

        var modifier = GetModifier(profile);
        if (!modifier)
        {
            return;
        }

        _modifierByProfile.Remove(profile); // remove index immediately
        modifier.State = ModifierState.Dead; // will be destroyed in next tick
        modifier.Remove();
    }

    public Modifier GetModifier(ModifierProfile profile)
    {
        if (!profile)
        {
            return null;
        }

        return _modifierByProfile.GetValueOrDefault(profile);
    }
}

// public static class ModifierUtil
// {
//     public void AddModifierIfNotNull(this modifier)
//     {
//     }
// }