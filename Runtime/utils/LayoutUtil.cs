using UnityEngine;
using UnityEngine.UI;

public static class LayoutUtil
{
	/**
     * considering add AspectFilter (fit in parent) to control instance size
     */
	public static T InstantiateInLayout<T>(T prefab, Transform parent, float preferredSize = -1) where T : Object
	{
		GameObject layoutIsolation = new GameObject("LayoutIsolation");

		// setup preferred size
		if (preferredSize >= 0)
		{
			LayoutElement layoutElement = layoutIsolation.AddComponent<LayoutElement>();
			LayoutGroup layoutGroup = parent.GetComponent<LayoutGroup>();
			switch (layoutGroup)
			{
				case HorizontalLayoutGroup _:
					layoutElement.preferredWidth = preferredSize;
					break;
				case VerticalLayoutGroup _:
					layoutElement.preferredHeight = preferredSize;
					break;
				default:
					Debug.LogError("preferred size is only available in horizontal or vertical layout");
					break;
			}
		}

		Transform isolatedTransform = layoutIsolation.transform;
		isolatedTransform.SetParent(parent, false);
		return Object.Instantiate(prefab, isolatedTransform);
	}
}