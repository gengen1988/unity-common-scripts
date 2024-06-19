using UnityEngine;

[DefaultExecutionOrder(100)]
public class OrbiterCtrl : MonoBehaviour
{
    public float Radius = 5f;
    public float TurnSpeed;
    public Transform[] Segments;

    private float _currentAngle;

    private void FixedUpdate()
    {
        Tick(Time.deltaTime);
    }

    private void Tick(float deltaTime)
    {
        // predict follow target
        TryGetComponent(out Rigidbody2D rbSelf);
        Vector2 predictedPosition = rbSelf.position + rbSelf.velocity * deltaTime;

        // rotate
        float nextAngle = _currentAngle - TurnSpeed * deltaTime;
        _currentAngle = MathUtil.Mod(nextAngle, 360f);
        Vector2 los = MathUtil.VectorByAngle(_currentAngle);

        // setup formation
        int index = 0;
        foreach (Vector3 pos in MathUtil.FormationArc(Segments.Length, predictedPosition, Radius, los, 360, true))
        {
            Transform segment = Segments[index];
            segment.TryGetComponent(out Rigidbody2D rb);
            rb.MovePosition(pos);

            index++;
        }
    }
}