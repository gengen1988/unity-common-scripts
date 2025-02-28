using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct RandomPerlin
{
    [SerializeField] private float Step;

    private float _position;

    /**
     * generate random value from 0 to 1
     */
    public float Next()
    {
        _position += Step;
        return Mathf.PerlinNoise1D(_position);
    }

    /**
     * generate random value form min to max
     */
    public float Next(float reference, float delta)
    {
        var randomValue = Next() - .5f;
        return reference + randomValue * delta;
    }

    public void Reset()
    {
        _position = Random.Range(0, 1000f);
    }

    public void Reset(float position)
    {
        _position = position;
    }
}