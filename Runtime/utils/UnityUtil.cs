using UnityEngine;

public static class UnityUtil
{
	public static GameObject FindPlayer()
	{
		return GameObject.FindWithTag("Player");
	}

	public static Vector2 MouseWorldPosition()
	{
		return Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
}