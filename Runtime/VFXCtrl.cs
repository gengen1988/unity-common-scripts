using System;
using System.Threading;
using UnityEngine;

public class VFXCtrl : IDisposable
{
    public readonly string Id;
    public float Timeout;
    public bool IsFollow;
    public GameObject Prefab;
    public VFXComponentRefs Refs;
    public Transform FollowingTarget;
    public CancellationTokenSource CTS;

    public VFXCtrl(string id = null)
    {
        if (string.IsNullOrEmpty(id))
        {
            id = Guid.NewGuid().ToString();
        }

        Id = id;
    }

    public void Dispose()
    {
        CTS?.Dispose();
    }
}