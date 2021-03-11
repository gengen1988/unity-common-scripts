using UnityEngine;

public static class SteeringUtil
{
    public static Vector3 Seek(Vector3 los, Vector3 velocity, float maxSpeed)
    {
        var desiredVelocity = los.normalized * maxSpeed;
        return desiredVelocity - velocity;
    }

    public static Vector3 Seek(Vector3 position, Vector3 velocity, Vector3 target, float maxSpeed)
    {
        return Seek(target - position, velocity, maxSpeed);
    }

    public static Vector3 Evade(Vector3 los, Vector3 velocity, float maxSpeed, float evadeDistance)
    {
        if (los.magnitude > evadeDistance) return Vector3.zero;
        return -Seek(los, velocity, maxSpeed);
    }

    public static Vector3 Evade(Vector3 position, Vector3 velocity, Vector3 target, float maxSpeed, float evadeDistance)
    {
        return Evade(target - position, velocity, maxSpeed, evadeDistance);
    }

    public static Vector3 Arrive(Vector3 los, Vector3 velocity, float maxSpeed, float arrivalDistance)
    {
        var currentDistance = los.magnitude;
        if (currentDistance > arrivalDistance) return Seek(los, velocity, maxSpeed);

        var arrivalRatio = currentDistance / arrivalDistance;
        var timeToTarget = currentDistance / velocity.magnitude;
        return Seek(los, velocity, maxSpeed * arrivalRatio) / timeToTarget;
    }

    public static Vector3 Arrive(Vector3 position, Vector3 velocity, Vector3 target, float maxSpeed, float arrivalDistance)
    {
        return Arrive(target - position, velocity, maxSpeed, arrivalDistance);
    }

    public static Vector3 Pursuit(Vector3 los, Vector3 velocity, Vector3 evaderVelocity, float maxSpeed)
    {
        if (!Kinematic.InterceptTime(los, evaderVelocity, maxSpeed, out var timeRequired)) return Vector3.zero;

        var seekLos = los + evaderVelocity * timeRequired;
        return Seek(seekLos, velocity, maxSpeed);
        // TODO 处理迎面撞击，无需预测的情况
    }

    public static Vector3 Pursuit(Vector3 position, Vector3 velocity, Vector3 evaderPosition, Vector3 evaderVelocity, float maxSpeed)
    {
        return Pursuit(evaderPosition - position, velocity, evaderVelocity, maxSpeed);
    }
}