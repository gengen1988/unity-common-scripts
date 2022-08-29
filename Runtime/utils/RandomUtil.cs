using System.Collections.Generic;
using UnityEngine;

public static class RandomUtil
{
	public static float TriangularOffset(float delta) => (Random.value - Random.value) * delta;

	public static T RandomPick<T>(this IList<T> list)
	{
		return list[Random.Range(0, list.Count)];
	}

	public static T RandomTake<T>(this IList<T> list)
	{
		var index = Random.Range(0, list.Count);
		var result = list[index];
		list.RemoveAt(index);
		return result;
	}

	public static Vector3 Position(Vector3 center, Vector3 extents, Quaternion rotation)
	{
		var x = Random.Range(-extents.x, extents.x);
		var y = Random.Range(-extents.y, extents.y);
		var z = Random.Range(-extents.z, extents.z);
		return center + rotation * new Vector3(x, y, z);
	}

	public static Vector2 Direction()
	{
		var angle = Random.Range(0, 360);
		return MathUtil.AngleToVector(angle);
	}
}