using UnityEngine;

public class DebugSinMove : MonoBehaviour
{
    public float Amplitude = 5;
    public float Period = 2f;
    public float PhaseOctave;
    public float Angle;

    private float _elapsedTime;

    private void Update()
    {
        var deltaTime = Time.deltaTime;
        var phase = PhaseOctave * Mathf.PI * 2;
        var localDisplacement = CalculateDisplacement(_elapsedTime, deltaTime, phase);
        var rotation = MathUtil.QuaternionByAngle(Angle) * transform.rotation;
        var displacement = rotation * localDisplacement;
        transform.position += displacement;
        _elapsedTime = MathUtil.Mod(_elapsedTime + deltaTime, Period);
    }

    private Vector3 CalculateDisplacement(float time, float deltaTime, float phase)
    {
        var v1 = MathUtil.Remap(time, 0, Period, 0, 2 * Mathf.PI);
        var v2 = MathUtil.Remap(time + deltaTime, 0, Period, 0, 2 * Mathf.PI);
        var delta = Mathf.Sin(v2 + phase) - Mathf.Sin(v1 + phase);
        var v = new Vector3(0, delta);
        return Amplitude * v;
    }
}