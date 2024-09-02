using UnityEditor;
using UnityEngine;

public static class PlacementTools
{
    [MenuItem("GameObject / Placement Tools / Align Ground")]
    private static void AlignGround()
    {
        GameObject currentSelection = Selection.activeGameObject;
        AlignGround(currentSelection);
    }

    private static void AlignGround(GameObject toBeMove)
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