using System;
using UnityEngine;

public static class Kinematic
{
	/**
	 * 命中某点的抛物线角度
	 */
	public static bool AngleRequiredToHit2D(Vector2 los, float initialSpeed, out float lowAngle, out float highAngle)
	{
		return AngleRequiredToHit2D(los, initialSpeed, Physics2D.gravity.magnitude, out lowAngle, out highAngle);
	}

	/**
	 * 命中某点的抛物线角度
	 */
	public static bool AngleRequiredToHit2D(Vector2 los, float initialSpeed, float g,
		out float lowAngle,
		out float highAngle)
	{
		var x = los.x;
		var y = los.y;
		var v2 = initialSpeed * initialSpeed;
		var v4 = v2 * v2;
		var x2 = x * x;
		var sqrt = Mathf.Sqrt(v4 - g * (g * x2 + 2 * y * v2));

		if (float.IsNaN(sqrt))
		{
			lowAngle = highAngle = float.NaN;
			return false;
		}

		var solution1 = Mathf.Atan((v2 - sqrt) / (g * x)) * Mathf.Rad2Deg;
		var solution2 = Mathf.Atan((v2 + sqrt) / (g * x)) * Mathf.Rad2Deg;

		if (x == 0)
		{
			if (y > 0)
			{
				highAngle = lowAngle = 90f;
			}
			else
			{
				highAngle = lowAngle = -90f;
			}
		}
		else if (x > 0)
		{
			lowAngle = solution1;
			highAngle = solution2;
		}
		else
		{
			lowAngle = solution1 + 180f;
			highAngle = solution2 + 180f;
		}

		return true;
	}

	/**
	 * 跳到具体高度所需的初始速度
	 */
	public static float JumpVelocity(float height)
	{
		return JumpVelocity(height, Physics2D.gravity.y);
	}

	/**
	 * 跳到具体高度所需的初始速度
	 */
	public static float JumpVelocity(float height, float gravity)
	{
		return Mathf.Sqrt(Mathf.Abs(2 * height * gravity));
	}

	/**
	 * 加速运动的位移
	 */
	public static Vector3 AccelerateDisplacement(Vector3 velocity, Vector3 acceleration, float time)
	{
		return velocity * time + acceleration * (.5f * time * time);
	}

	/**
     * 子弹命中匀速移动物体所需时间
     */
	public static bool InterceptTime(Vector3 los, Vector3 targetVelocity, float interceptSpeed, out float timeRequired)
	{
		var a = targetVelocity.sqrMagnitude - interceptSpeed * interceptSpeed;
		var b = 2f * Vector3.Dot(targetVelocity, los);
		var c = los.sqrMagnitude;

		// no solution, can't intercept
		timeRequired = float.NaN;
		if (MathUtil.Quadratic(a, b, c, out var t0, out var t1) == 0) return false;

		// determine short way
		if (t0 > 0)
		{
			if (t1 > 0)
			{
				timeRequired = Mathf.Min(t0, t1);
				return true;
			}

			timeRequired = t0;
			return true;
		}

		if (t1 <= 0) return false;

		timeRequired = t1;
		return true;
	}
}