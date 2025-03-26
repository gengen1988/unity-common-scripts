using UnityEngine;

[CreateAssetMenu]
public class TimeDomain : ScriptableObject
{
    [SerializeField] private float globalTimeScale = 1f;

    public float GlobalTimeScale
    {
        get => globalTimeScale;
        set => globalTimeScale = value;
    }
}