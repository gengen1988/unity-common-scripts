using System;
using System.Collections.Generic;
using UnityEngine;

public static class SteeringUtil
{
    /**
     * return delta velocity (acceleration = deltaVelocity / deltaTime)
     */
    public static Vector3 Seek(Vector3 los, Vector3 currentVelocity, float maxSpeed)
    {
        Vector3 desiredVelocity = los.normalized * maxSpeed;
        return VelocityMatch(currentVelocity, desiredVelocity);
    }

    public static Vector3 VelocityMatch(Vector3 currentVelocity, Vector3 desiredVelocity)
    {
        return desiredVelocity - currentVelocity;
    }

    public static Vector3 Flee(Vector3 los, Vector3 currentVelocity, float maxSpeed, float evadeDistance)
    {
        if (los.magnitude > evadeDistance)
        {
            return Vector3.zero;
        }

        return -Seek(los, currentVelocity, maxSpeed);
    }

    public static Vector3 Arrive(Vector3 los, Vector3 currentVelocity, float maxSpeed, float slowDistance = 0f)
    {
        if (Mathf.Approximately(slowDistance, 0f))
        {
            return Seek(los, currentVelocity, maxSpeed);
        }

        float ratio = Mathf.Clamp01(los.magnitude / slowDistance);
        return Seek(los, currentVelocity, ratio * maxSpeed);
    }

    public static Vector3 Pursue(
        Vector3 los,
        Vector3 currentVelocity,
        Vector3 targetVelocity,
        float maxSpeed,
        float slowDistance = 0f)
    {
        if (!KinematicUtil.InterceptTime(los, targetVelocity, maxSpeed, out float time))
        {
            return Seek(los, currentVelocity, maxSpeed);
        }

        Vector3 predictVector = los + targetVelocity * time;
        return Arrive(predictVector, currentVelocity, maxSpeed, slowDistance);
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

    public static Vector2 Filter2D(Vector2 steering, Quaternion rotation, float sideScale, float rearScale)
    {
        Vector2 local = Quaternion.Inverse(rotation) * steering;
        local.x *= local.x < 0 ? rearScale : 1;
        local.y *= sideScale;
        Vector2 filtered = rotation * local;
        return filtered;
    }

    public static Vector3 Truncate(Vector3 steering, float maxAcceleration, float deltaTime)
    {
        return Vector3.ClampMagnitude(steering / deltaTime, maxAcceleration);
    }

    public static void ContextSteeringCircle(
        IEnumerable<Transform> targets,
        int count,
        Action<Transform, Vector2> callback)
    {
        float angleStep = 360f / count;
        foreach (Transform target in targets)
        {
            for (int i = 0; i < count; ++i)
            {
                Vector2 direction = Quaternion.Euler(0, 0, i * angleStep) * Vector3.right;
                callback(target, direction);
            }
        }
    }

    public static void ContextSteeringCircle2(
        Transform self,
        IEnumerable<Transform> targets,
        int count,
        Action<Transform, int, float> callback)
    {
        float angleStep = 360f / count;
        foreach (Transform target in targets)
        {
            Vector3 los = target.position - self.position;
            Vector3 losDirection = los.normalized;
            for (int i = 0; i < count; ++i)
            {
                Vector2 direction = Quaternion.Euler(0, 0, i * angleStep) * Vector3.right;
                float dot = Vector2.Dot(losDirection, direction);
                callback(target, i, dot);
            }
        }
    }

    public static IEnumerable<Vector2> GenerateDirectionCircle(int resolution)
    {
        float angleStep = 360f / resolution;
        for (int i = 0; i < resolution; ++i)
        {
            Vector2 direction = Quaternion.Euler(0, 0, i * angleStep) * Vector3.right;
            yield return direction;
        }
    }
}