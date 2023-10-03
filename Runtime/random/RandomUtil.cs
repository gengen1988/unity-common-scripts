using System.Collections.Generic;
using UnityEngine;

public static class RandomUtil
{
    /**
     * 洗牌
     */
    public static void Shuffle<T>(this IList<T> list)
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
     * 从列表中随机选中一个
     */
    public static T RandomSelect<T>(this IList<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    /**
     * 从列表中随机拿走一个
     */
    public static T RandomTake<T>(this List<T> list)
    {
        var index = Random.Range(0, list.Count);
        var result = list[index];
        list.RemoveAt(index);
        return result;
    }

    /**
     * 给定一个成功机率，返回是否成功
     */
    public static bool Check(float successRate)
    {
        var roll = Random.value;
        if (roll > successRate)
        {
            return false;
        }

        return true;
    }

    public static float TriangularSample(float spread)
    {
        return (Random.value - Random.value) * spread / 2;
    }

    /**
     * 有期望值的随机采样，三角分布
     * 0 to 1
     */
    public static float SampleWithExpect(float expect = .5f)
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
     * 有期望值的随机数，三角分布
     * expect 在 from to 之间
     */
    public static float SampleWithExpect(float from, float to, float expect)
    {
        var delta = to - from;
        var distance = expect - from;
        var ratio = distance / delta;
        return from + SampleWithExpect(ratio) * delta;
    }

    public static Vector3 RandomPointInBox(Vector3 center, Vector3 extents)
    {
        var x = Random.Range(-extents.x, extents.x);
        var y = Random.Range(-extents.y, extents.y);
        var z = Random.Range(-extents.z, extents.z);
        return center + new Vector3(x, y, z);
    }

    public static Vector3 RandomPointInBox(Vector3 center, Vector3 extents, Quaternion rotation)
    {
        var x = Random.Range(-extents.x, extents.x);
        var y = Random.Range(-extents.y, extents.y);
        var z = Random.Range(-extents.z, extents.z);
        return center + rotation * new Vector3(x, y, z);
    }

    public static Vector2 RandomDirection()
    {
        var angle = Random.Range(0, 360f);
        return MathUtil.VectorByAngle(angle);
    }
}