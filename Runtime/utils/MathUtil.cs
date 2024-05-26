﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MathUtil
{
    public static float Acot(float f)
    {
        return 0.5f * Mathf.PI - Mathf.Atan(f);
    }

    public static float Remap(float fromValue, float fromA, float fromB, float toA, float toB)
    {
        // Prevent division by zero
        if (Mathf.Approximately(fromA, fromB))
        {
            return fromValue;
        }

        float ratio = (fromValue - fromA) / (fromB - fromA); // inverse lerp from range 1
        float scaledValue = toA + (toB - toA) * ratio; // lerp to range 2
        return scaledValue;
    }

    /**
     * remap value from 0 to 1 based to other range。
     * alias for Mathf.LerpUnclamped
     */
    public static float Remap01(float value01, float a, float b)
    {
        return Mathf.LerpUnclamped(a, b, value01);
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
     * resolve axx + bx + c = 0.
     * solution1 will be smaller
     */
    public static int Quadratic(float a, float b, float c, out float solution1, out float solution2)
    {
        float amount = b * b - 4 * a * c;

        // one solution
        if (Mathf.Approximately(amount, 0))
        {
            solution1 = solution2 = -.5f * b / a;
            return 1;
        }

        // no solution
        if (amount < 0)
        {
            solution1 = solution2 = float.NaN;
            return 0;
        }

        // two solutions
        float root = Mathf.Sqrt(amount);
        solution1 = (-b + root) / (2 * a);
        solution2 = (-b - root) / (2 * a);
        return 2;
    }

    public static Vector3 Project(Vector3 point, Ray ray)
    {
        return Vector3.Project(point - ray.origin, ray.direction) + ray.origin;
    }

    public static Vector2 Project(Vector2 point, Ray2D ray)
    {
        Ray ray3D = new Ray(ray.origin, ray.direction);
        return Project(point, ray3D);
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
        if (Mathf.Approximately(d1, d2))
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
        if (Mathf.Approximately(perpendicularLength, radius))
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
        if (Mathf.Approximately(number, 0))
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

    public static IEnumerable<float> ExponentialCurve(float min, float max, int levels, float factor = 1f)
    {
        float range = max - min;
        for (int i = 0; i < levels; ++i)
        {
            float ratio = Mathf.Pow((float)i / (levels - 1), factor);
            yield return min + ratio * range;
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

    /**
     * a standard sigmoid curve
     */
    public static float Logistic(float x, float height, float steepness, float midpoint)
    {
        return height / (1 + Mathf.Exp(-steepness * (x - midpoint)));
    }

    /**
     * credit to Jason Hise: https://www.jfurness.uk/sigmoid-functions-in-game-design/
     */
    public static float Sigmoid01(float x, float steepness = 0.5f, float midpoint = 0.5f)
    {
        x = Mathf.Clamp01(x);
        midpoint = Mathf.Clamp01(midpoint);
        steepness = Mathf.Clamp01(steepness);

        float c = 2 / (1 - steepness) - 1;

        if (x < midpoint)
        {
            return Mathf.Pow(x, c) / Mathf.Pow(midpoint, c - 1);
        }
        else
        {
            return 1 - Mathf.Pow(1 - x, c) / Mathf.Pow(1 - midpoint, c - 1);
        }
    }

    // public static void BalanceValues(ref float a, ref float b)
    // {
    //     if (Math.Abs(Mathf.Sign(a) - Mathf.Sign(b)) < Epsilon)
    //     {
    //         return;
    //     }
    //
    //     float c = a + b;
    //     if (Mathf.Abs(a) > Mathf.Abs(b))
    //     {
    //         a = c;
    //         b = 0;
    //     }
    //     else if (Math.Abs(a) < Math.Abs(b))
    //     {
    //         a = 0;
    //         b = c;
    //     }
    // }
}