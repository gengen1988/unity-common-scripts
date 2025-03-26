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
        var n = list.Count;
        while (n > 1)
        {
            n--;
            var k = Random.Range(0, n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    /**
     * mean of multiple random value
     *
     */
    public static float BatesSample(int n = 2)
    {
        var sum = 0f;
        for (var i = 0; i < n; ++i)
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
        var value = Random.value;
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
     * 从列表中随机选中一个 (放回)
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
        var index = Random.Range(0, list.Count);
        var result = list[index];
        list.RemoveAt(index);
        return result;
    }

    /**
     * 给定一个成功机率，返回是否成功。n 越大越极端。
     * 比方说 n 等于 10 时，0.8 的成功率比均匀分布更容易成功，但不影响 0.5
     */
    public static bool Check(float successRate, int n = 3)
    {
        var clamped = Mathf.Clamp01(successRate);
        var r = BatesSample(n);
        return r <= clamped;
    }

    public static Vector3 PointInBox(Vector3 size)
    {
        var extents = size / 2;
        var x = Random.Range(-extents.x, extents.x);
        var y = Random.Range(-extents.y, extents.y);
        var z = Random.Range(-extents.z, extents.z);
        return new Vector3(x, y, z);
    }

    public static Vector3 PointInBox(float size)
    {
        var extents = size / 2;
        var x = Random.Range(-extents, extents);
        var y = Random.Range(-extents, extents);
        var z = Random.Range(-extents, extents);
        return new Vector3(x, y, z);
    }

    public static Vector2 PointInDonut(float outer = 1f, float inner = 1f)
    {
        var angle = Random.Range(0f, 360f);
        var length = Random.Range(inner, outer);
        var rad = angle * Mathf.Deg2Rad;
        var dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
        return dir * length;
    }

    /**
     * negative weights are not allowed (zero is fine)
     */
    public static int WeightedIndex(params float[] weights)
    {
        var totalWeight = weights.Sum();
        var randomValue = Random.Range(0, totalWeight);
        var cumulativeWeight = 0f;

        for (var i = 0; i < weights.Length; ++i)
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

    /**
     * x is between 0 and 1
     */
    public static float FBM(float x, int octaveCount, int seed)
    {
        var rng = new System.Random(seed);
        var octaveDelta = 1f / octaveCount;
        var octaveStart = 0f;
        var startValue = (float)rng.NextDouble();
        var endValue = (float)rng.NextDouble();
        while (x > octaveStart + octaveDelta)
        {
            startValue = endValue;
            endValue = (float)rng.NextDouble();
            octaveStart += octaveDelta;
        }

        var ratio = (x - octaveStart) / octaveDelta;
        var y = MathUtil.SmootherStep(startValue, endValue, ratio);
        return y;
    }
}