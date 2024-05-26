using UnityEngine;

public static class Kinematic
{
    /**
     * 在某一方向上，能达到最远距离的发射角 (重力作用下)
     */
    public static float RangeOptimizedAngle(Vector2 los)
    {
        float x = los.x;
        float y = los.y;
        float m = y / x;
        float sqrt = Mathf.Sqrt(m * m + 1);
        float angle = Mathf.Atan(m + sqrt) * Mathf.Rad2Deg;

        // 一四象限
        if (x < 0)
        {
            angle += 90f;
        }

        return angle;
    }

    /**
     * 命中某点的抛物线角度
     */
    public static bool AngleToHit(Vector2 los, float speed, out float shallow, out float steep)
    {
        float g = Mathf.Abs(Physics2D.gravity.y);
        return AngleToHit(los, speed, g, out shallow, out steep);
    }

    /**
     * 命中某点的抛物线角度
     */
    public static bool AngleToHit(Vector2 los, float speed, float g, out float shallow, out float steep)
    {
        float x = los.x;
        float y = los.y;
        float v2 = speed * speed;
        float v4 = v2 * v2;
        float x2 = x * x;
        float sqrt = Mathf.Sqrt(v4 - g * (g * x2 + 2 * y * v2));

        // 无法命中时，角度为在视线上能达到最远距离的角度
        if (float.IsNaN(sqrt))
        {
            shallow = steep = RangeOptimizedAngle(los);
            return false;
        }

        // 注意：如果目标与发射位置重合，则角度为 NaN
        float a1 = Mathf.Atan((v2 - sqrt) / (g * x)) * Mathf.Rad2Deg;
        float a2 = Mathf.Atan((v2 + sqrt) / (g * x)) * Mathf.Rad2Deg;

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
     * 跳到具体高度所需的初始速度
     */
    public static float JumpVelocityY(float height)
    {
        return JumpVelocityY(height, Physics2D.gravity.y);
    }

    /**
     * 跳到具体高度所需的初始速度
     */
    public static float JumpVelocityY(float height, float gravity)
    {
        return Mathf.Sqrt(Mathf.Abs(2 * height * gravity));
    }

    /**
     * 加速运动的位移
     */
    public static Vector3 Displacement(Vector3 velocity, Vector3 acceleration, float time)
    {
        return velocity * time + acceleration * (.5f * time * time);
    }

    /**
     * 命中匀速移动物体所需方向
     */
    public static bool InterceptVector(Vector3 los, Vector3 targetVelocity, float interceptSpeed, out Vector3 predict)
    {
        predict = Vector3.zero;
        float a = targetVelocity.sqrMagnitude - interceptSpeed * interceptSpeed;
        float b = 2f * Vector3.Dot(targetVelocity, los);
        float c = los.sqrMagnitude;

        // no solution, can't intercept
        if (MathUtil.Quadratic(a, b, c, out float t1, out float t2) == 0)
        {
            return false;
        }

        // can't intercept in negative time
        if (t1 < 0 && t2 < 0)
        {
            return false;
        }

        // determine short way
        if (t2 < t1)
        {
            (t1, t2) = (t2, t1);
        }

        float time = t1 < 0 ? t2 : t1;
        predict = los + targetVelocity * time;
        return true;
    }
}