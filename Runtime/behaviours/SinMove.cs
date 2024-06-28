using UnityEngine;

public class SinMove : MonoBehaviour, IMoveHandler
{
    public float Amplitude = 5;
    public float Period = 2f;
    public float PhaseOctave;
    public float Angle;

    private float _elapsedTime;

    public void OnMove(IMoveSubject subject, float deltaTime)
    {
        float phase = PhaseOctave * Mathf.PI * 2;
        Vector3 localDisplacement = CalculateDisplacement(_elapsedTime, deltaTime, phase);
        Quaternion rotation = MathUtil.QuaternionByAngle(Angle) * subject.GetRotation();
        Vector3 displacement = rotation * localDisplacement;
        subject.MovePositionDelta(displacement);
        _elapsedTime = MathUtil.Mod(_elapsedTime + deltaTime, Period);
    }

    private Vector3 CalculateDisplacement(float time, float deltaTime, float phase)
    {
        float v1 = MathUtil.Remap(time, 0, Period, 0, 2 * Mathf.PI);
        float v2 = MathUtil.Remap(time + deltaTime, 0, Period, 0, 2 * Mathf.PI);
        float delta = Mathf.Sin(v2 + phase) - Mathf.Sin(v1 + phase);
        Vector3 v = new Vector3(0, delta);
        return Amplitude * v;
    }
}