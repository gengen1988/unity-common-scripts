using UnityEngine;

public class ActorSinMove2 : MonoBehaviour
{
    public float Amplitude = 5;
    public float Period = 2f;
    public float PhaseOctave;
    public float Angle;

    private float _elapsedTime;
    private Rigidbody2D _rb;

    private void Awake()
    {
        TryGetComponent(out _rb);
        TryGetComponent(out Actor actor);
        actor.OnMove += HandleMove;
    }

    private void HandleMove(Actor moveSubject)
    {
        float phase = PhaseOctave * Mathf.PI * 2;
        float localDeltaTime = moveSubject.Timer.LocalDeltaTime;
        Vector3 localDisplacement = CalculateDisplacement(_elapsedTime, localDeltaTime, phase);
        Quaternion rotation = MathUtil.QuaternionByAngle(Angle) * moveSubject.transform.rotation;
        Vector3 displacement = rotation * localDisplacement;
        _rb.MovePosition(displacement);
        _elapsedTime = MathUtil.Mod(_elapsedTime + localDeltaTime, Period);
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