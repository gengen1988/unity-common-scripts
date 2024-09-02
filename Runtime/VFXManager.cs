using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public bool DebugLog;

    private readonly List<VFXCtrl> _allVFXs = new();

    private void LateUpdate()
    {
        float deltaTime = Time.deltaTime;
        for (int i = _allVFXs.Count - 1; i >= 0; --i)
        {
            VFXCtrl vfx = _allVFXs[i];
            vfx.Tick(deltaTime);
            if (vfx.IsFinished())
            {
                _allVFXs.RemoveAt(i);
                PoolWrapper.Despawn(vfx.gameObject);
            }
        }
    }

    public VFXCtrl Spawn(
        GameObject prefab,
        Vector3 position,
        Quaternion rotation,
        Transform following,
        float timeout)
    {
        if (!prefab)
        {
            if (DebugLog)
            {
                Debug.LogWarning("vfx not exists");
            }

            return null;
        }

        GameObject go = PoolWrapper.Spawn(prefab, position, rotation);
        VFXCtrl vfx = go.EnsureComponent<VFXCtrl>();
        vfx.Init(following, timeout);
        _allVFXs.Add(vfx);
        return vfx;
    }
}