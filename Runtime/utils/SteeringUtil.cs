using UnityEngine;

public static class SteeringUtil
{
    public static Vector3 Seek(Vector3 los, Vector3 velocity, float maxSpeed)
    {
        Vector3 desiredVelocity = los.normalized * maxSpeed;
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
        float scale = Mathf.Clamp01(los.magnitude / arrivalDistance);
        return Seek(los, velocity, maxSpeed * scale);
    }

    public static Vector3 Pursue(
        Vector3 los,
        Vector3 selfVelocity,
        Vector3 targetVelocity,
        float maxSpeed,
        float arrivalDistance = 0f)
    {
        if (!Kinematic.InterceptVector(los, targetVelocity, maxSpeed, out Vector3 predictVector))
        {
            return Vector3.zero;
        }

        if (arrivalDistance > 0)
        {
            return Arrive(predictVector, selfVelocity, maxSpeed, arrivalDistance);
        }
        else
        {
            return Seek(predictVector, selfVelocity, maxSpeed);
        }
    }

    public static Vector3 Wander(
        Transform transform,
        float wanderDistance,
        float wanderRadius,
        float wanderJitter,
        ref Vector3 wanderTarget)
    {
        wanderTarget += (Vector3)RandomUtil.PointInDonut(wanderJitter, wanderJitter);
        wanderTarget = wanderTarget.normalized * wanderRadius;
        Vector3 targetLocal = wanderTarget + Vector3.right * wanderDistance;
        Vector3 targetWorld = transform.TransformPoint(targetLocal);
        return targetWorld - transform.position;
    }
}