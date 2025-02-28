using System;
using UnityEngine;

public static class KinematicUtil
{
    /**
     * 在某一方向上，能达到最远距离的发射角 (重力作用下)
     */
    public static float RangeOptimizedAngle(Vector2 los)
    {
        var x = los.x;
        var y = los.y;
        var m = y / x;
        var sqrt = Mathf.Sqrt(m * m + 1);
        var angle = Mathf.Atan(m + sqrt) * Mathf.Rad2Deg;

        // 一四象限
        if (x < 0)
        {
            angle += 90f;
        }

        return angle;
    }

    /**
     * 命中某点的抛物线角度。重力加速度是正数
     */
    public static bool AngleToHit(Vector2 los, float speed, float g, out float shallow, out float steep)
    {
        var x = los.x;
        var y = los.y;

        // 如果目标与发射位置重合，则角度为 NaN
        if (Mathf.Approximately(x, 0) && Mathf.Approximately(y, 0))
        {
            shallow = steep = float.NaN;
            return true;
        }

        var v2 = speed * speed;
        var v4 = v2 * v2;
        var x2 = x * x;
        var toBeSqrt = v4 - g * (g * x2 + 2 * y * v2);

        // 无法命中时，角度为在视线上能达到最远距离的角度
        if (toBeSqrt < 0)
        {
            shallow = steep = RangeOptimizedAngle(los);
            return false;
        }

        var sqrt = Mathf.Sqrt(toBeSqrt);
        var a1 = Mathf.Atan((v2 - sqrt) / (g * x)) * Mathf.Rad2Deg;
        var a2 = Mathf.Atan((v2 + sqrt) / (g * x)) * Mathf.Rad2Deg;

        // 一四象限
        if (x < 0)
        {
            a1 += 180f;
            a2 += 180f;
        }

        shallow = a1;
        steep = a2;
        return true;
    }

    /**
     * 命中某点的抛物线角度。使用引擎内置的重力
     */
    public static bool AngleToHit(Vector2 los, float speed, out float shallow, out float steep)
    {
        var g = Mathf.Abs(Physics2D.gravity.y);
        return AngleToHit(los, speed, g, out shallow, out steep);
    }

    /**
     * 跳到具体高度所需的初始速度
     */
    public static float JumpVelocityY(float height, float g)
    {
        return Mathf.Sqrt(Mathf.Abs(2 * height * g));
    }

    /**
     * 跳到具体高度所需的初始速度
     */
    public static float JumpVelocityY(float height)
    {
        return JumpVelocityY(height, Physics2D.gravity.y);
    }

    /**
     * 加速运动的位移
     */
    public static Vector3 Displacement(Vector3 velocity, Vector3 acceleration, float time)
    {
        // v0 * t + 0.5 * a * t * t
        return time * (velocity + .5f * time * acceleration);
    }

    /**
     * 命中匀速移动物体所需速度
     */
    public static bool InterceptTime(
        Vector3 los,
        Vector3 targetVelocity,
        float projectileSpeed,
        out float collisionTime)
    {
        collisionTime = float.NaN;
        var a = targetVelocity.sqrMagnitude - projectileSpeed * projectileSpeed;
        var b = 2f * Vector3.Dot(targetVelocity, los);
        var c = los.sqrMagnitude;

        // no solution, can't intercept
        if (MathUtil.Quadratic(a, b, c, out var t1, out var t2) == 0)
        {
            return false;
        }

        // can't intercept in negative time
        if (t1 < 0 && t2 < 0)
        {
            return false;
        }

        // determine the shorter time
        if (t1 < 0)
        {
            collisionTime = t2;
        }
        else if (t2 < 0)
        {
            collisionTime = t1;
        }
        else
        {
            collisionTime = Mathf.Min(t1, t2);
        }

        return true;
    }
}