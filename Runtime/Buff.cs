using UnityEngine;

public class Buff : Submodule<Buff>
{
    [SerializeField] private float DefaultLifeTime = -1; // -1 means forever

    public bool IsEnd { get; set; }
    public float ElapsedTime { get; set; }
    public string Key { get; set; }
    public int StackCount { get; set; }
    public float LifeTime { get; set; }

    private void OnEnable()
    {
        LifeTime = DefaultLifeTime;
    }
}