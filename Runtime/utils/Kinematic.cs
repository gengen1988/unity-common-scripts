using UnityEngine;

public static class Kinematic
{
    // 命中某点的抛物线角度
    public static bool AngleRequiredToHit2D(Vector2 los, float initialSpeed, out float[] angles)
    {
        return AngleRequiredToHit2D(los, initialSpeed, Physics2D.gravity.magnitude, out angles);
    }

    public static bool AngleRequiredToHit2D(Vector2 los, float initialSpeed, float g, out float[] angles)
    {
        var x = los.x;
        var y = los.y;
        var v2 = initialSpeed * initialSpeed;
        var v4 = v2 * v2;
        var x2 = x * x;
        var sqrt = Mathf.Sqrt(v4 - g * (g * x2 + 2 * y * v2));
        angles = default;

        if (float.IsNaN(sqrt))
        {
            return false;
        }

        var results = new[]
        {
            Mathf.Atan((v2 - sqrt) / (g * x)) * Mathf.Rad2Deg,
            Mathf.Atan((v2 + sqrt) / (g * x)) * Mathf.Rad2Deg
        };

        if (x == 0)
        {
            angles = y > 0 ? new[] {90f, 90f} : new[] {-90f, -90f};
        }
        else if (x > 0)
        {
            angles = results;
        }
        else
        {
            angles = new[] {results[0] + 180f, results[1] + 180f};
        }

        return true;
    }

    public static float JumpVelocity(float height)
    {
        return JumpVelocity(height, Physics2D.gravity.y);
    }

    public static float JumpVelocity(float height, float gravity)
    {
        return Mathf.Sqrt(Mathf.Abs(2 * height * gravity));
    }

    // 加速运动的位移
    public static Vector3 AccelerateDisplacement(Vector3 initialVelocity, Vector3 acceleration, float time)
    {
        return initialVelocity * time + acceleration * (.5f * time * time);
    }

    // 子弹命中匀速移动物体所需时间
    public static bool InterceptTime(Vector3 los, Vector3 targetVelocity, float shotSpeed, out float timeRequired)
    {
        var a = targetVelocity.sqrMagnitude - shotSpeed * shotSpeed;
        var b = 2f * Vector3.Dot(targetVelocity, los);
        var c = los.sqrMagnitude;
        timeRequired = default;

        if (!MathUtil.Quadratic(a, b, c, out var t)) return false;

        if (t[0] > 0)
        {
            if (t[1] > 0)
            {
                timeRequired = Mathf.Min(t[0], t[1]);
                return true;
            }

            timeRequired = t[0];
            return true;
        }

        if (t[1] <= 0) return false;

        timeRequired = t[1];
        return true;
    }
}