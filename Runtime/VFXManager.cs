using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class VFXManager : MonoBehaviour, ISystem
{
    public bool DebugLog;

    private readonly Dictionary<string, VFXCtrl> _vfxById = new();
    private readonly List<VFXCtrl> _followingVFXs = new();

    private void LateUpdate()
    {
        // follow logic
        for (int i = _followingVFXs.Count - 1; i >= 0; --i)
        {
            VFXCtrl vfx = _followingVFXs[i];
            Transform followingTarget = vfx.FollowingTarget;

            // clean destroyed follow target
            if (!PoolWrapper.IsAlive(followingTarget.gameObject))
            {
                if (DebugLog)
                {
                    Debug.Log($"[{Time.frameCount}] clean unused follow vfx");
                }

                _followingVFXs.RemoveAt(i);
                continue;
            }

            vfx.Refs.transform.position = vfx.FollowingTarget.position;
        }
    }

    public string SpawnIfExists(GameObject prefab, Vector3 position, Quaternion rotation, float timeout = -1)
    {
        if (!prefab)
        {
            return null;
        }

        VFXCtrl vfx = new()
        {
            Prefab = prefab,
            Timeout = timeout
        };
        VFXLifespan(vfx, position, rotation).Forget();
        return vfx.Id;
    }

    public string SpawnIfExists(GameObject prefab, Transform following, float timeout = -1)
    {
        if (!prefab)
        {
            return null;
        }

        VFXCtrl vfx = new()
        {
            Prefab = prefab,
            Timeout = timeout,
            IsFollow = true,
            FollowingTarget = following
        };
        VFXLifespan(vfx, following.position, following.rotation).Forget();
        return vfx.Id;
    }

    public void Despawn(string vfxId, bool force = false)
    {
        if (string.IsNullOrEmpty(vfxId))
        {
            return;
        }

        while (_vfxById.ContainsKey(vfxId))
        {
            VFXCtrl vfx = _vfxById[vfxId];
            vfx.CTS.Cancel();

            if (!force)
            {
                break;
            }
        }
    }

    private async UniTask VFXLifespan(VFXCtrl vfx, Vector3 position, Quaternion rotation)
    {
        if (DebugLog)
        {
            Debug.Log("vfx start");
        }

        // init
        _vfxById[vfx.Id] = vfx;
        if (vfx.IsFollow)
        {
            _followingVFXs.Add(vfx);
        }

        GameObject go = PoolWrapper.Spawn(vfx.Prefab, position, rotation, transform);
        go.EnsureComponent(ref vfx.Refs, true);

        // get controls
        ParticleSystemEventTrigger trigger = vfx.Refs.Trigger;
        ParticleSystem ps = vfx.Refs.Particles;

        // playing
        try
        {
            vfx.CTS = new CancellationTokenSource();
            UniTask task = trigger.OnStopped.OnInvokeAsync(vfx.CTS.Token);
            if (vfx.Timeout > 0)
            {
                task = task.Timeout(TimeSpan.FromSeconds(vfx.Timeout));
            }

            await task;
            if (DebugLog)
            {
                Debug.Log("ps success played");
            }
        }
        catch
        {
            if (DebugLog)
            {
                Debug.Log("ps canceled");
            }

            ps.Stop(true);
            try
            {
                vfx.CTS.Dispose();
                vfx.CTS = new CancellationTokenSource();
                await trigger.OnStopped.OnInvokeAsync(vfx.CTS.Token);
                if (DebugLog)
                {
                    Debug.Log("ps cancel gracefully");
                }
            }
            catch
            {
                if (DebugLog)
                {
                    Debug.Log("ps force cancel");
                }

                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        // unload
        if (vfx.IsFollow)
        {
            _followingVFXs.Remove(vfx);
        }

        PoolWrapper.Despawn(go);

        _vfxById.Remove(vfx.Id);
        vfx.Dispose();
        if (DebugLog)
        {
            Debug.Log("ps unloaded");
        }
    }
}