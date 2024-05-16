using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MathUtil
{
    public static float Acot(float f)
    {
        return 0.5f * Mathf.PI - Mathf.Atan(f);
    }

    public static float Scale(float value1, float from1, float to1, float from2, float to2)
    {
        // Prevent division by zero
        if (Math.Abs(from1 - to1) < Mathf.Epsilon)
        {
            return value1;
        }

        float normalizedValue = (value1 - from1) / (to1 - from1);
        float scaledValue = normalizedValue * (to2 - from2) + from2;
        return scaledValue;
    }

    public static int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public static float Mod(float x, float m)
    {
        return (x % m + m) % m;
    }

    public static Vector3 CenterOfMass(IList<Vector3> points)
    {
        return points.Aggregate(Vector3.zero, (current, sum) => current + sum) / points.Count;
    }

    public static Vector3 VectorByQuaternion(Quaternion rotation)
    {
        return rotation * Vector3.right;
    }

    public static Quaternion QuaternionByVector(Vector3 direction)
    {
        return Quaternion.FromToRotation(Vector3.right, direction);
    }

    public static float AngleByQuaternion(Quaternion rotation)
    {
        return rotation.eulerAngles.z;
    }

    public static Quaternion QuaternionByAngle(float degree)
    {
        return Quaternion.AngleAxis(degree, Vector3.forward);
    }

    public static Vector2 VectorByAngle(float degree)
    {
        float rad = degree * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }

    public static float AngleByVector(Vector2 direction)
    {
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    /**
     * resolve axx + bx + c = 0
     */
    public static int Quadratic(float a, float b, float c, out float solution1, out float solution2)
    {
        float amount = b * b - 4 * a * c;

        // no solution
        if (amount < 0)
        {
            solution1 = solution2 = float.NaN;
            return 0;
        }

        // one solution
        if (amount == 0)
        {
            solution1 = solution2 = -.5f * b / a;
            return 1;
        }

        // two solutions
        float root = Mathf.Sqrt(amount);
        solution1 = (-b + root) / (2 * a);
        solution2 = (-b - root) / (2 * a);
        // if (solution2 < solution1)
        // {
        //     float temp = solution1;
        //     solution1 = solution2;
        //     solution2 = temp;
        // }
        return 2;
    }

    public static Vector2 Project(Vector2 point, Ray2D ray)
    {
        return (Vector2)Vector3.Project(point - ray.origin, ray.direction) + ray.origin;
    }

    public static Vector3 Project(Vector3 point, Ray ray)
    {
        return Vector3.Project(point - ray.origin, ray.direction) + ray.origin;
    }

    public static Vector3 Project(Vector3 point, Vector3 from, Vector3 to)
    {
        return Vector3.Project(point - from, to - from) + from;
    }

    public static Vector3 Mirror(Vector3 point, Vector3 from, Vector3 to)
    {
        Vector3 projection = Project(point, from, to);
        Vector3 perpendicular = projection - point;
        return point + 2 * perpendicular;
    }

    /**
     * return false if projection point is out of line
     */
    public static bool IsPointInRange(Vector3 point, Vector3 from, Vector3 to)
    {
        if (Vector3.Dot(point - from, to - from) < 0)
        {
            return false;
        }

        if (Vector3.Dot(point - to, from - to) < 0)
        {
            return false;
        }

        return true;
    }

    public static bool LineIntersect(Vector2 from1, Vector2 to1, Vector2 from2, Vector2 to2, out Vector2 point)
    {
        float d1 = Vector3.Cross(from1 - from2, to2 - from2).z;
        float d2 = Vector3.Cross(to1 - from2, to2 - from2).z;

        // parallel check
        if (Mathf.Abs(d1 - d2) <= Mathf.Epsilon)
        {
            point = new Vector2(float.NaN, float.NaN);
            return false;
        }

        point = (d1 * to1 - d2 * from1) / (d1 - d2);

        if (!IsPointInRange(point, from1, to1))
        {
            return false;
        }

        if (!IsPointInRange(point, from2, to2))
        {
            return false;
        }

        return true;
    }

    /**
     * check circle intersection with a ray
     */
    public static int CircleRayIntersection(
        Vector2 center,
        float radius,
        Ray2D ray,
        out Vector2 point1,
        out Vector2 point2)
    {
        Vector2 perpendicular = Project(center, ray);
        Vector2 perpendicularToCircle = perpendicular - center;
        float perpendicularLength = perpendicularToCircle.magnitude;

        // too far
        if (perpendicularLength > radius)
        {
            point1 = point2 = new Vector2(float.NaN, float.NaN);
            return 0;
        }

        // just fit
        if (Math.Abs(perpendicularLength - radius) <= Mathf.Epsilon)
        {
            point1 = point2 = perpendicular;
            return 1;
        }

        // two point
        float alpha = Mathf.Asin(perpendicularLength / radius);
        float omegaDegree = 90 - alpha * Mathf.Rad2Deg;
        Vector2 direction1 = Rotate(perpendicularToCircle, omegaDegree);
        Vector2 direction2 = Rotate(perpendicularToCircle, -omegaDegree);

        point1 = center + direction1.normalized * radius;
        point2 = center + direction2.normalized * radius;
        return 2;
    }

    public static int SignWithZero(float number)
    {
        if (Mathf.Abs(number) <= Mathf.Epsilon)
        {
            return 0;
        }
        else if (number > 0)
        {
            return 1;
        }
        else if (number < 0)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    public static Vector3 VectorSubtractClamp(Vector3 vector, float magnitude)
    {
        return vector.normalized * Mathf.Max(vector.magnitude - magnitude, 0f);
    }

    public static Vector2 Rotate(Vector2 vector, float degrees)
    {
        return QuaternionByAngle(degrees) * vector;
    }

    public static bool IsNaN(Vector2 vector)
    {
        return float.IsNaN(vector.x) || float.IsNaN(vector.y);
    }

    public static bool IsNaN(Vector3 vector)
    {
        return float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z);
    }

    public static IEnumerable<float> ExponentialCurve(float min, float max, int levels, float factor = 1)
    {
        float delta = max - min;
        for (int i = 0; i < levels; ++i)
        {
            yield return Mathf.Pow((float)i / (levels - 1), factor) * delta + min;
        }
    }

    public static IEnumerable<Vector3> PointsOnLine(int count, Vector3 from, Vector3 to)
    {
        Vector3 line = to - from;
        float deltaX = line.x / (count - 1);
        float deltaY = line.y / (count - 1);
        for (int i = 0; i < count; ++i)
        {
            float x = from.x + deltaX * i;
            float y = from.y + deltaY * i;
            yield return new Vector3(x, y);
        }
    }

    public static void BalanceValues(ref float a, ref float b)
    {
        if (Math.Abs(Mathf.Sign(a) - Mathf.Sign(b)) < Mathf.Epsilon)
        {
            return;
        }

        float c = a + b;
        if (Mathf.Abs(a) > Mathf.Abs(b))
        {
            a = c;
            b = 0;
        }
        else if (Math.Abs(a) < Math.Abs(b))
        {
            a = 0;
            b = c;
        }
    }
}