using UnityEngine;

public static class VFXWrapper
{
    public static VFXCtrl Spawn(GameObject prefab, Vector3 position, Quaternion rotation, float timeout = -1)
    {
        VFXManager mgr = SystemManager.GetSystem<VFXManager>();
        return mgr.Spawn(prefab, position, rotation, null, timeout);
    }

    public static VFXCtrl Spawn(GameObject prefab, Transform following, float timeout = -1)
    {
        VFXManager mgr = SystemManager.GetSystem<VFXManager>();
        return mgr.Spawn(prefab, following.position, following.rotation, following, timeout);
    }
}