using UnityEngine;
using UnityEngine.Assertions;

public class SwingMovement : MonoBehaviour
{
    public Vector2 Amplitude = new Vector2(0, 5);
    public float PhaseDelta = 0.5f;
    public float Period = 2f;

    private float _elapsedTime;
    private Rigidbody2D _rb;
    private Vector3 _startPosition;

    private void OnDrawGizmosSelected()
    {
        if (Period <= 0)
        {
            return;
        }

        Vector2 previousPosition = transform.position;
        float phaseOffset = PhaseDelta * Mathf.PI;
        if (Application.isPlaying)
        {
            previousPosition = _startPosition;
        }

        float step = Period / 20f; // Divide the period into 20 steps for smoother curve
        float accumulatedTime = 0f;
        Gizmos.color = Color.red; // Initial color for the first segment
        for (float t = 0; t <= Period; t += step)
        {
            Vector2 nextPosition = previousPosition + CalculateDisplacement(t, step, phaseOffset);
            if (accumulatedTime > 1f)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(previousPosition, nextPosition);
            }
            else
            {
                Gizmos.DrawLine(previousPosition, nextPosition);
            }

            accumulatedTime += step;
            previousPosition = nextPosition;
        }
    }

    private void Awake()
    {
        TryGetComponent(out _rb);
        Assert.IsNotNull(_rb, "movement should has rigidbody");
    }

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void FixedUpdate()
    {
        float phaseOffset = PhaseDelta * Mathf.PI;
        Vector2 displacement = CalculateDisplacement(_elapsedTime, Time.deltaTime, phaseOffset);
        _rb.MovePosition(_rb.position + displacement);
        _elapsedTime = MathUtil.Mod(_elapsedTime + Time.deltaTime, Period);
    }

    private Vector2 CalculateDisplacement(float time, float deltaTime, float phaseOffset)
    {
        float v1 = MathUtil.Remap(time,             0, Period, 0, 2 * Mathf.PI);
        float v2 = MathUtil.Remap(time + deltaTime, 0, Period, 0, 2 * Mathf.PI);
        float deltaX = Amplitude.x * (Mathf.Sin(v2 + phaseOffset) - Mathf.Sin(v1 + phaseOffset));
        float deltaY = Amplitude.y * (Mathf.Sin(v2) - Mathf.Sin(v1));
        return new Vector2(deltaX, deltaY);
    }
}