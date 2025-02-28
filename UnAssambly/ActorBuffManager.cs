using System.Collections.Generic;
using UnityEngine;

[GameFrameOrder(-10)] // before actor tick itself (inner module logic)
public class ActorBuffManager : MonoBehaviour, IEntityAttach, IEntityFrame
{
    private Actor _actor;

    private readonly LinkedList<Buff> _buffs = new();
    private readonly Dictionary<string, Buff> _buffByKey = new();

    private void Awake()
    {
        // bind components
        _actor = this.EnsureComponent<Actor>();
        _actor.OnKill += HandleActorKill;
    }

    public void OnEntityAttach(GameEntity entity)
    {
        // reset
        _buffs.Clear();
        _buffByKey.Clear();
    }

    public void OnEntityFrame(GameEntity entity) // this is unscaled time
    {
        // use unscaled delta time
        var deltaTime = _actor.UnscaledDeltaTime;

        // tick buffs
        var node = _buffs.First;
        while (node != null)
        {
            var buff = node.Value;
            var next = node.Next;

            // timeout
            if (buff.LifeTime >= 0 && buff.ElapsedTime >= buff.LifeTime)
            {
                buff.IsEnd = true;
            }

            // cleanup
            if (buff.IsEnd)
            {
                buff.Unmount();
                _buffs.Remove(node);
                _buffByKey.Remove(buff.Key);
                Destroy(buff.gameObject);
                node = next;
                continue;
            }

            // tick
            buff.Tick(deltaTime);
            buff.ElapsedTime += deltaTime;

            node = next;
        }
    }

    private void HandleActorKill()
    {
        // cleanup
        foreach (var buff in _buffs)
        {
            buff.Unmount();
            Destroy(buff.gameObject);
        }
    }

    public Buff AddBuff(string buffKey)
    {
        if (string.IsNullOrEmpty(buffKey))
        {
            return null;
        }

        var matchingKey = buffKey.ToLowerInvariant();
        var buff = GetBuff(matchingKey);

        // stack (already added)
        if (buff != null)
        {
            buff.StackCount++;
            buff.Mount(_actor.gameObject);
            return null;
        }

        // create
        var prefab = BuffPrefabIndex.Find(buffKey);
        if (!prefab)
        {
            Debug.LogWarning($"buff {matchingKey} not in game database");
            return null;
        }

        buff = Instantiate(prefab, transform); // on enable callback - set default life time
        buff.Key = matchingKey;
        buff.StackCount = 1;
        buff.ElapsedTime = 0;
        buff.Mount(_actor.gameObject);

        // indexing
        _buffs.AddFirst(buff);
        _buffByKey[matchingKey] = buff;

        return buff;
    }

    public void RemoveBuff(string buffKey)
    {
        var buff = GetBuff(buffKey);
        if (buff == null)
        {
            return;
        }

        buff.IsEnd = true;
    }

    public Buff GetBuff(string buffKey)
    {
        if (string.IsNullOrEmpty(buffKey))
        {
            return null;
        }

        var matchingKey = buffKey.ToLowerInvariant();
        if (!_buffByKey.TryGetValue(matchingKey, out var buff))
        {
            Debug.LogWarning($"buff {matchingKey} not found");
            return null;
        }

        return buff;
    }
}

public static class BuffExtensions
{
    public static Buff AddBuff(this Actor actor, string buffKey)
    {
        if (!actor)
        {
            return null;
        }

        if (!actor.TryGetComponent(out ActorBuffManager buffManager))
        {
            return null;
        }

        return buffManager.AddBuff(buffKey);
    }
}