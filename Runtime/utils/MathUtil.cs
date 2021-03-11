using System.Linq;
using UnityEngine;


public static class MathUtil
{
    public static bool IsNaN(this Vector3 vector)
    {
        return float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z);
    }

    public static Vector3 CenterOfMass(Vector3[] points)
    {
        return points.Aggregate(Vector3.zero, (current, point) => current + point) / points.Length;
    }

    public static Vector3 RotationToDirection(Quaternion rotation)
    {
        return rotation * Vector3.right;
    }

    public static Quaternion AngleToRotation(float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public static Quaternion DirectionToRotation(Vector3 direction)
    {
        return Quaternion.FromToRotation(Vector3.right, direction);
    }

    public static float DirectionToAngle(Vector2 direction)
    {
        return Vector2.SignedAngle(Vector2.right, direction);
    }

    public static Vector2 Rotate(this Vector2 los, Quaternion rotation)
    {
        return rotation * Vector2.right * los.magnitude;
    }

    // 一元二次方程
    public static bool Quadratic(float a, float b, float c, out float[] result)
    {
        result = default;
        var sqrt = Mathf.Sqrt(b * b - 4 * a * c);
        if (float.IsNaN(sqrt)) return false;
        result = new[]
        {
            (-b + sqrt) / (2 * a),
            (-b - sqrt) / (2 * a)
        };
        return true;
    }

    public static int SignWithZero(float number)
    {
        if (number > 0) return 1;
        if (number < 0) return -1;
        return 0;
    }
}