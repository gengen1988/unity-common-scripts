using System;
using UnityEngine;

[Serializable]
public class HysteresisTrigger
{
    public float Startup = 0.01f;
    public float Cutoff = 0.001f;

    public bool Active { get; private set; }

    public float Update(float value)
    {
        if (Active && Mathf.Abs(value) <= Mathf.Abs(Cutoff))
        {
            Active = false;
        }
        else if (!Active && Mathf.Abs(value) >= Mathf.Abs(Startup))
        {
            Active = true;
        }

        return Active ? value : 0;
    }
}