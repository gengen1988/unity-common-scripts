using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class PrefabDataStore : ScriptableObject
{
    public string Prefix;

    [AssetList(Path = "_Game/Prefabs", CustomFilterMethod = nameof(Filter))]
    public GameObject[] Entries;

    private bool Filter(GameObject asset)
    {
#if UNITY_EDITOR
        var assetPrefix = asset.name.Split("_").FirstOrDefault();
        if (!string.Equals(Prefix, assetPrefix, StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        return true;
#else
        return false;
#endif
    }
}