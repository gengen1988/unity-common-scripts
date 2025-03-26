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

    public static float Remap(float value, float fromA, float fromB, float toA, float toB)
    {
        var ratio = RemapTo01(value, fromA, fromB);
        var scaledValue = RemapFrom01(ratio, toA, toB);
        return scaledValue;
    }

    public static float RemapClamp(float value, float fromA, float fromB, float toA, float toB)
    {
        var ratio = RemapTo01(value, fromA, fromB);
        var clamped = Mathf.Clamp01(ratio);
        var scaledValue = RemapFrom01(clamped, toA, toB);
        return scaledValue;
    }

    /**
     * remap value from 0 to 1 based to other range.
     * equals Mathf.LerpUnclamped()
     */
    public static float RemapFrom01(float value01, float toA, float toB)
    {
        return toA + (toB - toA) * value01;
    }

    /**
     * like Mathf.InverseLerp() but no clamp
     */
    public static float RemapTo01(float value, float fromA, float fromB)
    {
        return (value - fromA) / (fromB - fromA);
    }

    public static float LerpTimeCorrection(float steepness01)
    {
        return LerpTimeCorrection(steepness01, Time.deltaTime);
    }

    public static float LerpTimeCorrection(float steepness01, float deltaTime)
    {
        // see: https://gamedev.stackexchange.com/questions/149103/why-use-time-deltatime-in-lerping-functions
        const float REFERENCE_FRAME_RATE = 30f;
        var s = Mathf.Clamp01(steepness01);
        return 1 - Mathf.Pow(1 - s, deltaTime * REFERENCE_FRAME_RATE);
    }

    public static float CalcSlope(Vector2 from, Vector2 to)
    {
        var los = to - from;
        return los.y / los.x;
    }

    /**
     * fifth-order version of Mathf.SmoothStep().
     * see: https://en.wikipedia.org/wiki/Smoothstep
     */
    public static float SmootherStep(float from, float to, float t)
    {
        var y = t * t * t * (t * (6 * t - 15) + 10);
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

        var c = 2 / (1 - steepness) - 1;

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
        var sum = Vector3.zero;
        var count = 0;
        foreach (var point in points)
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
        var sum = Vector3.zero;
        var count = 0;
        foreach (var point in points)
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
        var sum = Vector3.zero;
        var count = 0;
        foreach (var point in points)
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
        return points switch
        {
            List<T> list => CenterOfMassList(list, selector),
            T[] array => CenterOfMassArray(array, selector),
            _ => CenterOfMassGeneric(points, selector)
        };
    }

    public static Vector3 VectorByQuaternion(Quaternion rotation)
    {
        return rotation * Vector3.right;
    }

    public static Quaternion QuaternionByVector(Vector3 direction)
    {
        var forward = Vector3.forward;
        var upwards = Quaternion.Euler(0, 0, 90f) * direction;
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
        var rad = degree * Mathf.Deg2Rad;
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
        solution1 = solution2 = float.NaN;

        // if a = 0, then solve bx + c = 0
        if (Mathf.Approximately(a, 0))
        {
            if (Mathf.Approximately(b, 0))
            {
                return 0;
            }
            else
            {
                solution1 = solution2 = -c / b;
                return 1;
            }
        }

        var delta = b * b - 4 * a * c;
        var twoA = 2 * a;

        // one solution (handle -0.0000000001 first)
        if (Mathf.Approximately(delta, 0))
        {
            solution1 = solution2 = -b / twoA;
            return 1;
        }

        // no solution
        if (delta < 0)
        {
            return 0;
        }

        // two solutions
        var root = Mathf.Sqrt(delta);
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
        var ray3D = new Ray(ray.origin, ray.direction);
        return Project(point, ray3D);
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
        var cornerRad = Mathf.Atan2(box.y, box.x);
        var a = Mathf.Tan(lineRad);
        if (lineRad < cornerRad)
        {
            var y = a * box.x;
            return new Vector2(box.x, y);
        }
        else
        {
            var x = box.y / a;
            return new Vector2(x, box.y);
        }
    }

    /**
     * check circle intersection with a ray
     */
    public static int CircleRayIntersect(
        Vector2 center,
        float radius,
        Ray2D ray,
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
        if (Mathf.Approximately(perpendicularLength, radius))
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
        return number switch
        {
            > 0 => 1,
            < 0 => -1,
            _ => 0
        };
    }

    public static Vector2 ChangeVectorMagnitude(
        Vector2 vector,
        float delta,
        float min = 0f,
        float max = float.PositiveInfinity)
    {
        var newMagnitude = vector.magnitude + delta;
        var clamped = Mathf.Clamp(newMagnitude, min, max);
        return vector.normalized * clamped;
    }

    public static Vector3 ChangeVectorMagnitude(
        Vector3 vector,
        float delta,
        float min = 0f,
        float max = float.PositiveInfinity)
    {
        var newMagnitude = vector.magnitude + delta;
        var clamped = Mathf.Clamp(newMagnitude, min, max);
        return vector.normalized * clamped;
    }

    public static float ChangeValueAbs(
        float value,
        float delta,
        float min = 0f,
        float max = float.PositiveInfinity)
    {
        var abs = Mathf.Abs(value);
        var sign = Mathf.Sign(value);
        var clamped = Mathf.Clamp(abs + delta, min, max);
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
            var delta = 1f / count;
            for (var i = 0; i < count; ++i)
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

            var delta = 1f / (count - 1);
            for (var i = 0; i < count; ++i)
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
        var rotation = QuaternionByVector(los);
        var halfAngle = 0.5f * angleRange;
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

        var sum = a + b;
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

    /**
     * similar with Vector2.ClampMagnitude but in 1D space
     */
    public static float ClampAbs(float value, float maxAbs)
    {
        return Mathf.Clamp(value, -maxAbs, maxAbs);
    }

    public static bool IsInAngle(Vector2 los, Vector2 from, float angle)
    {
        // 特殊情况：如果角度大于等于360度或小于等于-360度，返回true
        if (Mathf.Abs(angle) >= 360f)
        {
            return true;
        }

        var angleToOther = Vector2.SignedAngle(from, los); // range: -180 to 180
        if (angle > 180f)
        {
            return angleToOther >= 0f || angleToOther < angle - 360f;
        }
        else if (angle >= 0f && angle <= 180f)
        {
            return angleToOther >= 0f && angleToOther <= angle;
        }
        else if (angle >= -180f && angle < 0f)
        {
            return angleToOther < 0f && angleToOther >= angle;
        }
        else // angle < -180f
        {
            return angleToOther < 0f || angleToOther > angle + 360f;
        }
    }

    public static bool GetBitAt(int number, int index)
    {
        return (number & (1 << index)) > 0;
    }

    public static int GetHighestBit(int number)
    {
        var value = number;
        var count = 0;
        while (value > 0)
        {
            value >>= 1;
            count++;
        }

        return count;
    }

    public static void MoveByForce(ref float position, ref float velocity, float force, float deltaTime)
    {
        var nextVelocity = velocity + force * deltaTime;
        var deltaVelocity = nextVelocity - velocity;
        var deltaPosition = (velocity + 0.5f * deltaVelocity) * deltaTime;

        position += deltaPosition;
        velocity = nextVelocity;
    }

    public static void MoveByForce(ref Vector2 position, ref Vector2 velocity, Vector2 force, float deltaTime)
    {
        var nextVelocity = velocity + force * deltaTime;
        var deltaVelocity = nextVelocity - velocity;
        var deltaPosition = (velocity + 0.5f * deltaVelocity) * deltaTime;

        position += deltaPosition;
        velocity = nextVelocity;
    }

    public static void MoveByForce(ref Vector3 position, ref Vector3 velocity, Vector3 force, float deltaTime)
    {
        var nextVelocity = velocity + force * deltaTime;
        var deltaVelocity = nextVelocity - velocity;
        var deltaPosition = (velocity + 0.5f * deltaVelocity) * deltaTime;

        position += deltaPosition;
        velocity = nextVelocity;
    }
}