using UnityEngine;

public static class SteeringUtil
{
	public static Vector3 Seek(Vector3 los, Vector3 velocity, float maxSpeed)
	{
		var desiredVelocity = los.normalized * maxSpeed;
		return desiredVelocity - velocity;
	}

	public static Vector3 Flee(Vector3 los, Vector3 velocity, float maxSpeed, float evadeDistance)
	{
		if (los.magnitude > evadeDistance)
		{
			return Vector3.zero;
		}

		return -Seek(los, velocity, maxSpeed);
	}

	public static Vector3 Arrive(Vector3 los, Vector3 velocity, float maxSpeed, float arrivalDistance)
	{
		var currentDistance = los.magnitude;
		if (currentDistance > arrivalDistance)
		{
			return Seek(los, velocity, maxSpeed);
		}

		var arrivalRatio = currentDistance / arrivalDistance;
		return Seek(los, velocity, maxSpeed * arrivalRatio);
	}

	public static Vector3 Pursue(Vector3 los, Vector3 velocity, Vector3 evaderVelocity, float maxSpeed)
	{
		if (!Kinematic.InterceptTime(los, evaderVelocity, maxSpeed, out var timeRequired))
		{
			return Vector3.zero;
		}

		var targetLos = los + evaderVelocity * timeRequired;
		return Seek(targetLos, velocity, maxSpeed);
	}

	public static Vector3 Wander(Transform transform, float wanderDistance, float wanderRadius, float wanderJitter,
		ref Vector3 wanderTarget)
	{
		var extents = new Vector3(wanderJitter, wanderJitter);
		wanderTarget += RandomUtil.RandomPosition(Vector3.zero, extents, Quaternion.identity);
		wanderTarget = wanderTarget.normalized * wanderRadius;
		var targetLocal = wanderTarget + Vector3.right * wanderDistance;
		var targetWorld = transform.TransformPoint(targetLocal);
		return targetWorld - transform.position;
	}
}