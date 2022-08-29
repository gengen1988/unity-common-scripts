using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Util
{
	public static Vector2 MouseWorldPosition() => Camera.main.ScreenToWorldPoint(Input.mousePosition);
	public static GameObject FindPlayer() => GameObject.FindWithTag("Player");
	public static GameObject FindGameController() => GameObject.FindWithTag("GameController");

	public static void RemoveEmpty<T>(List<T> list) where T : Object => list.RemoveAll(item => !item);

	public static T[] WithoutEmpty<T>(IEnumerable<T> array) where T : Object
	{
		var newArray = from item in array where item select item;
		return newArray.ToArray();
	}

	public static void DestroyChildren(this Transform transform)
	{
		foreach (Transform child in transform)
		{
			Object.Destroy(child);
		}
	}

	public static Vector3 Los(this Transform self, MonoBehaviour target)
	{
		return target.transform.position - self.position;
	}
}