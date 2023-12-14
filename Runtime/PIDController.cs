using System;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

[Serializable]
public class PIDController
{
    public float Kp = 1;
    public float Ki = 0;
    public float Kd = 0;
    public float MaxIntegral = 100;

    private float _integral;
    private float _previousError;

#if ODIN_INSPECTOR
    [ShowInInspector, ReadOnly] 
#endif
    private float _input, _output;

#if ODIN_INSPECTOR
    [Button]
#endif
    public void Reset()
    {
        _integral = 0;
        _previousError = 0;
    }

    public float Update(float error, float deltaTime)
    {
        _integral = Mathf.Clamp(_integral + error * deltaTime, -MaxIntegral, MaxIntegral);
        var derivative = (error - _previousError) / deltaTime;
        var output = Kp * error + Ki * _integral + Kd * derivative;
        _previousError = error;
        _input = error;
        _output = output;

        return output;
    }
}