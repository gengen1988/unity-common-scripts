using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class DataView<T> : MonoBehaviour, IDataView<T>
{
    public GameObject Prefab;
    public Transform Container;

    private IDataEntry<T>[] Entries;

    public void Init()
    {
        Container.DestroyChildren();
        Entries = EnumerateData()
            .Select(data =>
            {
                GameObject go = Instantiate(Prefab, Container);
                IDataEntry<T> entry = go.GetComponent<IDataEntry<T>>();
                entry.OnDataChanged(data);
                return entry;
            })
            .ToArray();

        AfterSpawn();
    }

    public void Refresh()
    {
        foreach (IDataEntry<T> entry in Entries)
        {
            entry.Refresh();
        }

        RefreshSelf();
    }

    public abstract IEnumerable<T> EnumerateData();

    protected virtual void AfterSpawn()
    {
    }

    protected virtual void RefreshSelf()
    {
    }
}