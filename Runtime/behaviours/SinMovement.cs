using UnityEngine;

public class SinMovement : MonoBehaviour
{
    public float Amplitude = 5;
    public float Period = 2f;
    public float PhaseOctave;
    public float Angle;

    private float _elapsedTime;
    private Rigidbody2D _rb;
    private BlendMovement _blend;

    private void Awake()
    {
        TryGetComponent(out _rb);
        TryGetComponent(out _blend);
    }

    private void FixedUpdate()
    {
        float phase = PhaseOctave * Mathf.PI;
        Vector2 localDisplacement = CalculateDisplacement(_elapsedTime, Time.deltaTime, phase);
        Quaternion rotation = MathUtil.QuaternionByAngle(Angle) * transform.rotation;
        Vector2 displacement = rotation * localDisplacement;

        if (_blend)
        {
            _blend.AddDisplacement(displacement);
        }
        else
        {
            _rb.MovePosition(_rb.position + displacement);
        }

        _elapsedTime = MathUtil.Mod(_elapsedTime + Time.deltaTime, Period);
    }

    private Vector2 CalculateDisplacement(float time, float deltaTime, float phase)
    {
        float v1 = MathUtil.Remap(time, 0, Period, 0, 2 * Mathf.PI);
        float v2 = MathUtil.Remap(time + deltaTime, 0, Period, 0, 2 * Mathf.PI);
        float delta = Mathf.Sin(v2 + phase) - Mathf.Sin(v1 + phase);
        Vector2 v = new Vector2(0, delta);
        return Amplitude * v;
    }
}