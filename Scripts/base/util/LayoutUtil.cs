using UnityEngine;
using UnityEngine.UI;

public static class LayoutUtil
{
    /**
     * considering add AspectFilter (fit in parent) to control instance size
     */
    public static GameObject InstantiateInLayout(GameObject prefab, float preferredSize, LayoutGroup parent)
    {
        var instance = Object.Instantiate(prefab, parent.transform);

        // setup preferred size
        var layoutElement = instance.EnsureComponent<LayoutElement>(true);
        switch (parent)
        {
            case HorizontalLayoutGroup:
                layoutElement.preferredWidth = preferredSize;
                break;
            case VerticalLayoutGroup:
                layoutElement.preferredHeight = preferredSize;
                break;
            default:
                Debug.LogError("preferred size is only available in horizontal or vertical layout");
                break;
        }

        return instance;
    }
}