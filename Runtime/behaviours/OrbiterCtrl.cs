using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        float nextAngle = _currentAngle - TurnSpeed * deltaTime;
        _currentAngle = MathUtil.Mod(nextAngle, 360f);
        Vector2 los = MathUtil.VectorByAngle(_currentAngle);

        int index = 0;
        foreach (Vector3 p in MathUtil.FormationArc(Segments.Length, transform.position, Radius, los, 360, true))
        {
            Transform trans = Segments[index];
            if (trans)
            {
                trans.MovePosition(p);
            }

            index++;
        }
    }
}