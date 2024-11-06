using UnityEngine;

[CreateAssetMenu(fileName = "New Prefab Database")]
public class PrefabDatabase : ScriptableObject
{
    public string SearchPath;
    public GameObject[] Entries;
}