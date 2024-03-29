﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MathUtil
{
    public static int Mod(int x, int m)
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
        var rad = degree * Mathf.Deg2Rad;
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
        var amount = b * b - 4 * a * c;

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
        var root = Mathf.Sqrt(amount);
        solution1 = (-b + root) / (2 * a);
        solution2 = (-b - root) / (2 * a);
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
        var projection = Project(point, from, to);
        var perpendicular = projection - point;
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
        var d1 = Vector3.Cross(from1 - from2, to2 - from2).z;
        var d2 = Vector3.Cross(to1 - from2, to2 - from2).z;
        if (Mathf.Abs(d1 - d2) <= Mathf.Epsilon)
        {
            point = new Vector2(float.NaN, float.NaN);
            return false; // parallel
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
    public static int CircleRayIntersection(Vector2 center, float radius, Ray2D ray,
        out Vector2 point1,
        out Vector2 point2)
    {
        var perpendicular = Project(center, ray);
        var perpendicularToCircle = perpendicular - center;
        var perpendicularLength = perpendicularToCircle.magnitude;

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
        var alpha = Mathf.Asin(perpendicularLength / radius);
        var omegaDegree = 90 - alpha * Mathf.Rad2Deg;
        var direction1 = Rotate(perpendicularToCircle, omegaDegree);
        var direction2 = Rotate(perpendicularToCircle, -omegaDegree);

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
        var delta = max - min;
        for (var i = 0; i < levels; ++i)
        {
            yield return Mathf.Pow((float)i / (levels - 1), factor) * delta + min;
        }
    }

    public static IEnumerable<Vector3> PointsOnLine(int count, Vector3 from, Vector3 to)
    {
        var line = to - from;
        var deltaX = line.x / (count - 1);
        var deltaY = line.y / (count - 1);
        for (var i = 0; i < count; ++i)
        {
            var x = from.x + deltaX * i;
            var y = from.y + deltaY * i;
            yield return new Vector3(x, y);
        }
    }

    public static void BalanceValues(ref float a, ref float b)
    {
        if (Math.Abs(Mathf.Sign(a) - Mathf.Sign(b)) < Mathf.Epsilon)
        {
            return;
        }

        var c = a + b;
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