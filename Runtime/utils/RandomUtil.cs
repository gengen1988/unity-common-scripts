using System.Collections.Generic;
using UnityEngine;

public static class RandomUtil
{
	public static float TriangularOffset(float delta) => (Random.value - Random.value) * delta;

	/**
	 * 洗牌
	 */
	public static void Shuffle<T>(this IList<T> list)
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
	 * 从列表中随机选中一个
	 */
	public static T RandomPick<T>(this IList<T> list)
	{
		return list[Random.Range(0, list.Count)];
	}

	/**
	 * 从列表中随机拿走一个
	 */
	public static T RandomTake<T>(this IList<T> list)
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
		float roll = Random.value;
		if (roll > successRate)
		{
			// Debug.Log($"随机检定失败：掷出 {roll} (成功率 {successRate})");
			return false;
		}

		return true;
	}

	/**
	 * 有期望值的随机数，三角分布
	 */
	public static float RandomValueWithExpect(float from, float to, float expect)
	{
		float delta = to - from;
		float distance = expect - from;
		float ratio = distance / delta;
		return from + RandomValueWithExpect(ratio) * delta;
	}

	/**
	 * 有期望值的随机数，三角分布
	 */
	public static float RandomValueWithExpect(float expect)
	{
		float value = Random.value;
		float result;
		if (value < expect)
		{
			result = Mathf.Sqrt(expect * value);
		}
		else
		{
			result = 1 - Mathf.Sqrt((expect - 1) * (value - 1));
		}

		return result;
	}

	public static Vector3 RandomPosition(Vector3 center, Vector3 extents, Quaternion rotation)
	{
		var x = Random.Range(-extents.x, extents.x);
		var y = Random.Range(-extents.y, extents.y);
		var z = Random.Range(-extents.z, extents.z);
		return center + rotation * new Vector3(x, y, z);
	}

	public static Vector2 RandomDirection()
	{
		var angle = Random.Range(0, 360);
		return MathUtil.AngleToVector(angle);
	}
}