using System;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

[Serializable]
public class PIDControllerFloat
{
    [SerializeField] private float Kp = 10; // 基本的控制力度
    [SerializeField] private float Ki = 0; // 默认没有积分项。如果需要一定程度的预测，可以调大一些
    [SerializeField] private float Kd = 1; // 提供阻尼感

    // prevent saturation
    [SerializeField] private float MaxError = 100f;
    [SerializeField] private float MaxIntegral = 100f;
    [SerializeField] private float MaxOutput = 100f;

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

    /**
     * output are not scaled by delta time
     */
    public float Update(float desiredValue, float currentValue, float deltaTime)
    {
        var error = desiredValue - currentValue;
        var clampedError = Mathf.Clamp(error, -MaxError, MaxError);
        var integral = _integral + clampedError;
        var clampedIntegral = Mathf.Clamp(integral, -MaxIntegral, MaxIntegral);
        var derivative = (clampedError - _previousError) / deltaTime;
        var output = Kp * clampedError + Ki * clampedIntegral + Kd * derivative;
        var clampedOutput = Mathf.Clamp(output, -MaxOutput, MaxOutput);

        _integral = clampedIntegral;
        _previousError = clampedError;
        _input = clampedError;
        _output = clampedOutput;

        return clampedOutput;
    }
}

[Serializable]
public class PIDControllerVector3
{
    [SerializeField] private float Kp = 10; // 基本的控制力度
    [SerializeField] private float Ki = 0; // 默认没有积分项。如果需要一定程度的预测，可以调大一些
    [SerializeField] private float Kd = 1; // 提供阻尼感

    // prevent saturation
    [SerializeField] private float MaxError = 100f;
    [SerializeField] private float MaxIntegral = 100f;
    [SerializeField] private float MaxOutput = 100f;

    private Vector3 _integral;
    private Vector3 _previousError;

#if ODIN_INSPECTOR
    [ShowInInspector, ReadOnly]
#endif
    private Vector3 _input, _output;

#if ODIN_INSPECTOR
    [Button]
#endif
    public void Reset()
    {
        _integral = Vector3.zero;
        _previousError = Vector3.zero;
    }

    /**
     * output are not scaled by delta time
     */
    public Vector3 Update(Vector3 desiredValue, Vector3 currentValue, float deltaTime)
    {
        var error = desiredValue - currentValue;
        var clampedError = Vector3.ClampMagnitude(error, MaxError);
        var integral = _integral + clampedError;
        var clampedIntegral = Vector3.ClampMagnitude(integral, MaxIntegral);
        var derivative = (clampedError - _previousError) / deltaTime;
        var output = Kp * clampedError + Ki * clampedIntegral + Kd * derivative;
        var clampedOutput = Vector3.ClampMagnitude(output, MaxOutput);

        _integral = clampedIntegral;
        _previousError = clampedError;
        _input = clampedError;
        _output = clampedOutput;

        return clampedOutput;
    }
}