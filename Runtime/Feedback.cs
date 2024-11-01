using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Feedback : MonoBehaviour
{
    public event Action<Feedback> OnPlay, OnStop;

    [SerializeField] private float DefaultLifeTime = -1f;

    private float _elapsedTime;
    private readonly HashSet<Guid> _blockers = new();

    private void OnDestroy()
    {
        OnPlay = null;
        OnStop = null;
    }

    private void Update()
    {
        Tick(Time.deltaTime);
    }

    private void Tick(float deltaTime)
    {
        if (IsFinished())
        {
            PoolUtil.Despawn(gameObject);
            return;
        }

        // timeout
        if (DefaultLifeTime > 0 && _elapsedTime >= DefaultLifeTime)
        {
            Stop(); // graceful stop
            return;
        }

        _elapsedTime += deltaTime;
    }

    private void Play()
    {
        OnPlay?.Invoke(this);
    }

    private void Stop()
    {
        OnStop?.Invoke(this);
    }

    private bool IsFinished()
    {
        return _blockers.Count == 0;
    }

    public Guid AcquireBlocker()
    {
        Guid id = Guid.NewGuid();
        _blockers.Add(id);
        return id;
    }

    public void ReleaseBlocker(Guid blockId)
    {
        _blockers.Remove(blockId);
    }

    // static interfaces
    public static Feedback Spawn(Feedback prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (!prefab)
        {
            return null;
        }

        var go = PoolUtil.Spawn(prefab.gameObject, position, rotation, parent);
        go.TryGetComponent(out Feedback feedback);
        feedback._blockers.Clear();
        feedback._elapsedTime = 0;
        feedback.Play();
        return feedback;
    }

    public static Feedback Spawn(Feedback prefab, Transform parent = null)
    {
        var position = parent ? parent.position : Vector3.zero;
        var rotation = parent ? parent.rotation : Quaternion.identity;
        return Spawn(prefab, position, rotation, parent);
    }

    public static void Despawn(Component component)
    {
        if (!component)
        {
            return;
        }

        if (component is Feedback feedback)
        {
            feedback.Stop();
            return;
        }

        Despawn(component.gameObject);
    }

    public static void Despawn(GameObject instance)
    {
        if (!instance)
        {
            return;
        }

        if (instance.TryGetComponent(out Feedback feedback))
        {
            feedback.Stop();
        }
    }
}