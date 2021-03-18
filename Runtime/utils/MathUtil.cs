using System;
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

    public static Vector2 Rotate(this Vector2 los, float degrees)
    {
        return AngleToRotation(degrees) * los;
    }

    // solutions for: axx + bx + c = 0
    public static int Quadratic(float a, float b, float c, out float solution1, out float solution2)
    {
        solution1 = default;
        solution2 = default;

        // no solution
        var amount = b * b - 4 * a * c;
        if (amount < 0) return 0;

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

    public static Vector2 PerpendicularToPoint(Vector2 lineOrigin, Vector2 lineDirection, Vector2 point)
    {
        var los = point - lineOrigin;
        Vector2 projection = Vector3.Project(los, lineDirection);
        return lineOrigin + projection;
    }

    public static int CircleRayIntersection(Vector2 circleCenter, float circleRadius, Ray ray, out Vector2 result1, out Vector2 result2)
    {
        result1 = result2 = default;

        var perpendicular = PerpendicularToPoint(ray.origin, ray.direction, circleCenter);

        // too far
        var perpendicularToCircle = perpendicular - circleCenter;
        var perpendicularLenght = perpendicularToCircle.magnitude;
        if (perpendicularLenght > circleRadius) return 0;

        // just fit
        if (Math.Abs(perpendicularLenght - circleRadius) < Mathf.Epsilon)
        {
            result1 = result2 = perpendicular;
            return 1;
        }

        // two point
        var alpha = Mathf.Asin(perpendicularLenght / circleRadius);
        var omegaDegree = 90 - alpha * Mathf.Rad2Deg;

        var direction1 = perpendicularToCircle.Rotate(omegaDegree);
        var direction2 = perpendicularToCircle.Rotate(-omegaDegree);

        result1 = circleCenter + direction1.normalized * circleRadius;
        result2 = circleCenter + direction2.normalized * circleRadius;

        return 2;
    }

    public static int SignWithZero(float number)
    {
        if (number > 0) return 1;
        if (number < 0) return -1;
        return 0;
    }

    public static Vector2 AngleToDirection(float angleDegree)
    {
        return new Vector2(Mathf.Cos(angleDegree * Mathf.Deg2Rad), Mathf.Sin(angleDegree * Mathf.Deg2Rad));
    }

    public static float RotationToAngle(Quaternion rotation) => rotation.eulerAngles.z;
}