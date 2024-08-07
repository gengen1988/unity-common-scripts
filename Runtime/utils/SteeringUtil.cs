using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class SteeringUtil
{
    /**
     * return delta velocity (acceleration = deltaVelocity / deltaTime)
     */
    public static Vector2 Seek(Vector2 los, Vector2 currentVelocity, float speed)
    {
        Vector3 desiredVelocity = los.normalized * speed;
        return VelocityMatch(currentVelocity, desiredVelocity);
    }

    public static Vector2 VelocityMatch(Vector2 currentVelocity, Vector2 desiredVelocity)
    {
        return desiredVelocity - currentVelocity;
    }

    public static Vector2 Flee(Vector2 los, Vector2 currentVelocity, float speed, float evadeDistance)
    {
        if (los.magnitude > evadeDistance)
        {
            return Vector3.zero;
        }

        return -Seek(los, currentVelocity, speed);
    }

    public static Vector2 Arrive(Vector2 los, Vector2 currentVelocity, float maxSpeed, float slowDistance = 0f)
    {
        if (Mathf.Approximately(slowDistance, 0f))
        {
            return Seek(los, currentVelocity, maxSpeed);
        }

        float ratio = Mathf.Clamp01(los.magnitude / slowDistance);
        return Seek(los, currentVelocity, ratio * maxSpeed);
    }

    public static Vector2 Pursue(
        Vector2 los,
        Vector2 currentVelocity,
        Vector2 targetVelocity,
        float maxSpeed,
        float slowDistance = 0f)
    {
        if (!KinematicUtil.InterceptTime(los, targetVelocity, maxSpeed, out float time))
        {
            return Seek(los, currentVelocity, maxSpeed);
        }

        Vector2 predictLos = los + targetVelocity * time;
        Vector2 result = Arrive(predictLos, currentVelocity, maxSpeed, slowDistance);
        return result;
    }

    public static Vector2 Wander(
        ref Vector2 wanderingTarget,
        Vector2 currentVelocity,
        float distance,
        float radius,
        float jitter)
    {
        Vector2 newWanderingTarget = wanderingTarget + RandomUtil.PointInDonut(jitter, 0);
        wanderingTarget = newWanderingTarget.normalized * radius;
        return currentVelocity.normalized * distance + wanderingTarget;
    }

    public static Vector2 Filter2D(Vector2 steering, Quaternion rotation, float sideScale, float rearScale)
    {
        Vector2 local = Quaternion.Inverse(rotation) * steering;
        local.x *= local.x < 0 ? rearScale : 1;
        local.y *= sideScale;
        Vector2 filtered = rotation * local;
        return filtered;
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

    public static Vector2 InputToSteering(Vector2 input, Quaternion rotation, float maxSteering)
    {
        Vector3 inputWorld = Quaternion.Euler(0, 0, -90f) * rotation * input;
        Vector3 steering = Vector3.ClampMagnitude(inputWorld, 1) * maxSteering;
        return steering;
    }

    public static Vector2 SteeringToInput(Vector2 steering, Quaternion rotation, float maxSteering)
    {
        float ratio = Mathf.Clamp01(steering.magnitude / maxSteering);
        Vector3 inputWorld = Vector3.ClampMagnitude(steering, ratio);
        Vector3 inputLocal = Quaternion.Euler(0, 0, 90f) * Quaternion.Inverse(rotation) * inputWorld;
        return inputLocal;
    }
}