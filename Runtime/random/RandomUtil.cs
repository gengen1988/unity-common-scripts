using System.Collections.Generic;
using UnityEngine;

public static class RandomUtil
{
    /**
     * 洗牌
     */
    public static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    /**
     * 随机采样 n 次的平均值，n 越大越近似正态分布
     * range in [0.0 .. 1.0] inclusive
     */
    public static float BatesSample(int n = 2)
    {
        float sum = 0;
        for (int i = 0; i < n; ++i)
        {
            sum += Random.value;
        }

        return sum / n;
    }

    /**
     * 有期望值的随机采样，三角分布
     * range in [0.0 .. 1.0] inclusive
     */
    public static float TriangularSample(float expect = .5f)
    {
        float value = Random.value;
        if (value < expect)
        {
            return Mathf.Sqrt(expect * value);
        }
        else
        {
            return 1 - Mathf.Sqrt((expect - 1) * (value - 1));
        }
    }

    /**
     * 从列表中随机选中一个 (并放回)
     */
    public static T Select<T>(IList<T> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("对空列表随机挑选，返回默认值");
            return default;
        }

        return list[Random.Range(0, list.Count)];
    }

    /**
     * 从列表中随机拿走一个 (不放回)
     */
    public static T Take<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogError("调用异常，返回默认值");
            return default;
        }

        int index = Random.Range(0, list.Count);
        T result = list[index];
        list.RemoveAt(index);
        return result;
    }

    /**
     * 给定一个成功机率，返回是否成功。n 越大越极端
     * 比方说 n 等于 10 时，0.8 的成功率比均匀分布更容易成功，但不影响 0.5
     */
    public static bool Check(float successRate, int n = 3)
    {
        float r = BatesSample(n);
        return r <= successRate;
    }

    public static Vector3 PointInBox(Vector3 center, Vector3 extents)
    {
        float x = Random.Range(-extents.x, extents.x);
        float y = Random.Range(-extents.y, extents.y);
        float z = Random.Range(-extents.z, extents.z);
        return center + new Vector3(x, y, z);
    }

    public static Vector3 PointInBox(Vector3 center, Vector3 extents, Quaternion rotation)
    {
        float x = Random.Range(-extents.x, extents.x);
        float y = Random.Range(-extents.y, extents.y);
        float z = Random.Range(-extents.z, extents.z);
        return center + rotation * new Vector3(x, y, z);
    }

    public static Vector2 Direction()
    {
        float angle = Random.Range(0, 360f);
        return MathUtil.VectorByAngle(angle);
    }

    public static Quaternion Rotation(float angleRange)
    {
        float halfRange = angleRange / 2;
        float r = BatesSample();
        float angle = MathUtil.Scale(r, 0, 1, -halfRange, halfRange);
        Quaternion rotation = MathUtil.QuaternionByAngle(angle);
        return rotation;
    }
}