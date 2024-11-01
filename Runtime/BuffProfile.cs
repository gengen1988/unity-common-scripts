using UnityEngine;

[CreateAssetMenu]
public class BuffProfile : ScriptableObject
{
    public GameObject Prefab;
    public float DefaultLifeTime = -1f;
    public int StackCapacity = 1;
}