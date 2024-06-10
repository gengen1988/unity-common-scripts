using System.Collections.Generic;
using System.Linq;
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
     * mean of multiple random value
     *
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
     * generates a random number based on a triangular distribution with a specified expected value
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
        return list[Random.Range(0, list.Count)];
    }

    /**
     * 从列表中随机拿走一个 (不放回)
     */
    public static T Take<T>(List<T> list)
    {
        int index = Random.Range(0, list.Count);
        T result = list[index];
        list.RemoveAt(index);
        return result;
    }

    /**
     * 给定一个成功机率，返回是否成功。n 越大越极端。
     * 比方说 n 等于 10 时，0.8 的成功率比均匀分布更容易成功，但不影响 0.5
     */
    public static bool Check(float successRate, int n = 3)
    {
        float clamped = Mathf.Clamp01(successRate);
        float r = BatesSample(n);
        return r <= clamped;
    }

    public static Vector3 PointInBox(Vector3 size)
    {
        Vector3 extents = size / 2;
        float x = Random.Range(-extents.x, extents.x);
        float y = Random.Range(-extents.y, extents.y);
        float z = Random.Range(-extents.z, extents.z);
        return new Vector3(x, y, z);
    }

    public static Vector2 PointInDonut(float outer = 1f, float inner = 1f)
    {
        float angle = Random.Range(0f, 360f);
        float length = Random.Range(inner, outer);
        return MathUtil.VectorByAngle(angle) * length;
    }

    public static Quaternion Rotation(float angleRange)
    {
        float half = angleRange / 2;
        float r = BatesSample();
        float angle = MathUtil.RemapFrom01(r, -half, half);
        Quaternion rotation = MathUtil.QuaternionByAngle(angle);
        return rotation;
    }

    /**
     * negative weight is not allowed (zero is fine)
     */
    public static int WeightedIndex(params float[] weights)
    {
        float totalWeight = weights.Sum();
        float randomValue = Random.Range(0, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < weights.Length; ++i)
        {
            cumulativeWeight += weights[i];
            if (randomValue < cumulativeWeight)
            {
                return i;
            }
        }

        // In case of rounding errors, return the last index
        return weights.Length - 1;
    }
}