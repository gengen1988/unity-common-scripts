using System;
using UnityEngine;
using UnityEngine.Serialization;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

[Serializable]
public class PIDControlFloat
{
    [SerializeField] private float kp = 10; // 基本的控制力度
    [SerializeField] private float ki = 0; // 默认没有积分项。如果需要一定程度的预测，可以调大一些
    [SerializeField] private float kd = 1; // 提供阻尼感

    // prevent saturation
    [SerializeField] private float maxIntegral = 100f;
    [SerializeField] private float maxOutput = 100f;

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
    public float CalcControlForce(float error, float deltaTime)
    {
        var integral = _integral + error;
        var clampedIntegral = Mathf.Clamp(integral, -maxIntegral, maxIntegral);
        var derivative = (error - _previousError) / deltaTime;
        var output = kp * error + ki * clampedIntegral + kd * derivative;
        var clampedOutput = Mathf.Clamp(output, -maxOutput, maxOutput);

        _integral = clampedIntegral;
        _previousError = error;
        _input = error;
        _output = clampedOutput;

        return clampedOutput;
    }
}

[Serializable]
public class PIDControlVector3
{
    [SerializeField] private float kp = 10; // 基本的控制力度
    [SerializeField] private float ki = 0; // 默认没有积分项。如果需要一定程度的预测，可以调大一些
    [SerializeField] private float kd = 1; // 提供阻尼感

    // prevent saturation
    [SerializeField] private float maxIntegral = 100f;
    [SerializeField] private float maxOutput = 100f;

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
    public Vector3 CalcControlForce(Vector3 error, float deltaTime)
    {
        var integral = _integral + error;
        var clampedIntegral = Vector3.ClampMagnitude(integral, maxIntegral);
        var derivative = (error - _previousError) / deltaTime;
        var output = kp * error + ki * clampedIntegral + kd * derivative;
        var clampedOutput = Vector3.ClampMagnitude(output, maxOutput);

        _integral = clampedIntegral;
        _previousError = error;
        _input = error;
        _output = clampedOutput;

        return clampedOutput;
    }
}

[Serializable]
public class ProportionalControl
{
    [SerializeField] private float kp = 10;
    [SerializeField] private float maxOutput = 100;

    public float CalcControlForce(float error)
    {
        var scaled = kp * error;
        var clamped = Mathf.Clamp(scaled, -maxOutput, maxOutput);
        return clamped;
    }
}

[Serializable]
public class SeekControl
{
    [SerializeField] private float kp = 10;
    [SerializeField] private float maxOutput = 100;

    public float MaxOutput => maxOutput;

    public Vector2 CalcControlForce(Vector2 los, Vector2 currentVelocity, float speed)
    {
        var seekVector = kp * SteeringUtil.Seek(los, currentVelocity, speed);
        var scaled = Vector2.ClampMagnitude(seekVector, maxOutput);
        return scaled;
    }
}