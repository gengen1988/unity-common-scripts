using UnityEngine;

public static class MathUtil
{
    public static Quaternion AngleToRotation(float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public static Quaternion DirectionToRotation(Vector2 direction)
    {
        return Quaternion.FromToRotation(Vector2.right, direction);
    }

    public static float DirectionToAngle(Vector2 direction)
    {
        return Vector2.SignedAngle(Vector2.right, direction);
    }

    public static Vector2 Rotate(this Vector2 vector2, Quaternion rotation)
    {
        return rotation * Vector2.right * vector2.magnitude;
    }

    public static Vector2 ClampMagnitude(this Vector2 vector2, float maxMagnitude)
    {
        return vector2.magnitude > maxMagnitude ? vector2.normalized * maxMagnitude : vector2;
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