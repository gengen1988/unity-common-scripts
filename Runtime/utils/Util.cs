using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Util
{
	public static GameObject FindPlayer()
	{
		return GameObject.FindWithTag("Player");
	}

	public static GameObject FindGameController()
	{
		return GameObject.FindWithTag("GameController");
	}

	public static Vector2 MouseWorldPosition()
	{
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}

	public static void RemoveEmpty<T>(List<T> list) where T : Object => list.RemoveAll(item => !item);

	public static T[] WithoutEmpty<T>(IEnumerable<T> array) where T : Object
	{
		var newArray = from item in array where item select item;
		return newArray.ToArray();
	}

	/**
	 * 清理 transform 下的子物体
	 */
	public static void DestroyChildren(this Transform root, Transform without = null)
	{
		if (Application.isPlaying)
		{
			foreach (Transform child in root)
			{
				if (child == without)
				{
					continue;
				}

				Object.Destroy(child.gameObject);
			}
		}
		else
		{
			// 在编辑阶段里只能用 DestroyImmediate，而且 DestroyImmediate 用 foreach 会导致漏删
			int skip = 0;
			while (root.childCount > skip)
			{
				Transform target = root.GetChild(skip);
				if (target == without)
				{
					skip++;
					continue;
				}

				Object.DestroyImmediate(target.gameObject);
			}
		}
	}

	public static T PopLast<T>(this IList<T> list)
	{
		T result = list[list.Count - 1];
		list.RemoveAt(list.Count - 1);
		return result;
	}
}