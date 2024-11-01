using System.Collections.Generic;
using UnityEngine;

public static class RaycastUtil
{
    /**
     * used in bullets with width
     */
    public static int ParallelRaycastAll(
        Vector2 origin,
        int rayCount,
        float castWidth,
        Vector2 direction,
        ContactFilter2D filter,
        List<RaycastHit2D> results,
        float distance,
        Dictionary<Collider2D, RaycastHit2D> workingMemory
    )
    {
        if (Mathf.Approximately(castWidth, 0))
        {
            rayCount = 1;
        }

        if (rayCount < 1)
        {
            rayCount = 1;
        }

        if (rayCount == 1)
        {
            return Physics2D.Raycast(origin, direction, filter, results, distance);
        }

        // Perform a series of parallel raycasts and store the results
        float halfWidth = castWidth / 2;
        float spacing = castWidth / (rayCount - 1);
        Vector2 normal = Vector2.Perpendicular(direction).normalized;

        // group by collider
        workingMemory.Clear();
        for (int i = 0; i < rayCount; i++)
        {
            Vector2 rayOrigin = origin + normal * (spacing * i - halfWidth);
            Physics2D.Raycast(rayOrigin, direction, filter, results, distance);
            foreach (RaycastHit2D hit in results)
            {
                Collider2D collider = hit.collider;
                if (workingMemory.TryGetValue(collider, out RaycastHit2D previousHit))
                {
                    if (previousHit.distance > hit.distance)
                    {
                        workingMemory[collider] = hit;
                    }
                }
                else
                {
                    workingMemory[collider] = hit;
                }
            }
        }

        // collect results
        results.Clear();
        results.AddRange(workingMemory.Values);
        return results.Count;
    }

    /**
     * return nearest hit
     */
    public static RaycastHit2D ParallelRaycast(
        Vector2 origin,
        int rayCount,
        float castWidth,
        Vector2 direction,
        float distance,
        LayerMask mask
    )
    {
        if (Mathf.Approximately(castWidth, 0))
        {
            rayCount = 1;
        }

        if (rayCount < 1)
        {
            rayCount = 1;
        }

        if (rayCount == 1)
        {
            return Physics2D.Raycast(origin, direction, distance, mask);
        }

        float halfWidth = castWidth / 2;
        float spacing = castWidth / (rayCount - 1);
        Vector2 normal = Vector2.Perpendicular(direction).normalized;
        RaycastHit2D result = default;
        for (int i = 0; i < rayCount; i++)
        {
            Vector2 rayOrigin = origin + normal * (spacing * i - halfWidth);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, distance, mask);

#if UNITY_EDITOR && DEBUG_DRAW
            Color debugColor = hit ? Color.red : Color.green;
            float debugDistance = hit ? hit.distance : distance;
            Vector2 debugVector = direction.normalized * debugDistance;
            // DebugUtil.DrawCross(rayOrigin, Color.blue);
            Debug.DrawLine(rayOrigin, rayOrigin + debugVector, debugColor);
#endif

            if (!hit)
            {
                continue;
            }

            if (!result || result.distance > hit.distance)
            {
                result = hit;
            }
        }

        if (result)
        {
            result.distance = Vector2.Dot(result.point - origin, direction);
        }

        return result;
    }

    public static RaycastHit2D PlatformerRaycast(
        Vector2 origin,
        int rayCount,
        float castWidth,
        float characterRadius,
        float shellThickness,
        Vector2 direction, // usually up or right
        float distance,
        LayerMask mask
    )
    {
        float actualWidth = castWidth - 2 * shellThickness;
        Vector2 actualOrigin = origin + direction.normalized * (characterRadius - shellThickness);
        float actualDistance = distance + shellThickness;
        RaycastHit2D hit = ParallelRaycast(actualOrigin, rayCount, actualWidth, direction, actualDistance, mask);
        hit.distance -= shellThickness;
        return hit;
    }
}