using System;
using System.Collections.Generic;
using UnityEngine;

public static class SteeringUtil
{
    /**
     * return delta velocity (acceleration = deltaVelocity / deltaTime)
     */
    public static Vector2 Seek(Vector2 los, Vector2 currentVelocity, float speed)
    {
        var desiredVelocity = los.normalized * speed;
        return desiredVelocity - currentVelocity;
    }

    public static Vector2 VelocityMatch(Vector2 currentVelocity, Vector2 desiredVelocity)
    {
        return desiredVelocity - currentVelocity;
    }

    public static Vector2 Flee(Vector2 los, Vector2 currentVelocity, float speed, float evadeDistance = float.PositiveInfinity)
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

        var ratio = Mathf.Clamp01(los.magnitude / slowDistance);
        return Seek(los, currentVelocity, ratio * maxSpeed);
    }

    public static Vector2 Pursue(
        Vector2 los,
        Vector2 currentVelocity,
        Vector2 targetVelocity,
        float maxSpeed,
        float slowDistance = 0f)
    {
        if (!KinematicUtil.InterceptTime(los, targetVelocity, maxSpeed, out var time))
        {
            return Seek(los, currentVelocity, maxSpeed);
        }

        var predictLos = los + targetVelocity * time;
        var result = Arrive(predictLos, currentVelocity, maxSpeed, slowDistance);
        return result;
    }

    public static Vector2 Wander(
        ref Vector2 wanderingTarget,
        Vector2 currentVelocity,
        float distance,
        float radius,
        float jitter)
    {
        var newWanderingTarget = wanderingTarget + RandomUtil.PointInDonut(jitter, 0);
        wanderingTarget = newWanderingTarget.normalized * radius;
        return currentVelocity.normalized * distance + wanderingTarget;
    }

    public static Vector2 Filter2D(Vector2 steering, Quaternion rotation, float sideScale, float rearScale)
    {
        var local = Quaternion.Inverse(rotation) * steering;
        local.x *= local.x < 0 ? rearScale : 1;
        local.y *= sideScale;
        var filtered = rotation * local;
        return filtered;
    }

    public static void ContextSteeringCircle(
        IEnumerable<Transform> targets,
        int count,
        Action<Transform, Vector2> callback)
    {
        var angleStep = 360f / count;
        foreach (var target in targets)
        {
            for (var i = 0; i < count; ++i)
            {
                var direction = Quaternion.Euler(0, 0, i * angleStep) * Vector3.right;
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
        var angleStep = 360f / count;
        foreach (var target in targets)
        {
            var los = target.position - self.position;
            var losDirection = los.normalized;
            for (var i = 0; i < count; ++i)
            {
                var direction = Quaternion.Euler(0, 0, i * angleStep) * Vector3.right;
                var dot = Vector2.Dot(losDirection, direction);
                callback(target, i, dot);
            }
        }
    }

    public static IEnumerable<Vector2> GenerateDirectionCircle(int resolution)
    {
        var angleStep = 360f / resolution;
        for (var i = 0; i < resolution; ++i)
        {
            Vector2 direction = Quaternion.Euler(0, 0, i * angleStep) * Vector3.right;
            yield return direction;
        }
    }

    public static Vector2 InputToSteering(Vector2 input, Quaternion rotation, float maxSteering)
    {
        var inputWorld = rotation * input;
        var steering = Vector3.ClampMagnitude(inputWorld, 1) * maxSteering;
        return steering;
    }

    public static Vector2 SteeringToInput(Vector2 steering, Quaternion rotation, float maxSteering)
    {
        var ratio = Mathf.Clamp01(steering.magnitude / maxSteering);
        var inputWorld = steering.normalized * ratio;
        var inputLocal = Quaternion.Inverse(rotation) * inputWorld;
        return inputLocal;
    }
}