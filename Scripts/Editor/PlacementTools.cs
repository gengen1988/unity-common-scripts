using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PlacementTools
{
    [MenuItem("GameObject / Placement Tools / Align Ground")]
    private static void AlignGround3D(MenuCommand cmd)
    {
        GameObject go = cmd.context as GameObject;
        AlignGround3D(go);
    }

    [MenuItem("GameObject / Placement Tools / Distribute X")]
    private static void DistributeX(MenuCommand cmd)
    {
        // bypass single context call
        if (Selection.objects.Length > 1 && cmd.context != Selection.activeObject)
        {
            return;
        }

        List<GameObject> selected = new List<GameObject>();
        selected.AddRange(Selection.gameObjects);
        selected.RemoveAll(AssetDatabase.Contains);
        if (selected.Count == 0)
        {
            Debug.Log("no scene object selected");
            return;
        }

        Bounds bounds = default;
        for (int i = 0; i < selected.Count; i++)
        {
            GameObject go = selected[i];
            if (i == 0)
            {
                bounds = new Bounds(go.transform.position, Vector3.zero);
            }
            else
            {
                bounds.Encapsulate(go.transform.position);
            }

            Debug.Log($"point {go.transform.position}; bounds: {bounds}");
        }

        float totalWidth = bounds.size.x;

        // Calculate the spacing between each object
        float spacing = totalWidth / (selected.Count - 1);

        // Distribute the objects evenly along the x-axis within the bounds
        Undo.IncrementCurrentGroup();
        Vector3 center = bounds.center;
        for (int i = 0; i < selected.Count; i++)
        {
            float offset = i - (selected.Count - 1) / 2.0f;
            Transform trans = selected[i].transform;
            Vector3 newPosition = center + Vector3.right * offset * spacing;
            Undo.RecordObject(trans, "Change Position");
            trans.position = newPosition;
        }

        Undo.SetCurrentGroupName("Distribute Objects");
    }

    private static void AlignGround3D(GameObject toBeMove)
    {
        Transform trans = toBeMove.transform;
        Vector3 origin = trans.position;
        RaycastHit[] results = Physics.RaycastAll(origin, Vector3.down);
        Vector3? hitPoint = null;
        foreach (RaycastHit hit in results)
        {
            if (hit.transform == trans)
            {
                continue;
            }

            hitPoint = hit.point;
            break;
        }

        if (!hitPoint.HasValue)
        {
            Debug.LogWarning("Ground not found", toBeMove);
            return;
        }

        Collider collider = toBeMove.GetComponent<Collider>();
        Vector3 closestPoint = collider.ClosestPoint(hitPoint.Value);
        Vector3 offset = origin - closestPoint;
        Vector3 destinationPoint = hitPoint.Value + offset;

        Undo.RecordObject(trans, "Align Ground");
        trans.position = destinationPoint;

        Debug.Log("Alignment complete. Final position: " + trans.position);
    }
}