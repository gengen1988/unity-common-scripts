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

    public static float Remap(float fromValue, float fromA, float fromB, float toA, float toB)
    {
        float ratio = RemapTo01(fromValue, fromA, fromB);
        float scaledValue = RemapFrom01(ratio, toA, toB);
        return scaledValue;
    }

    public static float RemapClamp(float fromValue, float fromA, float fromB, float toA, float toB)
    {
        float ratio = RemapTo01(fromValue, fromA, fromB);
        float clamped = Mathf.Clamp01(ratio);
        float scaledValue = RemapFrom01(clamped, toA, toB);
        return scaledValue;
    }

    /**
     * remap value from 0 to 1 based to other range.
     * alias of Mathf.LerpUnclamped()
     */
    public static float RemapFrom01(float value01, float a, float b)
    {
        return a + (b - a) * value01;
    }

    /**
     * like Mathf.InverseLerp() but no clamp
     */
    public static float RemapTo01(float value, float a, float b)
    {
        return (value - a) / (b - a);
    }

    public static float LerpTimeCorrection(float steepness)
    {
        return LerpTimeCorrection(steepness, Time.deltaTime);
    }

    public static float LerpTimeCorrection(float steepness, float deltaTime)
    {
        // see: https://gamedev.stackexchange.com/questions/149103/why-use-time-deltatime-in-lerping-functions
        const float REFERENCE_FRAME_RATE = 30f;
        float s = Mathf.Clamp01(steepness);
        return 1 - Mathf.Pow(1 - s, deltaTime * REFERENCE_FRAME_RATE);
    }

    /**
     * fifth-order version of Mathf.SmoothStep().
     */
    public static float SmootherStep(float from, float to, float t)
    {
        // see: https://en.wikipedia.org/wiki/Smoothstep
        float y = t * t * t * (t * (6 * t - 15) + 10);
        return Mathf.Lerp(from, to, y);
    }

    /**
     * a standard sigmoid curve.
     * easier pan than Sigmoid01()
     */
    public static float Logistic(float x, float steepness, float midpoint)
    {
        return 1 / (1 + Mathf.Exp(-steepness * (x - midpoint)));
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

    public static int Mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    public static float Mod(float x, float m)
    {
        return (x % m + m) % m;
    }

    private static Vector3 CenterOfMassList<T>(List<T> points, Func<T, Vector3> selector)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;
        foreach (T point in points)
        {
            if (selector != null)
            {
                sum += selector(point);
            }
            else
            {
                sum += point switch
                {
                    Vector3 v3 => v3,
                    Vector2 v2 => v2,
                    _ => throw new Exception("type mismatch")
                };
            }

            count++;
        }

        return sum / count;
    }

    private static Vector3 CenterOfMassArray<T>(T[] points, Func<T, Vector3> selector)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;
        foreach (T point in points)
        {
            if (selector != null)
            {
                sum += selector(point);
            }
            else
            {
                sum += point switch
                {
                    Vector3 v3 => v3,
                    Vector2 v2 => v2,
                    _ => throw new Exception("type mismatch")
                };
            }

            count++;
        }

        return sum / count;
    }

    private static Vector3 CenterOfMassGeneric<T>(IEnumerable<T> points, Func<T, Vector3> selector)
    {
        Vector3 sum = Vector3.zero;
        int count = 0;
        foreach (T point in points)
        {
            if (selector != null)
            {
                sum += selector(point);
            }
            else
            {
                sum += point switch
                {
                    Vector3 v3 => v3,
                    Vector2 v2 => v2,
                    _ => throw new Exception("type mismatch")
                };
            }

            count++;
        }

        return sum / count;
    }

    public static Vector3 CenterOfMass<T>(this IEnumerable<T> points, Func<T, Vector3> selector = null)
    {
        switch (points)
        {
            case List<T> list:
                return CenterOfMassList(list, selector);
            case T[] array:
                return CenterOfMassArray(array, selector);
            default:
                // this has gc alloc for IEnumerable
                return CenterOfMassGeneric(points, selector);
        }
    }

    public static Vector3 VectorByQuaternion(Quaternion rotation)
    {
        return rotation * Vector3.right;
    }

    public static Quaternion QuaternionByVector(Vector3 direction)
    {
        Vector3 forward = Vector3.forward;
        Vector3 upwards = Quaternion.AngleAxis(90f, forward) * direction;
        return Quaternion.LookRotation(forward, upwards);
    }

    public static float AngleByQuaternion(Quaternion rotation)
    {
        return Mathf.Repeat(rotation.eulerAngles.z, 360f) - 180f;
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

    public static Quaternion DeltaQuaternion(Quaternion from, Quaternion to)
    {
        return to * Quaternion.Inverse(from);
    }

    public static float FixedPointRound(float value, int digits = 5)
    {
        return (float)Math.Round(value, digits);
    }

    /**
     * solve axx + bx + c = 0
     */
    public static int Quadratic(float a, float b, float c, out float solution1, out float solution2)
    {
        // when a = 0, bx + c = 0
        if (Mathf.Approximately(a, 0))
        {
            if (Mathf.Approximately(b, 0))
            {
                solution1 = solution2 = float.NaN;
                return 0;
            }
            else
            {
                solution1 = solution2 = -c / b;
                return 1;
            }
        }

        float delta = b * b - 4 * a * c;
        float twoA = 2 * a;

        // one solution (handle -0.0000000001 first)
        if (Mathf.Approximately(delta, 0))
        {
            solution1 = solution2 = -b / twoA;
            return 1;
        }

        // no solution
        if (delta < 0)
        {
            solution1 = solution2 = float.NaN;
            return 0;
        }

        // two solutions
        float root = Mathf.Sqrt(delta);
        solution1 = (-b + root) / twoA;
        solution2 = (-b - root) / twoA;
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

    public static Vector2 LineBoxIntersect(float lineRad, Vector2 box)
    {
        float cornerRad = Mathf.Atan2(box.y, box.x);
        float a = Mathf.Tan(lineRad);
        if (lineRad < cornerRad)
        {
            float y = a * box.x;
            return new Vector2(box.x, y);
        }
        else
        {
            float x = box.y / a;
            return new Vector2(x, box.y);
        }
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

    public static Vector3 ChangeVectorMagnitude(
        Vector3 vector,
        float delta,
        float min = 0f,
        float max = float.PositiveInfinity)
    {
        float newMagnitude = vector.magnitude + delta;
        float clamped = Mathf.Clamp(newMagnitude, min, max);
        return vector.normalized * clamped;
    }

    public static float ChangeValueAbs(
        float value,
        float delta,
        float min = 0f,
        float max = float.PositiveInfinity)
    {
        float abs = Mathf.Abs(value);
        float sign = Mathf.Sign(value);
        float clamped = Mathf.Clamp(abs + delta, min, max);
        return sign * clamped;
    }

    /**
     * positive values are counter-clockwise
     */
    public static Vector2 Rotate(Vector2 vector, float degree)
    {
        return Quaternion.Euler(0, 0, degree) * vector;
    }

    public static bool IsNaN(Vector2 vector)
    {
        return float.IsNaN(vector.x) || float.IsNaN(vector.y);
    }

    public static bool IsNaN(Vector3 vector)
    {
        return float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z);
    }

    /**
     * generate a sequence of evenly spaced number from 0 to 1.
     */
    public static IEnumerable<float> Progress01(int count, bool spaceAround = false)
    {
        if (count <= 0)
        {
            yield break;
        }

        if (spaceAround)
        {
            float delta = 1f / count;
            for (int i = 0; i < count; ++i)
            {
                yield return (i + 0.5f) * delta;
            }
        }
        else
        {
            if (count == 1)
            {
                yield return 0.5f;
                yield break;
            }

            float delta = 1f / (count - 1);
            for (int i = 0; i < count; ++i)
            {
                yield return i * delta;
            }
        }
    }

    public static IEnumerable<float> Exponential(int count, float from, float to, float factor = 1f)
    {
        return Progress01(count)
            .Select(t => Mathf.Pow(t, factor))
            .Select(t => Mathf.Lerp(from, to, t));
    }

    public static IEnumerable<Vector3> FormationLine(
        int count,
        Vector3 from,
        Vector3 to,
        bool spaceAround = false)
    {
        return Progress01(count, spaceAround)
            .Select(t => Vector3.Lerp(from, to, t));
    }

    /**
     * spaceAround set to true when drawing a circle with angle 360
     */
    public static IEnumerable<Vector3> FormationArc(
        int count,
        Vector3 center,
        float radius,
        Vector3 los,
        float angleRange = 180f,
        bool spaceAround = false)
    {
        Quaternion rotation = QuaternionByVector(los);
        float halfAngle = 0.5f * angleRange;
        return Progress01(count, spaceAround)
            .Select(t => Mathf.Lerp(-halfAngle, halfAngle, t))
            .Select(VectorByAngle)
            .Select(direction => center + rotation * direction * radius);
    }

    /**
     * turn inverse values in to one way, leave sum no changes
     */
    public static void BalanceValues(ref float a, ref float b)
    {
        if (Mathf.Approximately(Mathf.Sign(a), Mathf.Sign(b)))
        {
            return;
        }

        float sum = a + b;
        if (Mathf.Abs(a) > Mathf.Abs(b))
        {
            a = sum;
            b = 0;
        }
        else if (Mathf.Abs(a) < Mathf.Abs(b))
        {
            a = 0;
            b = sum;
        }
    }

    public static void WeightedSum()
    {
        throw new NotImplementedException();
    }
}