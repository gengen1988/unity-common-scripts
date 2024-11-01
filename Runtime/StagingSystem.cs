using System;
using System.Collections.Generic;
using UnityEngine;

public partial class StagingSystem : SingletonBehaviour<StagingSystem>
{
    private class Entry
    {
        public GameObject GameObject;
        public Transform Parent;
    }

    private Transform _container;
    private readonly Dictionary<GameObject, Entry> _entryByGameObject = new();

    protected override void AfterAwake()
    {
        GameObject go = new GameObject("StagingContainer");
        Transform trans = go.transform;
        trans.SetParent(transform);
        go.SetActive(false);
        _container = trans;
    }

    private GameObject _Prepare(Func<Transform, GameObject> constructor, Transform parent)
    {
        GameObject go = constructor(_container);
        Entry entry = new Entry
        {
            GameObject = go,
            Parent = parent,
        };
        _entryByGameObject[go] = entry;
        return go;
    }

    private void _Commit(GameObject go)
    {
        Entry entry = _entryByGameObject[go];
        entry.GameObject.transform.SetParent(entry.Parent);
        _entryByGameObject.Remove(go);
    }

    private bool _IsStaging(GameObject go)
    {
        return _entryByGameObject.ContainsKey(go);
    }

    private void _Remove(GameObject go)
    {
        _entryByGameObject.Remove(go);
    }
}

public partial class StagingSystem
{
    public static bool IsStaging(GameObject go)
    {
        return Instance._IsStaging(go);
    }

    public static GameObject Prepare(Func<Transform, GameObject> constructor, Transform parent = null)
    {
        return Instance._Prepare(constructor, parent);
    }

    public static void Commit(GameObject go)
    {
        Instance._Commit(go);
    }

    public static void Remove(GameObject go)
    {
        Instance._Remove(go);
    }
}