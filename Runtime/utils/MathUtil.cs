using System;
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

	public static Vector3 QuaternionToVector(Quaternion rotation)
	{
		return rotation * Vector3.right;
	}

	public static Quaternion VectorToQuaternion(Vector3 direction)
	{
		return Quaternion.FromToRotation(Vector3.right, direction);
	}

	public static float QuaternionToAngle(Quaternion rotation)
	{
		return rotation.eulerAngles.z;
	}

	public static Quaternion AngleToQuaternion(float degree)
	{
		return Quaternion.AngleAxis(degree, Vector3.forward);
	}

	public static Vector2 AngleToVector(float degree)
	{
		var rad = degree * Mathf.Deg2Rad;
		return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
	}

	public static float VectorToAngle(Vector2 direction)
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

	public static Vector2 PerpendicularToPoint(Vector2 lineOrigin, Vector2 lineDirection, Vector2 point)
	{
		var los = point - lineOrigin;
		Vector2 projection = Vector3.Project(los, lineDirection);
		return lineOrigin + projection;
	}

	public static Vector2 PerpendicularToPoint(Ray2D ray, Vector2 point)
	{
		var los = point - ray.origin;
		Vector2 projection = Vector3.Project(los, ray.direction);
		return ray.origin + projection;
	}

	/**
	 * check circle intersection with ray
	 */
	public static int CircleRayIntersection(Vector2 center, float radius, Ray2D ray,
		out Vector2 point1,
		out Vector2 point2)
	{
		var perpendicular = PerpendicularToPoint(ray, center);
		var perpendicularToCircle = perpendicular - center;
		var perpendicularLength = perpendicularToCircle.magnitude;

		// too far
		if (perpendicularLength > radius)
		{
			point1 = point2 = new Vector2(float.NaN, float.NaN);
			return 0;
		}

		// just fit
		if (Math.Abs(perpendicularLength - radius) < Mathf.Epsilon)
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
		if (number > 0) return 1;
		if (number < 0) return -1;
		return 0;
	}

	public static Vector3 VectorSubtractClamp(Vector3 vector, float magnitude)
	{
		return vector.normalized * Mathf.Max(vector.magnitude - magnitude, 0f);
	}

	public static Vector2 Rotate(Vector2 vector, float degrees)
	{
		return AngleToQuaternion(degrees) * vector;
	}

	public static bool IsNaN(this Vector3 vector)
	{
		return float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z);
	}

	public static IEnumerable<float> GenerateExponentialCurve(float min, float max, int levels, float factor = 1)
	{
		float delta = max - min;
		for (int i = 0; i < levels; ++i)
		{
			yield return Mathf.Pow((float)i / (levels - 1), factor) * delta + min;
		}
	}

	public static IEnumerable<Vector3> SliceLine(Vector3 from, Vector3 to, int count)
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
}