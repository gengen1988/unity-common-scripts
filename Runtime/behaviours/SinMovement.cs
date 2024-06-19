using UnityEngine;

public class SinMovement : MonoBehaviour, IMovement
{
    public float Amplitude = 5;
    public float Period = 2f;
    public float PhaseOctave;
    public float Angle;

    private float _elapsedTime;
    private BlendMovement _blend;

    private void Awake()
    {
        _blend = GetComponentInParent<BlendMovement>();
    }

    public void Tick(float deltaTime)
    {
        float phase = PhaseOctave * Mathf.PI * 2;
        Vector3 localDisplacement = CalculateDisplacement(_elapsedTime, deltaTime, phase);
        Quaternion rotation = MathUtil.QuaternionByAngle(Angle) * _blend.GetRotation();
        Vector3 displacement = rotation * localDisplacement;
        Vector3 position = _blend.GetPosition();
        _blend.MovePosition(position + displacement);
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